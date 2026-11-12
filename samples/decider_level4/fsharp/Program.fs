open System
open System.IO
open System.Diagnostics
open System.Text.Json
open System.Text.Json.Nodes
open System.Collections.Generic
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open SchoolDomain
open Playground.Dcb

exception HttpError of int*string
let options=JsonSerializerOptions(PropertyNamingPolicy=JsonNamingPolicy.CamelCase,AllowOutOfOrderMetadataProperties=true,RespectRequiredConstructorParameters=true)
let encode value=JsonSerializer.Serialize(value,options)
let bad()=raise(HttpError(400,"InvalidRequest"))
let spec=JsonDocument.Parse(File.ReadAllText(Environment.GetEnvironmentVariable("API_SPEC") |> nonNull))
let schemas=spec.RootElement.GetProperty("components").GetProperty("schemas")
let prop name (e:JsonElement)=match e.TryGetProperty(name:string) with true,v->Some v | _->None
let rec validate (body:JsonElement) (schema:JsonElement)=
    match prop "$ref" schema,prop "oneOf" schema with
    | Some reference,_->validate body (schemas.GetProperty((reference.GetString() |> nonNull).Split('/') |> Array.last))
    | _,Some choices->
        if body.ValueKind<>JsonValueKind.Object then bad()
        let kind=match prop "$type" body with Some v when v.ValueKind=JsonValueKind.String->v.GetString() |> nonNull | _->bad()
        if not(choices.EnumerateArray() |> Seq.exists(fun c->c.GetProperty("$ref").GetString()="#/components/schemas/"+kind)) then bad()
        validate body (schemas.GetProperty(kind))
    | _->
        match schema.GetProperty("type").GetString() with
        | "object"->
            if body.ValueKind<>JsonValueKind.Object then bad()
            let properties=schema.GetProperty("properties")
            for key in schema.GetProperty("required").EnumerateArray() do if prop (key.GetString() |> nonNull) body |> Option.isNone then bad()
            for field in body.EnumerateObject() do match prop field.Name properties with Some child->validate field.Value child | None->bad()
        | "string"->
            if body.ValueKind<>JsonValueKind.String then bad()
            let s=body.GetString() |> nonNull
            match prop "const" schema with Some c when c.GetString()<>s->bad() | _->()
            match prop "enum" schema with Some values when not(values.EnumerateArray() |> Seq.exists(fun v->v.GetString()=s))->bad() | _->()
            if Option.isSome(prop "minLength" schema) && (String.IsNullOrWhiteSpace(s)||(s.EnumerateRunes() |> Seq.length)>128) then bad()
        | "integer"->
            if body.ValueKind<>JsonValueKind.Number then bad()
            let n=body.GetDouble()
            if n<>Math.Truncate(n)||n<float Int32.MinValue||n>float Int32.MaxValue then bad()
        | _->bad()
let rec normalize name (value:JsonNode)=
    match value with
    | :? JsonObject as obj->
        for key in obj |> Seq.map(fun p->p.Key) |> Seq.toArray do
            let child=obj[key] |> nonNull
            obj.Remove(key) |> ignore
            obj[key]<-normalize key child
        value
    | :? JsonArray as items->
        for i in 0..items.Count-1 do
            let child=items[i] |> nonNull
            items[i]<-null
            items[i]<-normalize "" child
        value
    | :? JsonValue as scalar when (name="progress"||name="prerequisite") && (scalar.ToJsonString()="\"Reserved\""||scalar.ToJsonString()="\"NoPrerequisite\"")->JsonNode.Parse("{\"$type\":"+scalar.ToJsonString()+"}") |> nonNull
    | _->value
let json value=JsonNode.Parse(encode value) |> nonNull |> normalize ""
let journal:IJsonJournal=match Environment.GetEnvironmentVariable("DECIDER_PERSISTENT") with null->new MemoryJsonJournal() | path->new SqliteJournal(path,"FULL")
let store=new JsonEventStore<Event>(journal,Routing.describe,(fun e->encode e),(fun s->JsonSerializer.Deserialize<Event>(s,options) |> nonNull))
let projection=SchoolProjection()
do store.Subscribe projection.Accept
let executor=DcbExecutor<Event,Rejection>(store,3)
let tickets=Dictionary<string,int64*PreparedCommand<Rejection>>()
let ticketGate=obj()
let ttl=match Environment.GetEnvironmentVariable("TICKET_TTL_SECONDS") with null->60. | s->Double.Parse(s,Globalization.CultureInfo.InvariantCulture)
let expire()=for key in tickets |> Seq.filter(fun p->Stopwatch.GetElapsedTime(fst p.Value).TotalSeconds>=ttl) |> Seq.map(fun p->p.Key) |> Seq.toArray do tickets.Remove(key) |> ignore
let commit (ticket:PreparedCommand<Rejection>)=
    let r=ticket.Commit()
    (if r.Status="Committed" then 200 elif r.Status="Rejected" then 422 else 409),json {|status=r.Status;conflicts=r.Conflicts;appended=r.Appended;position=r.Position;projectionPosition=projection.Checkpoint;errors=r.Errors|}
