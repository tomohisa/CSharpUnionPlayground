using System.Collections.Immutable;
using Microsoft.Data.Sqlite;
namespace Playground.Dcb;

public sealed class SqliteJournal:IJsonJournal
{
    private readonly object gate=new();
    private readonly SqliteConnection connection;
    private readonly Dictionary<string,SqliteCommand> statements=new();
    private SqliteTransaction? transaction;
    public string EngineVersion {get;}
    public SqliteJournal(string path,string synchronous="FULL")
    {
        if(synchronous is not ("FULL" or "NORMAL"))throw new ArgumentException("durability must be FULL or NORMAL");
        var directory=Path.GetDirectoryName(Path.GetFullPath(path));if(path!=":memory:" && directory is not null)Directory.CreateDirectory(directory);
        connection=new(new SqliteConnectionStringBuilder{DataSource=path,Pooling=false,DefaultTimeout=5}.ToString());connection.Open();
        Exec("PRAGMA journal_mode=WAL");Exec("PRAGMA synchronous="+synchronous);Exec("PRAGMA foreign_keys=ON");Exec("PRAGMA busy_timeout=5000");Exec("PRAGMA cache_size=-2048");Exec("PRAGMA wal_autocheckpoint=1000");
        Exec("CREATE TABLE IF NOT EXISTS events(position INTEGER PRIMARY KEY,command_id TEXT NOT NULL,event_type TEXT NOT NULL,payload_json TEXT NOT NULL,tags_json TEXT NOT NULL)");
        Exec("CREATE TABLE IF NOT EXISTS event_tags(position INTEGER NOT NULL REFERENCES events(position),category TEXT NOT NULL,keys_key BLOB NOT NULL,PRIMARY KEY(position,category,keys_key))");
        Exec("CREATE INDEX IF NOT EXISTS tags_match ON event_tags(category,keys_key,position)");Exec("CREATE INDEX IF NOT EXISTS events_type_position ON events(event_type,position)");
        var version=Convert.ToInt32(Scalar("PRAGMA user_version"));if(version is not (0 or 1))throw new InvalidDataException("unsupported schema version");Exec("PRAGMA user_version=1");EngineVersion=Convert.ToString(Scalar("SELECT sqlite_version()"))!;
    }
    private SqliteCommand Statement(string sql,params object[] args)
    {
        if(!statements.TryGetValue(sql,out var command))
        {
            command=connection.CreateCommand();command.CommandText=sql;statements.Add(sql,command);
        }
        command.Transaction=transaction;command.Parameters.Clear();for(int i=0;i<args.Length;i++)command.Parameters.AddWithValue("$p"+i,args[i]);return command;
    }
    private void Exec(string sql,params object[] args)=>Statement(sql,args).ExecuteNonQuery();
    private object Scalar(string sql,params object[] args)=>Statement(sql,args).ExecuteScalar()!;
    private int High()=>Convert.ToInt32(Scalar("SELECT COALESCE(MAX(position),0) FROM events"));
    private static (string Sql,object[] Args) Selection(Query query,int low,int high,bool exists)
    {
        var args=new List<object>();string Param(object value){var name="$p"+args.Count;args.Add(value);return name;}
        string Range()=>"position>"+Param(low)+" AND position<="+Param(high);
        string condition;
        if(query.Items.IsEmpty || query.Items.Any(i=>i.Tags.IsEmpty && i.Types.IsEmpty))condition=Range();
        else
        {
            var alternatives=new List<string>();
            foreach(var item in query.Items)
            {
                var terms=new List<string>();
                foreach(var tag in item.Tags)terms.Add("SELECT position FROM event_tags WHERE category="+Param(tag.Category)+" AND keys_key="+Param(JournalMetadata.Keys(tag.Keys))+" AND "+Range());
                if(!item.Types.IsEmpty)terms.Add("SELECT position FROM events WHERE event_type IN ("+string.Join(",",item.Types.Select(t=>Param(t)))+") AND "+Range());
                alternatives.Add("SELECT position FROM ("+string.Join(" INTERSECT ",terms)+")");
            }
            condition="position IN ("+string.Join(" UNION ",alternatives)+")";
        }
        string columns=exists?"1":"position,command_id,event_type,payload_json,tags_json";
        return("SELECT "+columns+" FROM events WHERE "+condition+(exists?" LIMIT 1":" ORDER BY position"),args.ToArray());
    }
    public JsonRead Read(Query query,int after=0)
    {
        lock(gate)
        {
            using var tx=connection.BeginTransaction(deferred:true);transaction=tx;
            try
            {
                int high=High();if(after<0 || after>high)throw new ArgumentException("invalid position");
                var select=Selection(query,after,high,false);var rows=ImmutableArray.CreateBuilder<JsonEvent>();
                using(var reader=Statement(select.Sql,select.Args).ExecuteReader())while(reader.Read())rows.Add(new(reader.GetInt32(0),reader.GetString(1),reader.GetString(2),reader.GetString(3),reader.GetString(4)));
                tx.Commit();return new(high,rows.ToImmutable());
            }
            finally{transaction=null;}
        }
    }
    private void Insert(JsonEvent row)
    {
        Exec("INSERT INTO events(position,command_id,event_type,payload_json,tags_json) VALUES($p0,$p1,$p2,$p3,$p4)",row.Position,row.CommandId,row.EventType,row.PayloadJson,row.TagsJson);
        foreach(var tag in JournalMetadata.Decode(row.TagsJson))Exec("INSERT INTO event_tags(position,category,keys_key) VALUES($p0,$p1,$p2)",row.Position,tag.Category,JournalMetadata.Keys(tag.Keys));
    }
    public JsonAppend Append(string id,Query query,int after,ImmutableArray<JsonEvent> events)
    {
        lock(gate)
        {
            using var tx=connection.BeginTransaction(deferred:false);transaction=tx;
            try
            {
                int high=High();if(events.IsEmpty || id.Length==0 || after<0 || after>high)throw new ArgumentException("invalid append");
                var select=Selection(query,after,high,true);if(Statement(select.Sql,select.Args).ExecuteScalar() is not null){tx.Rollback();return new(false,[]);}
                var rows=events.Select((e,i)=>e with{Position=high+i+1,CommandId=id}).ToImmutableArray();foreach(var row in rows)Insert(row);
                tx.Commit();return new(true,rows);
            }
            finally{transaction=null;}
        }
    }
    public void Load(ImmutableArray<JsonEvent> events)
    {
        lock(gate)
        {
            using var tx=connection.BeginTransaction(deferred:false);transaction=tx;
            try{if(High()!=0)throw new InvalidOperationException("load requires empty journal");if(events.Where((e,i)=>e.Position!=i+1).Any())throw new ArgumentException("invalid imported position");foreach(var row in events)Insert(row);tx.Commit();}
            finally{transaction=null;}
        }
    }
    public void Dispose(){lock(gate){foreach(var command in statements.Values)command.Dispose();connection.Dispose();}}
}
