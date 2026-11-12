using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Playground.Dcb;
using SchoolDomain;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:" + (Environment.GetEnvironmentVariable("PORT") ?? "8080"));
builder.Logging.ClearProviders();
builder.WebHost.ConfigureKestrel(o => o.Limits.MaxRequestBodySize = 1024 * 1024);
var app = builder.Build();
using var contract = JsonDocument.Parse(File.ReadAllText(Environment.GetEnvironmentVariable("API_SPEC")!));
var schemas = contract.RootElement.GetProperty("components").GetProperty("schemas");
var path = Environment.GetEnvironmentVariable("DECIDER_PERSISTENT");
using var store = new JsonEventStore<Event>(path is null ? new MemoryJsonJournal() : new SqliteJournal(path, "FULL"), Routing.Describe,
    e => JsonSerializer.Serialize(e, Wire.Default.Event), s => JsonSerializer.Deserialize(s, Wire.Default.Event)!);
var projection = new SchoolProjection(); store.Subscribe(projection.Accept);
var executor = new DcbExecutor<Event, Rejection>(store); var handler = new SchoolHandler();
var tickets = new Dictionary<string, (long Created, PreparedCommand<Rejection> Command)>(); var ticketGate = new object();
var ttl = double.Parse(Environment.GetEnvironmentVariable("TICKET_TTL_SECONDS") ?? "60", System.Globalization.CultureInfo.InvariantCulture);
void Expire() { foreach (var key in tickets.Where(p => Stopwatch.GetElapsedTime(p.Value.Created).TotalSeconds >= ttl).Select(p => p.Key).ToArray()) tickets.Remove(key); }
JsonObject Result(HandlingResult<Rejection> r) => new() { ["status"] = r.Status, ["conflicts"] = r.Conflicts, ["appended"] = r.Appended, ["position"] = r.Position, ["projectionPosition"] = projection.Checkpoint, ["errors"] = new JsonArray(r.Errors.Select(e => JsonValue.Create(e.ToString())).ToArray()) };
void Validate(JsonElement body, JsonElement schema)
{
    if (schema.TryGetProperty("$ref", out var reference)) { Validate(body, schemas.GetProperty(reference.GetString()!.Split('/')[^1])); return; }
    if (schema.TryGetProperty("oneOf", out var choices)) { if (body.ValueKind != JsonValueKind.Object || !body.TryGetProperty("$type", out var kind) || kind.ValueKind != JsonValueKind.String || !choices.EnumerateArray().Any(x => x.GetProperty("$ref").GetString() == "#/components/schemas/" + kind.GetString())) throw new JsonException(); Validate(body, schemas.GetProperty(kind.GetString()!)); return; }
    string type = schema.GetProperty("type").GetString()!;
    if (type == "object")
    {
        if (body.ValueKind != JsonValueKind.Object) throw new JsonException(); var properties = schema.GetProperty("properties");
        foreach (var key in schema.GetProperty("required").EnumerateArray()) if (!body.TryGetProperty(key.GetString()!, out _)) throw new JsonException();
        foreach (var property in body.EnumerateObject()) { if (!properties.TryGetProperty(property.Name, out var child)) throw new JsonException(); Validate(property.Value, child); }
    }
    else if (type == "string")
    {
        if (body.ValueKind != JsonValueKind.String) throw new JsonException(); var s = body.GetString()!;
        if (schema.TryGetProperty("const", out var c) && s != c.GetString()) throw new JsonException();
        if (schema.TryGetProperty("enum", out var values) && !values.EnumerateArray().Any(x => x.GetString() == s)) throw new JsonException();
        if (schema.TryGetProperty("minLength", out _) && (string.IsNullOrWhiteSpace(s) || s.EnumerateRunes().Count() > 128)) throw new JsonException();
    }
    else if (type == "integer" && (body.ValueKind != JsonValueKind.Number || !body.TryGetDouble(out var n) || n != Math.Truncate(n) || n < int.MinValue || n > int.MaxValue)) throw new JsonException();
}
app.Run(async context =>
{
    int code = 200; JsonNode body = new JsonObject();
    try
    {
        var route = context.Request.Path.Value!; var method = context.Request.Method;
        bool get = route is "/health" or "/school" or "/status" || route.StartsWith("/students/");
        bool post = route is "/commands" or "/tickets" || System.Text.RegularExpressions.Regex.IsMatch(route, @"^/tickets/[^/]+/commit$");
        if (!get && !post) { code = 404; throw new ApiError("NotFound"); }
        if (method != (get ? "GET" : "POST")) { code = 405; context.Response.Headers.Allow = get ? "GET" : "POST"; throw new ApiError("MethodNotAllowed"); }
        if (get)
        {
            store.Refresh(); if (store.ListenerFaults != 0) throw new InvalidOperationException("projection unavailable");
            if (route == "/health") body = new JsonObject { ["status"] = "ok" };
            else if (route == "/status") body = new JsonObject { ["eventPosition"] = store.Read(new([])).After, ["projectionPosition"] = projection.Checkpoint, ["listenerFaults"] = store.ListenerFaults };
            else if (route == "/school") { var s = projection.Snapshot(); body = new JsonObject { ["position"] = s.Position, ["state"] = JsonSerializer.SerializeToNode(s.State, Wire.Default.SchoolList) }; }
            else { var s = projection.FindStudent(route[10..]); if (s.State.IsEmpty) { code = 404; throw new ApiError("NotFound"); } body = new JsonObject { ["position"] = s.Position, ["student"] = JsonSerializer.SerializeToNode(s.State[0], Wire.Default.Student) }; }
        }
        else if (route is "/commands" or "/tickets")
        {
            if (context.Request.ContentType?.Split(';')[0].Trim() != "application/json") { code = 415; throw new ApiError("UnsupportedMediaType"); }
            using var input = await JsonDocument.ParseAsync(context.Request.Body); Validate(input.RootElement, schemas.GetProperty("CommandRequest"));
            var id = input.RootElement.GetProperty("commandId").GetString()!; var rawCommand = input.RootElement.GetProperty("command");
            var numeric = rawCommand.EnumerateObject().Where(p => p.Value.ValueKind == JsonValueKind.Number && !p.Value.TryGetInt32(out _)).ToArray();
            Command command;
            if (numeric.Length == 0) command = rawCommand.Deserialize(Wire.Default.Command)!;
            else { var node = JsonNode.Parse(rawCommand.GetRawText())!.AsObject(); foreach (var p in numeric) node[p.Name] = (int)p.Value.GetDouble(); command = JsonSerializer.Deserialize(node.ToJsonString(), Wire.Default.Command)!; }
            var prepared = executor.Prepare(id, command, handler);
            if (route == "/commands" || !prepared.WasRead) { var r = prepared.Commit(); body = Result(r); code = r.Status == "Committed" ? 200 : r.Status == "Rejected" ? 422 : 409; }
            else { lock (ticketGate) { Expire(); if (tickets.Count >= 1024) { code = 429; throw new ApiError("TooManyTickets"); } var key = Guid.NewGuid().ToString("N"); tickets.Add(key, (Stopwatch.GetTimestamp(), prepared)); body = new JsonObject { ["ticket"] = key, ["position"] = prepared.After }; code = 201; } }
        }
        else
        {
            PreparedCommand<Rejection> prepared; lock (ticketGate) { Expire(); if (!tickets.Remove(route.Split('/')[2], out var t)) { code = 404; throw new ApiError("NotFound"); } prepared = t.Command; }
            var r = prepared.Commit(); body = Result(r); code = r.Status == "Committed" ? 200 : r.Status == "Rejected" ? 422 : 409;
        }
    }
    catch (ApiError e) { body = new JsonObject { ["error"] = e.Message }; }
    catch (BadHttpRequestException e) { code = e.StatusCode; body = new JsonObject { ["error"] = "PayloadTooLarge" }; }
    catch (Exception e) when (e is JsonException or KeyNotFoundException) { code = 400; body = new JsonObject { ["error"] = "InvalidRequest" }; }
    catch (Exception e) { Console.Error.WriteLine(e); code = 500; body = new JsonObject { ["error"] = "InternalError" }; }
    context.Response.StatusCode = code; context.Response.ContentType = "application/json"; await context.Response.WriteAsync(body.ToJsonString());
});
await app.RunAsync();
sealed class ApiError(string message) : Exception(message);
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, AllowOutOfOrderMetadataProperties = true, RespectNullableAnnotations = true, RespectRequiredConstructorParameters = true)]
[JsonSerializable(typeof(Command))]
[JsonSerializable(typeof(Event))]
[JsonSerializable(typeof(SchoolList))]
[JsonSerializable(typeof(Student))]
internal partial class Wire : JsonSerializerContext;