let builder=WebApplication.CreateBuilder()
do builder.WebHost.UseUrls("http://127.0.0.1:"+(match Environment.GetEnvironmentVariable("PORT") with null->"8080" | s->s)) |> ignore
do builder.Logging.ClearProviders() |> ignore
let app=builder.Build()
let handle (context:HttpContext):Task=(task {
    let mutable code=200
    let mutable body=json {|error="InternalError"|}
    try
        let route=context.Request.Path.Value |> nonNull
        let get=List.contains route ["/health";"/school";"/status"]||route.StartsWith("/students/")
        let post=List.contains route ["/commands";"/tickets"]||System.Text.RegularExpressions.Regex.IsMatch(route,"^/tickets/[^/]+/commit$")
        if not(get||post) then raise(HttpError(404,"NotFound"))
        let method=if get then "GET" else "POST"
        if context.Request.Method<>method then
            context.Response.Headers.Allow<-Microsoft.Extensions.Primitives.StringValues(method)
            raise(HttpError(405,"MethodNotAllowed"))
        if get then
            store.Refresh()
            if store.ListenerFaults<>0 then failwith "projection unavailable"
            body<-
                match route with
                | "/health"->json {|status="ok"|}
                | "/status"->json {|eventPosition=(store.Read {Items=[]}).After;projectionPosition=projection.Checkpoint;listenerFaults=store.ListenerFaults|}
                | "/school"->let position,state=projection.Snapshot() in json {|position=position;state=state|}
                | _->match projection.FindStudent(route.Substring(10)) with position,Some student->json {|position=position;student=student|} | _,None->raise(HttpError(404,"NotFound"))
        elif route="/commands"||route="/tickets" then
            let content=match context.Request.ContentType with null->"" | s->(s.Split(';')[0]).Trim()
            if content<>"application/json" then raise(HttpError(415,"UnsupportedMediaType"))
            use memory=new MemoryStream()
            let buffer=Array.zeroCreate<byte> 8192
            let mutable reading=true
            while reading do
                let! n=context.Request.Body.ReadAsync(buffer.AsMemory())
                if n=0 then reading<-false else
                    if memory.Length+int64 n>1048576L then raise(HttpError(413,"PayloadTooLarge"))
                    memory.Write(buffer,0,n)
            use input=JsonDocument.Parse(memory.ToArray())
            validate input.RootElement (schemas.GetProperty("CommandRequest"))
            let id=input.RootElement.GetProperty("commandId").GetString() |> nonNull
            let rawCommand=input.RootElement.GetProperty("command")
            let numeric=rawCommand.EnumerateObject() |> Seq.filter(fun p->p.Value.ValueKind=JsonValueKind.Number && not(fst(p.Value.TryGetInt32()))) |> Seq.toArray
            let command=
                if numeric.Length=0 then JsonSerializer.Deserialize<Command>(rawCommand,options) |> nonNull
                else
                    let node=JsonNode.Parse(rawCommand.GetRawText()) |> nonNull
                    for p in numeric do node[p.Name]<-JsonValue.Create(int(p.Value.GetDouble()))
                    JsonSerializer.Deserialize<Command>(node.ToJsonString(),options) |> nonNull
            let prepared=executor.Prepare(id,command,Handlers.school)
            if route="/commands"||not prepared.WasRead then let c,b=commit prepared in code<-c;body<-b
            else
                let c,b=lock ticketGate (fun()->
                    expire()
                    if tickets.Count>=1024 then raise(HttpError(429,"TooManyTickets"))
                    let id=Guid.NewGuid().ToString("N")
                    tickets.Add(id,(Stopwatch.GetTimestamp(),prepared))
                    201,json {|ticket=id;position=prepared.After|})
                code<-c;body<-b
        else
            let prepared=lock ticketGate (fun()->
                expire()
                let key=route.Split('/')[2]
                match tickets.TryGetValue(key) with true,(_,t)->tickets.Remove(key) |> ignore;t | _->raise(HttpError(404,"NotFound")))
            let c,b=commit prepared
            code<-c;body<-b
    with
    | HttpError(c,message)->code<-c;body<-json {|error=message|}
    | :? JsonException->code<-400;body<-json {|error="InvalidRequest"|}
    | error->Console.Error.WriteLine(error);code<-500;body<-json {|error="InternalError"|}
    context.Response.StatusCode<-code
    context.Response.ContentType<-"application/json"
    do! context.Response.WriteAsync(body.ToJsonString())
} :> Task)
app.Run(RequestDelegate(handle))
app.Lifetime.ApplicationStopping.Register(fun()->(store:>IDisposable).Dispose()) |> ignore
app.Run()

