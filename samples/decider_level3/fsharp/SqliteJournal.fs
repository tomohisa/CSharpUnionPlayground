namespace Playground.Dcb
open System
open System.Collections.Generic
open System.IO
open Microsoft.Data.Sqlite

type SqliteJournal(path:string,synchronous:string) =
    let gate=obj()
    let boxed value=box value |> nonNull
    let connection=new SqliteConnection(SqliteConnectionStringBuilder(DataSource=path,Pooling=false,DefaultTimeout=5).ToString())
    let statements=Dictionary<string,SqliteCommand>(StringComparer.Ordinal)
    let mutable transaction:SqliteTransaction | null=null
    let statement sql (args:obj array) =
        let command=match statements.TryGetValue sql with true,c->c | _->let c=connection.CreateCommand() in c.CommandText<-sql;statements.Add(sql,c);c
        command.Transaction<-transaction
        command.Parameters.Clear()
        args |> Array.iteri(fun i value->command.Parameters.AddWithValue("$p"+string i,value) |> ignore)
        command
    let execute sql args=(statement sql args).ExecuteNonQuery() |> ignore
    let scalar sql args=(statement sql args).ExecuteScalar()
    let high()=Convert.ToInt32(scalar "SELECT COALESCE(MAX(position),0) FROM events" [||])
    let selection (query:Query) low high exists =
        let args=ResizeArray<obj>()
        let param value=let name="$p"+string args.Count in args.Add value;name
        let range()="position>"+param(boxed low)+" AND position<="+param(boxed high)
        let condition=
            if query.Items.IsEmpty || query.Items |> List.exists(fun i->i.Tags.IsEmpty && i.Types.IsEmpty) then range()
            else
                let alternatives=[for item in query.Items do
                                      let terms=ResizeArray<string>()
                                      for tag in item.Tags do terms.Add("SELECT position FROM event_tags WHERE category="+param(boxed tag.Category)+" AND keys_key="+param(boxed(JournalMetadata.keys tag.Keys))+" AND "+range())
                                      if not item.Types.IsEmpty then terms.Add("SELECT position FROM events WHERE event_type IN ("+String.Join(",",item.Types |> List.map(fun t->param(boxed t)))+") AND "+range())
                                      yield "SELECT position FROM ("+String.Join(" INTERSECT ",terms)+")"]
                "position IN ("+String.Join(" UNION ",alternatives)+")"
        let columns=if exists then "1" else "position,command_id,event_type,payload_json,tags_json"
        "SELECT "+columns+" FROM events WHERE "+condition+(if exists then " LIMIT 1" else " ORDER BY position"),args.ToArray()
    let insert row=
        execute "INSERT INTO events(position,command_id,event_type,payload_json,tags_json) VALUES($p0,$p1,$p2,$p3,$p4)" [|boxed row.Position;boxed row.CommandId;boxed row.EventType;boxed row.PayloadJson;boxed row.TagsJson|]
        for tag in JournalMetadata.decode row.TagsJson do execute "INSERT INTO event_tags(position,category,keys_key) VALUES($p0,$p1,$p2)" [|boxed row.Position;boxed tag.Category;boxed(JournalMetadata.keys tag.Keys)|]
    do
        if synchronous<>"FULL" && synchronous<>"NORMAL" then invalidArg "synchronous" "durability must be FULL or NORMAL"
        if path<>":memory:" then Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath path) |> nonNull) |> ignore
        connection.Open()
        for sql in ["PRAGMA journal_mode=WAL";"PRAGMA synchronous="+synchronous;"PRAGMA foreign_keys=ON";"PRAGMA busy_timeout=5000";"PRAGMA cache_size=-2048";"PRAGMA wal_autocheckpoint=1000"
                    "CREATE TABLE IF NOT EXISTS events(position INTEGER PRIMARY KEY,command_id TEXT NOT NULL,event_type TEXT NOT NULL,payload_json TEXT NOT NULL,tags_json TEXT NOT NULL)"
                    "CREATE TABLE IF NOT EXISTS event_tags(position INTEGER NOT NULL REFERENCES events(position),category TEXT NOT NULL,keys_key BLOB NOT NULL,PRIMARY KEY(position,category,keys_key))"
                    "CREATE INDEX IF NOT EXISTS tags_match ON event_tags(category,keys_key,position)";"CREATE INDEX IF NOT EXISTS events_type_position ON events(event_type,position)"] do execute sql [||]
        let version=Convert.ToInt32(scalar "PRAGMA user_version" [||])
        if version<>0 && version<>1 then invalidOp "unsupported schema version"
        execute "PRAGMA user_version=1" [||]
    let version=Convert.ToString(scalar "SELECT sqlite_version()" [||]) |> nonNull
    interface IJsonJournal with
        member _.EngineVersion=version
        member _.Read(query,after)=lock gate (fun ()->
            use tx=connection.BeginTransaction(deferred=true)
            transaction<-tx
            try
                let position=high()
                if after<0 || after>position then invalidArg "after" "invalid position"
                let sql,args=selection query after position false
                let rows=ResizeArray<JsonEvent>()
                do
                    use reader=(statement sql args).ExecuteReader()
                    while reader.Read() do rows.Add {Position=reader.GetInt32(0);CommandId=reader.GetString(1);EventType=reader.GetString(2);PayloadJson=reader.GetString(3);TagsJson=reader.GetString(4)}
                tx.Commit()
                {After=position;Events=List.ofSeq rows}
            finally transaction<-null)
        member _.Append(id,query,after,events)=lock gate (fun ()->
            use tx=connection.BeginTransaction(deferred=false)
            transaction<-tx
            try
                let position=high()
                if List.isEmpty events || String.IsNullOrEmpty id || after<0 || after>position then invalidArg "events" "invalid append"
                let sql,args=selection query after position true
                if not(isNull(scalar sql args)) then tx.Rollback();RawConflict
                else
                    let rows=events |> List.mapi(fun i e->{e with Position=position+i+1;CommandId=id})
                    for row in rows do insert row
                    tx.Commit();RawCommitted rows
            finally transaction<-null)
        member _.Load events=lock gate (fun ()->
            use tx=connection.BeginTransaction(deferred=false)
            transaction<-tx
            try
                if high()<>0 then invalidOp "load requires empty journal"
                if events |> List.mapi(fun i e->e.Position<>i+1) |> List.exists id then invalidArg "events" "invalid imported position"
                for row in events do insert row
                tx.Commit()
            finally transaction<-null)
        member _.Dispose()=lock gate (fun ()->
            for command in statements.Values do command.Dispose()
            connection.Dispose())
