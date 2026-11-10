from pathlib import Path
HERE=Path(__file__).resolve().parent
EVENTS=['Created','Renamed','BalanceAdded','ItemAppended','FlagToggled','CountAdded','TagChanged','Seen','Revised','Archived']
NAMES=[f'D{d:02}{e}' for d in range(10) for e in EVENTS]
def render(name,**values):
 s=(HERE/'templates'/name).read_text()
 for k,v in values.items():s=s.replace('@@'+k+'@@',str(v))
 return s
def generate(out):
 out.mkdir(parents=True,exist_ok=True)
 handlers={lang:[] for lang in ['cs','rs','ts','java','go','swift']}
 for d in range(10):
  ns=NAMES[d*10:(d+1)*10]
  handlers['cs'].append(f'static void Apply{d}(IEventPayload payload, Store store, string mode) {{ switch(payload) {{\n'+ '\n'.join(f'case {n} e: Update(store, e.Data, {k}, mode); break;' for k,n in enumerate(ns))+'\ndefault: store.Ignored++; break; } }')
  handlers['rs'].append(f'fn apply{d}(p:&dyn EventPayload,s:&mut Store,mode:&str) {{\n'+' else '.join(f'if let Some(e)=p.as_any().downcast_ref::<{n}>() {{ update(s,&e.data,{k},mode); }}' for k,n in enumerate(ns))+' else {s.ignored+=1;}\n}')
  handlers['ts'].append(f'function apply{d}(p:IEventPayload,s:Store,mode:string):void {{switch(p.kind){{\n'+'\n'.join(f'case "{n}": update(s,p.data,{k},mode); break;' for k,n in enumerate(ns))+'\ndefault:s.ignored++;}}')
  handlers['java'].append(f'static void apply{d}(EventPayload p,Store s,String mode) {{switch(p){{\n'+'\n'.join(f'case {n} e -> update(s,e.data(),{k},mode);' for k,n in enumerate(ns))+'\ndefault -> s.ignored++;}}')
  handlers['go'].append(f'func apply{d}(p EventPayload,s *Store,mode string){{switch e:=p.(type){{\n'+'\n'.join(f'case {n}:update(s,e.Data,{k},mode)' for k,n in enumerate(ns))+'\ndefault:s.Ignored++}}')
  handlers['swift'].append(f'func apply{d}(_ p:any EventPayload,_ s:Store,_ mode:String){{switch p{{\n'+'\n'.join(f'case let e as {n}: update(s,e.data,{k},mode)' for k,n in enumerate(ns))+'\ndefault:s.ignored+=1}}')
 (out/'Program.cs').write_text(render('projection.cs',TYPES='\n'.join(f'public record {n}(EventData Data):IEventPayload;' for n in NAMES),CREATE='\n'.join(f'{i}=>new {n}(data),' for i,n in enumerate(NAMES)),HANDLERS='\n'.join(handlers['cs']),ROUTE='\n'.join(f'case {d}:Apply{d}(p,stores[{d}],mode);break;' for d in range(10))))
 (out/'Projection.csproj').write_text('''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net11.0</TargetFramework><LangVersion>15.0</LangVersion><Nullable>enable</Nullable><ImplicitUsings>enable</ImplicitUsings><PublishAot>true</PublishAot><JsonSerializerIsReflectionEnabledByDefault>false</JsonSerializerIsReflectionEnabledByDefault><WarningsAsErrors>IL2026;IL3050</WarningsAsErrors></PropertyGroup></Project>''')
 (out/'projection.rs').write_text(render('projection.rs',TYPES='\n'.join(f'struct {n} {{data:EventData}}\nimpl EventPayload for {n}{{fn as_any(&self)->&dyn Any{{self}}}}' for n in NAMES),CREATE='\n'.join(f'{i}=>Box::new({n}{{data}}),' for i,n in enumerate(NAMES)),HANDLERS='\n'.join(handlers['rs']),ROUTE='\n'.join(f'{d}=>apply{d}(p,&mut stores[{d}],mode),' for d in range(10))))
 (out/'projection.ts').write_text(render('projection.ts',TYPES='\n'.join(f'class {n} implements IEventPayload {{kind="{n}";data:EventData;constructor(data:EventData){{this.data=data;}}}}' for n in NAMES),CREATE='\n'.join(f'case {i}:return new {n}(data);' for i,n in enumerate(NAMES)),HANDLERS='\n'.join(handlers['ts']),ROUTE='\n'.join(f'case {d}:apply{d}(p,stores[{d}],mode);break;' for d in range(10))))
 (out/'Projection.java').write_text(render('Projection.java',TYPES='\n'.join(f'record {n}(EventData data) implements EventPayload {{}}' for n in NAMES),CREATE='\n'.join(f'case {i}->new {n}(data);' for i,n in enumerate(NAMES)),HANDLERS='\n'.join(handlers['java']),ROUTE='\n'.join(f'case {d}->apply{d}(p,stores[{d}],mode);' for d in range(10))))
 (out/'projection.go').write_text(render('projection.go',TYPES='\n'.join(f'type {n} struct{{Data EventData}}\nfunc({n})isPayload(){{}}' for n in NAMES),CREATE='\n'.join(f'case {i}:return {n}{{data}}' for i,n in enumerate(NAMES)),HANDLERS='\n'.join(handlers['go']),ROUTE='\n'.join(f'case {d}:apply{d}(p,stores[{d}],mode)' for d in range(10))))
 (out/'Projection.swift').write_text(render('Projection.swift',TYPES='\n'.join(f'struct {n}:EventPayload{{let data:EventData}}' for n in NAMES),CREATE='\n'.join(f'case {i}:return {n}(data:data)' for i,n in enumerate(NAMES)),HANDLERS='\n'.join(handlers['swift']),ROUTE='\n'.join(f'case {d}:apply{d}(p,stores[{d}],mode)' for d in range(10))))

 ts=(out/'projection.ts').read_text()
 schema='import { z } from "zod";\nconst EventDataSchema = z.object({id:z.number().int(),value:z.number().int(),label:z.string()});\n'
 (out/'projection-zod.ts').write_text(schema+ts.replace('update(s,p.data,','update(s,EventDataSchema.parse(p.data),'))
