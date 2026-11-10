use std::{any::Any,collections::HashMap,time::Instant,io::Write};
struct EventData{id:usize,value:i64,label:String}
trait EventPayload{fn as_any(&self)->&dyn Any;}
struct Unknown;impl EventPayload for Unknown{fn as_any(&self)->&dyn Any{self}}
struct D00Created {data:EventData}
impl EventPayload for D00Created{fn as_any(&self)->&dyn Any{self}}
struct D00Renamed {data:EventData}
impl EventPayload for D00Renamed{fn as_any(&self)->&dyn Any{self}}
struct D00BalanceAdded {data:EventData}
impl EventPayload for D00BalanceAdded{fn as_any(&self)->&dyn Any{self}}
struct D00ItemAppended {data:EventData}
impl EventPayload for D00ItemAppended{fn as_any(&self)->&dyn Any{self}}
struct D00FlagToggled {data:EventData}
impl EventPayload for D00FlagToggled{fn as_any(&self)->&dyn Any{self}}
struct D00CountAdded {data:EventData}
impl EventPayload for D00CountAdded{fn as_any(&self)->&dyn Any{self}}
struct D00TagChanged {data:EventData}
impl EventPayload for D00TagChanged{fn as_any(&self)->&dyn Any{self}}
struct D00Seen {data:EventData}
impl EventPayload for D00Seen{fn as_any(&self)->&dyn Any{self}}
struct D00Revised {data:EventData}
impl EventPayload for D00Revised{fn as_any(&self)->&dyn Any{self}}
struct D00Archived {data:EventData}
impl EventPayload for D00Archived{fn as_any(&self)->&dyn Any{self}}
struct D01Created {data:EventData}
impl EventPayload for D01Created{fn as_any(&self)->&dyn Any{self}}
struct D01Renamed {data:EventData}
impl EventPayload for D01Renamed{fn as_any(&self)->&dyn Any{self}}
struct D01BalanceAdded {data:EventData}
impl EventPayload for D01BalanceAdded{fn as_any(&self)->&dyn Any{self}}
struct D01ItemAppended {data:EventData}
impl EventPayload for D01ItemAppended{fn as_any(&self)->&dyn Any{self}}
struct D01FlagToggled {data:EventData}
impl EventPayload for D01FlagToggled{fn as_any(&self)->&dyn Any{self}}
struct D01CountAdded {data:EventData}
impl EventPayload for D01CountAdded{fn as_any(&self)->&dyn Any{self}}
struct D01TagChanged {data:EventData}
impl EventPayload for D01TagChanged{fn as_any(&self)->&dyn Any{self}}
struct D01Seen {data:EventData}
impl EventPayload for D01Seen{fn as_any(&self)->&dyn Any{self}}
struct D01Revised {data:EventData}
impl EventPayload for D01Revised{fn as_any(&self)->&dyn Any{self}}
struct D01Archived {data:EventData}
impl EventPayload for D01Archived{fn as_any(&self)->&dyn Any{self}}
struct D02Created {data:EventData}
impl EventPayload for D02Created{fn as_any(&self)->&dyn Any{self}}
struct D02Renamed {data:EventData}
impl EventPayload for D02Renamed{fn as_any(&self)->&dyn Any{self}}
struct D02BalanceAdded {data:EventData}
impl EventPayload for D02BalanceAdded{fn as_any(&self)->&dyn Any{self}}
struct D02ItemAppended {data:EventData}
impl EventPayload for D02ItemAppended{fn as_any(&self)->&dyn Any{self}}
struct D02FlagToggled {data:EventData}
impl EventPayload for D02FlagToggled{fn as_any(&self)->&dyn Any{self}}
struct D02CountAdded {data:EventData}
impl EventPayload for D02CountAdded{fn as_any(&self)->&dyn Any{self}}
struct D02TagChanged {data:EventData}
impl EventPayload for D02TagChanged{fn as_any(&self)->&dyn Any{self}}
struct D02Seen {data:EventData}
impl EventPayload for D02Seen{fn as_any(&self)->&dyn Any{self}}
struct D02Revised {data:EventData}
impl EventPayload for D02Revised{fn as_any(&self)->&dyn Any{self}}
struct D02Archived {data:EventData}
impl EventPayload for D02Archived{fn as_any(&self)->&dyn Any{self}}
struct D03Created {data:EventData}
impl EventPayload for D03Created{fn as_any(&self)->&dyn Any{self}}
struct D03Renamed {data:EventData}
impl EventPayload for D03Renamed{fn as_any(&self)->&dyn Any{self}}
struct D03BalanceAdded {data:EventData}
impl EventPayload for D03BalanceAdded{fn as_any(&self)->&dyn Any{self}}
struct D03ItemAppended {data:EventData}
impl EventPayload for D03ItemAppended{fn as_any(&self)->&dyn Any{self}}
struct D03FlagToggled {data:EventData}
impl EventPayload for D03FlagToggled{fn as_any(&self)->&dyn Any{self}}
struct D03CountAdded {data:EventData}
impl EventPayload for D03CountAdded{fn as_any(&self)->&dyn Any{self}}
struct D03TagChanged {data:EventData}
impl EventPayload for D03TagChanged{fn as_any(&self)->&dyn Any{self}}
struct D03Seen {data:EventData}
impl EventPayload for D03Seen{fn as_any(&self)->&dyn Any{self}}
struct D03Revised {data:EventData}
impl EventPayload for D03Revised{fn as_any(&self)->&dyn Any{self}}
struct D03Archived {data:EventData}
impl EventPayload for D03Archived{fn as_any(&self)->&dyn Any{self}}
struct D04Created {data:EventData}
impl EventPayload for D04Created{fn as_any(&self)->&dyn Any{self}}
struct D04Renamed {data:EventData}
impl EventPayload for D04Renamed{fn as_any(&self)->&dyn Any{self}}
struct D04BalanceAdded {data:EventData}
impl EventPayload for D04BalanceAdded{fn as_any(&self)->&dyn Any{self}}
struct D04ItemAppended {data:EventData}
impl EventPayload for D04ItemAppended{fn as_any(&self)->&dyn Any{self}}
struct D04FlagToggled {data:EventData}
impl EventPayload for D04FlagToggled{fn as_any(&self)->&dyn Any{self}}
struct D04CountAdded {data:EventData}
impl EventPayload for D04CountAdded{fn as_any(&self)->&dyn Any{self}}
struct D04TagChanged {data:EventData}
impl EventPayload for D04TagChanged{fn as_any(&self)->&dyn Any{self}}
struct D04Seen {data:EventData}
impl EventPayload for D04Seen{fn as_any(&self)->&dyn Any{self}}
struct D04Revised {data:EventData}
impl EventPayload for D04Revised{fn as_any(&self)->&dyn Any{self}}
struct D04Archived {data:EventData}
impl EventPayload for D04Archived{fn as_any(&self)->&dyn Any{self}}
struct D05Created {data:EventData}
impl EventPayload for D05Created{fn as_any(&self)->&dyn Any{self}}
struct D05Renamed {data:EventData}
impl EventPayload for D05Renamed{fn as_any(&self)->&dyn Any{self}}
struct D05BalanceAdded {data:EventData}
impl EventPayload for D05BalanceAdded{fn as_any(&self)->&dyn Any{self}}
struct D05ItemAppended {data:EventData}
impl EventPayload for D05ItemAppended{fn as_any(&self)->&dyn Any{self}}
struct D05FlagToggled {data:EventData}
impl EventPayload for D05FlagToggled{fn as_any(&self)->&dyn Any{self}}
struct D05CountAdded {data:EventData}
impl EventPayload for D05CountAdded{fn as_any(&self)->&dyn Any{self}}
struct D05TagChanged {data:EventData}
impl EventPayload for D05TagChanged{fn as_any(&self)->&dyn Any{self}}
struct D05Seen {data:EventData}
impl EventPayload for D05Seen{fn as_any(&self)->&dyn Any{self}}
struct D05Revised {data:EventData}
impl EventPayload for D05Revised{fn as_any(&self)->&dyn Any{self}}
struct D05Archived {data:EventData}
impl EventPayload for D05Archived{fn as_any(&self)->&dyn Any{self}}
struct D06Created {data:EventData}
impl EventPayload for D06Created{fn as_any(&self)->&dyn Any{self}}
struct D06Renamed {data:EventData}
impl EventPayload for D06Renamed{fn as_any(&self)->&dyn Any{self}}
struct D06BalanceAdded {data:EventData}
impl EventPayload for D06BalanceAdded{fn as_any(&self)->&dyn Any{self}}
struct D06ItemAppended {data:EventData}
impl EventPayload for D06ItemAppended{fn as_any(&self)->&dyn Any{self}}
struct D06FlagToggled {data:EventData}
impl EventPayload for D06FlagToggled{fn as_any(&self)->&dyn Any{self}}
struct D06CountAdded {data:EventData}
impl EventPayload for D06CountAdded{fn as_any(&self)->&dyn Any{self}}
struct D06TagChanged {data:EventData}
impl EventPayload for D06TagChanged{fn as_any(&self)->&dyn Any{self}}
struct D06Seen {data:EventData}
impl EventPayload for D06Seen{fn as_any(&self)->&dyn Any{self}}
struct D06Revised {data:EventData}
impl EventPayload for D06Revised{fn as_any(&self)->&dyn Any{self}}
struct D06Archived {data:EventData}
impl EventPayload for D06Archived{fn as_any(&self)->&dyn Any{self}}
struct D07Created {data:EventData}
impl EventPayload for D07Created{fn as_any(&self)->&dyn Any{self}}
struct D07Renamed {data:EventData}
impl EventPayload for D07Renamed{fn as_any(&self)->&dyn Any{self}}
struct D07BalanceAdded {data:EventData}
impl EventPayload for D07BalanceAdded{fn as_any(&self)->&dyn Any{self}}
struct D07ItemAppended {data:EventData}
impl EventPayload for D07ItemAppended{fn as_any(&self)->&dyn Any{self}}
struct D07FlagToggled {data:EventData}
impl EventPayload for D07FlagToggled{fn as_any(&self)->&dyn Any{self}}
struct D07CountAdded {data:EventData}
impl EventPayload for D07CountAdded{fn as_any(&self)->&dyn Any{self}}
struct D07TagChanged {data:EventData}
impl EventPayload for D07TagChanged{fn as_any(&self)->&dyn Any{self}}
struct D07Seen {data:EventData}
impl EventPayload for D07Seen{fn as_any(&self)->&dyn Any{self}}
struct D07Revised {data:EventData}
impl EventPayload for D07Revised{fn as_any(&self)->&dyn Any{self}}
struct D07Archived {data:EventData}
impl EventPayload for D07Archived{fn as_any(&self)->&dyn Any{self}}
struct D08Created {data:EventData}
impl EventPayload for D08Created{fn as_any(&self)->&dyn Any{self}}
struct D08Renamed {data:EventData}
impl EventPayload for D08Renamed{fn as_any(&self)->&dyn Any{self}}
struct D08BalanceAdded {data:EventData}
impl EventPayload for D08BalanceAdded{fn as_any(&self)->&dyn Any{self}}
struct D08ItemAppended {data:EventData}
impl EventPayload for D08ItemAppended{fn as_any(&self)->&dyn Any{self}}
struct D08FlagToggled {data:EventData}
impl EventPayload for D08FlagToggled{fn as_any(&self)->&dyn Any{self}}
struct D08CountAdded {data:EventData}
impl EventPayload for D08CountAdded{fn as_any(&self)->&dyn Any{self}}
struct D08TagChanged {data:EventData}
impl EventPayload for D08TagChanged{fn as_any(&self)->&dyn Any{self}}
struct D08Seen {data:EventData}
impl EventPayload for D08Seen{fn as_any(&self)->&dyn Any{self}}
struct D08Revised {data:EventData}
impl EventPayload for D08Revised{fn as_any(&self)->&dyn Any{self}}
struct D08Archived {data:EventData}
impl EventPayload for D08Archived{fn as_any(&self)->&dyn Any{self}}
struct D09Created {data:EventData}
impl EventPayload for D09Created{fn as_any(&self)->&dyn Any{self}}
struct D09Renamed {data:EventData}
impl EventPayload for D09Renamed{fn as_any(&self)->&dyn Any{self}}
struct D09BalanceAdded {data:EventData}
impl EventPayload for D09BalanceAdded{fn as_any(&self)->&dyn Any{self}}
struct D09ItemAppended {data:EventData}
impl EventPayload for D09ItemAppended{fn as_any(&self)->&dyn Any{self}}
struct D09FlagToggled {data:EventData}
impl EventPayload for D09FlagToggled{fn as_any(&self)->&dyn Any{self}}
struct D09CountAdded {data:EventData}
impl EventPayload for D09CountAdded{fn as_any(&self)->&dyn Any{self}}
struct D09TagChanged {data:EventData}
impl EventPayload for D09TagChanged{fn as_any(&self)->&dyn Any{self}}
struct D09Seen {data:EventData}
impl EventPayload for D09Seen{fn as_any(&self)->&dyn Any{self}}
struct D09Revised {data:EventData}
impl EventPayload for D09Revised{fn as_any(&self)->&dyn Any{self}}
struct D09Archived {data:EventData}
impl EventPayload for D09Archived{fn as_any(&self)->&dyn Any{self}}
#[derive(Default)]struct State{id:usize,balance:i64,counter:i64,seen:i64,revision:i64,name:String,exists:bool,flag:bool,archived:bool,items:Vec<i64>,tags:HashMap<i64,i64>}
#[derive(Default)]struct Store{map:HashMap<usize,State>,list:Vec<State>,ignored:i64}
fn create(i:usize,data:EventData)->Box<dyn EventPayload>{match i{
0=>Box::new(D00Created{data}),
1=>Box::new(D00Renamed{data}),
2=>Box::new(D00BalanceAdded{data}),
3=>Box::new(D00ItemAppended{data}),
4=>Box::new(D00FlagToggled{data}),
5=>Box::new(D00CountAdded{data}),
6=>Box::new(D00TagChanged{data}),
7=>Box::new(D00Seen{data}),
8=>Box::new(D00Revised{data}),
9=>Box::new(D00Archived{data}),
10=>Box::new(D01Created{data}),
11=>Box::new(D01Renamed{data}),
12=>Box::new(D01BalanceAdded{data}),
13=>Box::new(D01ItemAppended{data}),
14=>Box::new(D01FlagToggled{data}),
15=>Box::new(D01CountAdded{data}),
16=>Box::new(D01TagChanged{data}),
17=>Box::new(D01Seen{data}),
18=>Box::new(D01Revised{data}),
19=>Box::new(D01Archived{data}),
20=>Box::new(D02Created{data}),
21=>Box::new(D02Renamed{data}),
22=>Box::new(D02BalanceAdded{data}),
23=>Box::new(D02ItemAppended{data}),
24=>Box::new(D02FlagToggled{data}),
25=>Box::new(D02CountAdded{data}),
26=>Box::new(D02TagChanged{data}),
27=>Box::new(D02Seen{data}),
28=>Box::new(D02Revised{data}),
29=>Box::new(D02Archived{data}),
30=>Box::new(D03Created{data}),
31=>Box::new(D03Renamed{data}),
32=>Box::new(D03BalanceAdded{data}),
33=>Box::new(D03ItemAppended{data}),
34=>Box::new(D03FlagToggled{data}),
35=>Box::new(D03CountAdded{data}),
36=>Box::new(D03TagChanged{data}),
37=>Box::new(D03Seen{data}),
38=>Box::new(D03Revised{data}),
39=>Box::new(D03Archived{data}),
40=>Box::new(D04Created{data}),
41=>Box::new(D04Renamed{data}),
42=>Box::new(D04BalanceAdded{data}),
43=>Box::new(D04ItemAppended{data}),
44=>Box::new(D04FlagToggled{data}),
45=>Box::new(D04CountAdded{data}),
46=>Box::new(D04TagChanged{data}),
47=>Box::new(D04Seen{data}),
48=>Box::new(D04Revised{data}),
49=>Box::new(D04Archived{data}),
50=>Box::new(D05Created{data}),
51=>Box::new(D05Renamed{data}),
52=>Box::new(D05BalanceAdded{data}),
53=>Box::new(D05ItemAppended{data}),
54=>Box::new(D05FlagToggled{data}),
55=>Box::new(D05CountAdded{data}),
56=>Box::new(D05TagChanged{data}),
57=>Box::new(D05Seen{data}),
58=>Box::new(D05Revised{data}),
59=>Box::new(D05Archived{data}),
60=>Box::new(D06Created{data}),
61=>Box::new(D06Renamed{data}),
62=>Box::new(D06BalanceAdded{data}),
63=>Box::new(D06ItemAppended{data}),
64=>Box::new(D06FlagToggled{data}),
65=>Box::new(D06CountAdded{data}),
66=>Box::new(D06TagChanged{data}),
67=>Box::new(D06Seen{data}),
68=>Box::new(D06Revised{data}),
69=>Box::new(D06Archived{data}),
70=>Box::new(D07Created{data}),
71=>Box::new(D07Renamed{data}),
72=>Box::new(D07BalanceAdded{data}),
73=>Box::new(D07ItemAppended{data}),
74=>Box::new(D07FlagToggled{data}),
75=>Box::new(D07CountAdded{data}),
76=>Box::new(D07TagChanged{data}),
77=>Box::new(D07Seen{data}),
78=>Box::new(D07Revised{data}),
79=>Box::new(D07Archived{data}),
80=>Box::new(D08Created{data}),
81=>Box::new(D08Renamed{data}),
82=>Box::new(D08BalanceAdded{data}),
83=>Box::new(D08ItemAppended{data}),
84=>Box::new(D08FlagToggled{data}),
85=>Box::new(D08CountAdded{data}),
86=>Box::new(D08TagChanged{data}),
87=>Box::new(D08Seen{data}),
88=>Box::new(D08Revised{data}),
89=>Box::new(D08Archived{data}),
90=>Box::new(D09Created{data}),
91=>Box::new(D09Renamed{data}),
92=>Box::new(D09BalanceAdded{data}),
93=>Box::new(D09ItemAppended{data}),
94=>Box::new(D09FlagToggled{data}),
95=>Box::new(D09CountAdded{data}),
96=>Box::new(D09TagChanged{data}),
97=>Box::new(D09Seen{data}),
98=>Box::new(D09Revised{data}),
99=>Box::new(D09Archived{data}),
_=>unreachable!()}}
fn route(domain:usize,p:&dyn EventPayload,stores:&mut[Store],mode:&str){match domain{
0=>apply0(p,&mut stores[0],mode),
1=>apply1(p,&mut stores[1],mode),
2=>apply2(p,&mut stores[2],mode),
3=>apply3(p,&mut stores[3],mode),
4=>apply4(p,&mut stores[4],mode),
5=>apply5(p,&mut stores[5],mode),
6=>apply6(p,&mut stores[6],mode),
7=>apply7(p,&mut stores[7],mode),
8=>apply8(p,&mut stores[8],mode),
9=>apply9(p,&mut stores[9],mode),
_=>unreachable!()}}
fn apply0(p:&dyn EventPayload,s:&mut Store,mode:&str) {
if let Some(e)=p.as_any().downcast_ref::<D00Created>() { update(s,&e.data,0,mode); } else if let Some(e)=p.as_any().downcast_ref::<D00Renamed>() { update(s,&e.data,1,mode); } else if let Some(e)=p.as_any().downcast_ref::<D00BalanceAdded>() { update(s,&e.data,2,mode); } else if let Some(e)=p.as_any().downcast_ref::<D00ItemAppended>() { update(s,&e.data,3,mode); } else if let Some(e)=p.as_any().downcast_ref::<D00FlagToggled>() { update(s,&e.data,4,mode); } else if let Some(e)=p.as_any().downcast_ref::<D00CountAdded>() { update(s,&e.data,5,mode); } else if let Some(e)=p.as_any().downcast_ref::<D00TagChanged>() { update(s,&e.data,6,mode); } else if let Some(e)=p.as_any().downcast_ref::<D00Seen>() { update(s,&e.data,7,mode); } else if let Some(e)=p.as_any().downcast_ref::<D00Revised>() { update(s,&e.data,8,mode); } else if let Some(e)=p.as_any().downcast_ref::<D00Archived>() { update(s,&e.data,9,mode); } else {s.ignored+=1;}
}
fn apply1(p:&dyn EventPayload,s:&mut Store,mode:&str) {
if let Some(e)=p.as_any().downcast_ref::<D01Created>() { update(s,&e.data,0,mode); } else if let Some(e)=p.as_any().downcast_ref::<D01Renamed>() { update(s,&e.data,1,mode); } else if let Some(e)=p.as_any().downcast_ref::<D01BalanceAdded>() { update(s,&e.data,2,mode); } else if let Some(e)=p.as_any().downcast_ref::<D01ItemAppended>() { update(s,&e.data,3,mode); } else if let Some(e)=p.as_any().downcast_ref::<D01FlagToggled>() { update(s,&e.data,4,mode); } else if let Some(e)=p.as_any().downcast_ref::<D01CountAdded>() { update(s,&e.data,5,mode); } else if let Some(e)=p.as_any().downcast_ref::<D01TagChanged>() { update(s,&e.data,6,mode); } else if let Some(e)=p.as_any().downcast_ref::<D01Seen>() { update(s,&e.data,7,mode); } else if let Some(e)=p.as_any().downcast_ref::<D01Revised>() { update(s,&e.data,8,mode); } else if let Some(e)=p.as_any().downcast_ref::<D01Archived>() { update(s,&e.data,9,mode); } else {s.ignored+=1;}
}
fn apply2(p:&dyn EventPayload,s:&mut Store,mode:&str) {
if let Some(e)=p.as_any().downcast_ref::<D02Created>() { update(s,&e.data,0,mode); } else if let Some(e)=p.as_any().downcast_ref::<D02Renamed>() { update(s,&e.data,1,mode); } else if let Some(e)=p.as_any().downcast_ref::<D02BalanceAdded>() { update(s,&e.data,2,mode); } else if let Some(e)=p.as_any().downcast_ref::<D02ItemAppended>() { update(s,&e.data,3,mode); } else if let Some(e)=p.as_any().downcast_ref::<D02FlagToggled>() { update(s,&e.data,4,mode); } else if let Some(e)=p.as_any().downcast_ref::<D02CountAdded>() { update(s,&e.data,5,mode); } else if let Some(e)=p.as_any().downcast_ref::<D02TagChanged>() { update(s,&e.data,6,mode); } else if let Some(e)=p.as_any().downcast_ref::<D02Seen>() { update(s,&e.data,7,mode); } else if let Some(e)=p.as_any().downcast_ref::<D02Revised>() { update(s,&e.data,8,mode); } else if let Some(e)=p.as_any().downcast_ref::<D02Archived>() { update(s,&e.data,9,mode); } else {s.ignored+=1;}
}
fn apply3(p:&dyn EventPayload,s:&mut Store,mode:&str) {
if let Some(e)=p.as_any().downcast_ref::<D03Created>() { update(s,&e.data,0,mode); } else if let Some(e)=p.as_any().downcast_ref::<D03Renamed>() { update(s,&e.data,1,mode); } else if let Some(e)=p.as_any().downcast_ref::<D03BalanceAdded>() { update(s,&e.data,2,mode); } else if let Some(e)=p.as_any().downcast_ref::<D03ItemAppended>() { update(s,&e.data,3,mode); } else if let Some(e)=p.as_any().downcast_ref::<D03FlagToggled>() { update(s,&e.data,4,mode); } else if let Some(e)=p.as_any().downcast_ref::<D03CountAdded>() { update(s,&e.data,5,mode); } else if let Some(e)=p.as_any().downcast_ref::<D03TagChanged>() { update(s,&e.data,6,mode); } else if let Some(e)=p.as_any().downcast_ref::<D03Seen>() { update(s,&e.data,7,mode); } else if let Some(e)=p.as_any().downcast_ref::<D03Revised>() { update(s,&e.data,8,mode); } else if let Some(e)=p.as_any().downcast_ref::<D03Archived>() { update(s,&e.data,9,mode); } else {s.ignored+=1;}
}
fn apply4(p:&dyn EventPayload,s:&mut Store,mode:&str) {
if let Some(e)=p.as_any().downcast_ref::<D04Created>() { update(s,&e.data,0,mode); } else if let Some(e)=p.as_any().downcast_ref::<D04Renamed>() { update(s,&e.data,1,mode); } else if let Some(e)=p.as_any().downcast_ref::<D04BalanceAdded>() { update(s,&e.data,2,mode); } else if let Some(e)=p.as_any().downcast_ref::<D04ItemAppended>() { update(s,&e.data,3,mode); } else if let Some(e)=p.as_any().downcast_ref::<D04FlagToggled>() { update(s,&e.data,4,mode); } else if let Some(e)=p.as_any().downcast_ref::<D04CountAdded>() { update(s,&e.data,5,mode); } else if let Some(e)=p.as_any().downcast_ref::<D04TagChanged>() { update(s,&e.data,6,mode); } else if let Some(e)=p.as_any().downcast_ref::<D04Seen>() { update(s,&e.data,7,mode); } else if let Some(e)=p.as_any().downcast_ref::<D04Revised>() { update(s,&e.data,8,mode); } else if let Some(e)=p.as_any().downcast_ref::<D04Archived>() { update(s,&e.data,9,mode); } else {s.ignored+=1;}
}
fn apply5(p:&dyn EventPayload,s:&mut Store,mode:&str) {
if let Some(e)=p.as_any().downcast_ref::<D05Created>() { update(s,&e.data,0,mode); } else if let Some(e)=p.as_any().downcast_ref::<D05Renamed>() { update(s,&e.data,1,mode); } else if let Some(e)=p.as_any().downcast_ref::<D05BalanceAdded>() { update(s,&e.data,2,mode); } else if let Some(e)=p.as_any().downcast_ref::<D05ItemAppended>() { update(s,&e.data,3,mode); } else if let Some(e)=p.as_any().downcast_ref::<D05FlagToggled>() { update(s,&e.data,4,mode); } else if let Some(e)=p.as_any().downcast_ref::<D05CountAdded>() { update(s,&e.data,5,mode); } else if let Some(e)=p.as_any().downcast_ref::<D05TagChanged>() { update(s,&e.data,6,mode); } else if let Some(e)=p.as_any().downcast_ref::<D05Seen>() { update(s,&e.data,7,mode); } else if let Some(e)=p.as_any().downcast_ref::<D05Revised>() { update(s,&e.data,8,mode); } else if let Some(e)=p.as_any().downcast_ref::<D05Archived>() { update(s,&e.data,9,mode); } else {s.ignored+=1;}
}
fn apply6(p:&dyn EventPayload,s:&mut Store,mode:&str) {
if let Some(e)=p.as_any().downcast_ref::<D06Created>() { update(s,&e.data,0,mode); } else if let Some(e)=p.as_any().downcast_ref::<D06Renamed>() { update(s,&e.data,1,mode); } else if let Some(e)=p.as_any().downcast_ref::<D06BalanceAdded>() { update(s,&e.data,2,mode); } else if let Some(e)=p.as_any().downcast_ref::<D06ItemAppended>() { update(s,&e.data,3,mode); } else if let Some(e)=p.as_any().downcast_ref::<D06FlagToggled>() { update(s,&e.data,4,mode); } else if let Some(e)=p.as_any().downcast_ref::<D06CountAdded>() { update(s,&e.data,5,mode); } else if let Some(e)=p.as_any().downcast_ref::<D06TagChanged>() { update(s,&e.data,6,mode); } else if let Some(e)=p.as_any().downcast_ref::<D06Seen>() { update(s,&e.data,7,mode); } else if let Some(e)=p.as_any().downcast_ref::<D06Revised>() { update(s,&e.data,8,mode); } else if let Some(e)=p.as_any().downcast_ref::<D06Archived>() { update(s,&e.data,9,mode); } else {s.ignored+=1;}
}
fn apply7(p:&dyn EventPayload,s:&mut Store,mode:&str) {
if let Some(e)=p.as_any().downcast_ref::<D07Created>() { update(s,&e.data,0,mode); } else if let Some(e)=p.as_any().downcast_ref::<D07Renamed>() { update(s,&e.data,1,mode); } else if let Some(e)=p.as_any().downcast_ref::<D07BalanceAdded>() { update(s,&e.data,2,mode); } else if let Some(e)=p.as_any().downcast_ref::<D07ItemAppended>() { update(s,&e.data,3,mode); } else if let Some(e)=p.as_any().downcast_ref::<D07FlagToggled>() { update(s,&e.data,4,mode); } else if let Some(e)=p.as_any().downcast_ref::<D07CountAdded>() { update(s,&e.data,5,mode); } else if let Some(e)=p.as_any().downcast_ref::<D07TagChanged>() { update(s,&e.data,6,mode); } else if let Some(e)=p.as_any().downcast_ref::<D07Seen>() { update(s,&e.data,7,mode); } else if let Some(e)=p.as_any().downcast_ref::<D07Revised>() { update(s,&e.data,8,mode); } else if let Some(e)=p.as_any().downcast_ref::<D07Archived>() { update(s,&e.data,9,mode); } else {s.ignored+=1;}
}
fn apply8(p:&dyn EventPayload,s:&mut Store,mode:&str) {
if let Some(e)=p.as_any().downcast_ref::<D08Created>() { update(s,&e.data,0,mode); } else if let Some(e)=p.as_any().downcast_ref::<D08Renamed>() { update(s,&e.data,1,mode); } else if let Some(e)=p.as_any().downcast_ref::<D08BalanceAdded>() { update(s,&e.data,2,mode); } else if let Some(e)=p.as_any().downcast_ref::<D08ItemAppended>() { update(s,&e.data,3,mode); } else if let Some(e)=p.as_any().downcast_ref::<D08FlagToggled>() { update(s,&e.data,4,mode); } else if let Some(e)=p.as_any().downcast_ref::<D08CountAdded>() { update(s,&e.data,5,mode); } else if let Some(e)=p.as_any().downcast_ref::<D08TagChanged>() { update(s,&e.data,6,mode); } else if let Some(e)=p.as_any().downcast_ref::<D08Seen>() { update(s,&e.data,7,mode); } else if let Some(e)=p.as_any().downcast_ref::<D08Revised>() { update(s,&e.data,8,mode); } else if let Some(e)=p.as_any().downcast_ref::<D08Archived>() { update(s,&e.data,9,mode); } else {s.ignored+=1;}
}
fn apply9(p:&dyn EventPayload,s:&mut Store,mode:&str) {
if let Some(e)=p.as_any().downcast_ref::<D09Created>() { update(s,&e.data,0,mode); } else if let Some(e)=p.as_any().downcast_ref::<D09Renamed>() { update(s,&e.data,1,mode); } else if let Some(e)=p.as_any().downcast_ref::<D09BalanceAdded>() { update(s,&e.data,2,mode); } else if let Some(e)=p.as_any().downcast_ref::<D09ItemAppended>() { update(s,&e.data,3,mode); } else if let Some(e)=p.as_any().downcast_ref::<D09FlagToggled>() { update(s,&e.data,4,mode); } else if let Some(e)=p.as_any().downcast_ref::<D09CountAdded>() { update(s,&e.data,5,mode); } else if let Some(e)=p.as_any().downcast_ref::<D09TagChanged>() { update(s,&e.data,6,mode); } else if let Some(e)=p.as_any().downcast_ref::<D09Seen>() { update(s,&e.data,7,mode); } else if let Some(e)=p.as_any().downcast_ref::<D09Revised>() { update(s,&e.data,8,mode); } else if let Some(e)=p.as_any().downcast_ref::<D09Archived>() { update(s,&e.data,9,mode); } else {s.ignored+=1;}
}
fn update(store:&mut Store,e:&EventData,kind:usize,mode:&str){
 let s=if mode=="linear-append"{let pos=store.list.iter().position(|s|s.id==e.id).unwrap_or_else(||{store.list.push(State{id:e.id,..Default::default()});store.list.len()-1});&mut store.list[pos]}else{store.map.entry(e.id).or_insert_with(||State{id:e.id,..Default::default()})};
 match kind{0=>s.exists=true,1=>s.name=e.label.clone(),2=>s.balance+=e.value,3=>{if mode=="indexed-copy"{let mut copy=s.items.clone();copy.push(e.value);s.items=copy;}else{s.items.push(e.value);}},4=>s.flag=!s.flag,5=>s.counter+=e.value,6=>{s.tags.insert(e.value%16,e.value);},7=>s.seen=e.value,8=>s.revision=e.value,9=>s.archived=true,_=>unreachable!()}
}
fn main(){
 println!("{{\"event\":\"ready\"}}");std::io::stdout().flush().unwrap();
 let args:Vec<_>=std::env::args().collect();let events:usize=args[1].parse().unwrap();let aggregates:usize=args[2].parse().unwrap();let mode=&args[3];let routing=&args[4];
 let mut stores:Vec<Store>=(0..10).map(|_|Store::default()).collect();
 println!("{{\"event\":\"prepared\"}}");std::io::stdout().flush().unwrap();let start=Instant::now();
 for i in 0..events{let data=EventData{id:(i/100)%aggregates,value:(i+1)as i64,label:if i%10==1{format!("name-{}",i+1)}else{String::new()}};let p=create(i%100,data);
 if routing=="fanout"{for d in 0..10{route(d,p.as_ref(),&mut stores,mode);}}else{route((i%100)/10,p.as_ref(),&mut stores,mode);}}
 for d in 0..10{route(d,&Unknown,&mut stores,mode);}let elapsed=start.elapsed().as_secs_f64();
 let(mut sum,mut items,mut states,mut max_history,mut ignored)=(0i64,0usize,0usize,0usize,0i64);
 for(d,store)in stores.iter().enumerate(){ignored+=store.ignored;let all:Box<dyn Iterator<Item=&State>>=if mode=="linear-append"{Box::new(store.list.iter())}else{Box::new(store.map.values())};for s in all{states+=1;items+=s.items.len();max_history=max_history.max(s.items.len());sum+=s.id as i64+d as i64+s.balance+s.counter+s.seen+s.revision+s.name.len()as i64+s.exists as i64+s.flag as i64+s.archived as i64+s.items.iter().sum::<i64>()+s.items.len()as i64+s.tags.iter().map(|(k,v)|k+v).sum::<i64>();}}
 println!("{{\"roots\":{events},\"samples\":1,\"sink\":{sum},\"stateCount\":{states},\"retainedItems\":{items},\"maxHistoryLength\":{max_history},\"ignored\":{ignored},\"measurements\":[{{\"encodeSeconds\":0,\"decodeSeconds\":0,\"switchSeconds\":{elapsed},\"totalSeconds\":{elapsed},\"bytes\":{}}}]}}",(items*8).max(1));
}
