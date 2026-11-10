import { z } from "zod";
const EventDataSchema = z.object({id:z.number().int(),value:z.number().int(),label:z.string()});
import{performance}from"node:perf_hooks";
interface EventData{id:number;value:number;label:string}
interface IEventPayload{kind:string;data:EventData}
class Unknown implements IEventPayload{kind="Unknown";data={id:0,value:0,label:""}}
class D00Created implements IEventPayload {kind="D00Created";data:EventData;constructor(data:EventData){this.data=data;}}
class D00Renamed implements IEventPayload {kind="D00Renamed";data:EventData;constructor(data:EventData){this.data=data;}}
class D00BalanceAdded implements IEventPayload {kind="D00BalanceAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D00ItemAppended implements IEventPayload {kind="D00ItemAppended";data:EventData;constructor(data:EventData){this.data=data;}}
class D00FlagToggled implements IEventPayload {kind="D00FlagToggled";data:EventData;constructor(data:EventData){this.data=data;}}
class D00CountAdded implements IEventPayload {kind="D00CountAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D00TagChanged implements IEventPayload {kind="D00TagChanged";data:EventData;constructor(data:EventData){this.data=data;}}
class D00Seen implements IEventPayload {kind="D00Seen";data:EventData;constructor(data:EventData){this.data=data;}}
class D00Revised implements IEventPayload {kind="D00Revised";data:EventData;constructor(data:EventData){this.data=data;}}
class D00Archived implements IEventPayload {kind="D00Archived";data:EventData;constructor(data:EventData){this.data=data;}}
class D01Created implements IEventPayload {kind="D01Created";data:EventData;constructor(data:EventData){this.data=data;}}
class D01Renamed implements IEventPayload {kind="D01Renamed";data:EventData;constructor(data:EventData){this.data=data;}}
class D01BalanceAdded implements IEventPayload {kind="D01BalanceAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D01ItemAppended implements IEventPayload {kind="D01ItemAppended";data:EventData;constructor(data:EventData){this.data=data;}}
class D01FlagToggled implements IEventPayload {kind="D01FlagToggled";data:EventData;constructor(data:EventData){this.data=data;}}
class D01CountAdded implements IEventPayload {kind="D01CountAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D01TagChanged implements IEventPayload {kind="D01TagChanged";data:EventData;constructor(data:EventData){this.data=data;}}
class D01Seen implements IEventPayload {kind="D01Seen";data:EventData;constructor(data:EventData){this.data=data;}}
class D01Revised implements IEventPayload {kind="D01Revised";data:EventData;constructor(data:EventData){this.data=data;}}
class D01Archived implements IEventPayload {kind="D01Archived";data:EventData;constructor(data:EventData){this.data=data;}}
class D02Created implements IEventPayload {kind="D02Created";data:EventData;constructor(data:EventData){this.data=data;}}
class D02Renamed implements IEventPayload {kind="D02Renamed";data:EventData;constructor(data:EventData){this.data=data;}}
class D02BalanceAdded implements IEventPayload {kind="D02BalanceAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D02ItemAppended implements IEventPayload {kind="D02ItemAppended";data:EventData;constructor(data:EventData){this.data=data;}}
class D02FlagToggled implements IEventPayload {kind="D02FlagToggled";data:EventData;constructor(data:EventData){this.data=data;}}
class D02CountAdded implements IEventPayload {kind="D02CountAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D02TagChanged implements IEventPayload {kind="D02TagChanged";data:EventData;constructor(data:EventData){this.data=data;}}
class D02Seen implements IEventPayload {kind="D02Seen";data:EventData;constructor(data:EventData){this.data=data;}}
class D02Revised implements IEventPayload {kind="D02Revised";data:EventData;constructor(data:EventData){this.data=data;}}
class D02Archived implements IEventPayload {kind="D02Archived";data:EventData;constructor(data:EventData){this.data=data;}}
class D03Created implements IEventPayload {kind="D03Created";data:EventData;constructor(data:EventData){this.data=data;}}
class D03Renamed implements IEventPayload {kind="D03Renamed";data:EventData;constructor(data:EventData){this.data=data;}}
class D03BalanceAdded implements IEventPayload {kind="D03BalanceAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D03ItemAppended implements IEventPayload {kind="D03ItemAppended";data:EventData;constructor(data:EventData){this.data=data;}}
class D03FlagToggled implements IEventPayload {kind="D03FlagToggled";data:EventData;constructor(data:EventData){this.data=data;}}
class D03CountAdded implements IEventPayload {kind="D03CountAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D03TagChanged implements IEventPayload {kind="D03TagChanged";data:EventData;constructor(data:EventData){this.data=data;}}
class D03Seen implements IEventPayload {kind="D03Seen";data:EventData;constructor(data:EventData){this.data=data;}}
class D03Revised implements IEventPayload {kind="D03Revised";data:EventData;constructor(data:EventData){this.data=data;}}
class D03Archived implements IEventPayload {kind="D03Archived";data:EventData;constructor(data:EventData){this.data=data;}}
class D04Created implements IEventPayload {kind="D04Created";data:EventData;constructor(data:EventData){this.data=data;}}
class D04Renamed implements IEventPayload {kind="D04Renamed";data:EventData;constructor(data:EventData){this.data=data;}}
class D04BalanceAdded implements IEventPayload {kind="D04BalanceAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D04ItemAppended implements IEventPayload {kind="D04ItemAppended";data:EventData;constructor(data:EventData){this.data=data;}}
class D04FlagToggled implements IEventPayload {kind="D04FlagToggled";data:EventData;constructor(data:EventData){this.data=data;}}
class D04CountAdded implements IEventPayload {kind="D04CountAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D04TagChanged implements IEventPayload {kind="D04TagChanged";data:EventData;constructor(data:EventData){this.data=data;}}
class D04Seen implements IEventPayload {kind="D04Seen";data:EventData;constructor(data:EventData){this.data=data;}}
class D04Revised implements IEventPayload {kind="D04Revised";data:EventData;constructor(data:EventData){this.data=data;}}
class D04Archived implements IEventPayload {kind="D04Archived";data:EventData;constructor(data:EventData){this.data=data;}}
class D05Created implements IEventPayload {kind="D05Created";data:EventData;constructor(data:EventData){this.data=data;}}
class D05Renamed implements IEventPayload {kind="D05Renamed";data:EventData;constructor(data:EventData){this.data=data;}}
class D05BalanceAdded implements IEventPayload {kind="D05BalanceAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D05ItemAppended implements IEventPayload {kind="D05ItemAppended";data:EventData;constructor(data:EventData){this.data=data;}}
class D05FlagToggled implements IEventPayload {kind="D05FlagToggled";data:EventData;constructor(data:EventData){this.data=data;}}
class D05CountAdded implements IEventPayload {kind="D05CountAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D05TagChanged implements IEventPayload {kind="D05TagChanged";data:EventData;constructor(data:EventData){this.data=data;}}
class D05Seen implements IEventPayload {kind="D05Seen";data:EventData;constructor(data:EventData){this.data=data;}}
class D05Revised implements IEventPayload {kind="D05Revised";data:EventData;constructor(data:EventData){this.data=data;}}
class D05Archived implements IEventPayload {kind="D05Archived";data:EventData;constructor(data:EventData){this.data=data;}}
class D06Created implements IEventPayload {kind="D06Created";data:EventData;constructor(data:EventData){this.data=data;}}
class D06Renamed implements IEventPayload {kind="D06Renamed";data:EventData;constructor(data:EventData){this.data=data;}}
class D06BalanceAdded implements IEventPayload {kind="D06BalanceAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D06ItemAppended implements IEventPayload {kind="D06ItemAppended";data:EventData;constructor(data:EventData){this.data=data;}}
class D06FlagToggled implements IEventPayload {kind="D06FlagToggled";data:EventData;constructor(data:EventData){this.data=data;}}
class D06CountAdded implements IEventPayload {kind="D06CountAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D06TagChanged implements IEventPayload {kind="D06TagChanged";data:EventData;constructor(data:EventData){this.data=data;}}
class D06Seen implements IEventPayload {kind="D06Seen";data:EventData;constructor(data:EventData){this.data=data;}}
class D06Revised implements IEventPayload {kind="D06Revised";data:EventData;constructor(data:EventData){this.data=data;}}
class D06Archived implements IEventPayload {kind="D06Archived";data:EventData;constructor(data:EventData){this.data=data;}}
class D07Created implements IEventPayload {kind="D07Created";data:EventData;constructor(data:EventData){this.data=data;}}
class D07Renamed implements IEventPayload {kind="D07Renamed";data:EventData;constructor(data:EventData){this.data=data;}}
class D07BalanceAdded implements IEventPayload {kind="D07BalanceAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D07ItemAppended implements IEventPayload {kind="D07ItemAppended";data:EventData;constructor(data:EventData){this.data=data;}}
class D07FlagToggled implements IEventPayload {kind="D07FlagToggled";data:EventData;constructor(data:EventData){this.data=data;}}
class D07CountAdded implements IEventPayload {kind="D07CountAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D07TagChanged implements IEventPayload {kind="D07TagChanged";data:EventData;constructor(data:EventData){this.data=data;}}
class D07Seen implements IEventPayload {kind="D07Seen";data:EventData;constructor(data:EventData){this.data=data;}}
class D07Revised implements IEventPayload {kind="D07Revised";data:EventData;constructor(data:EventData){this.data=data;}}
class D07Archived implements IEventPayload {kind="D07Archived";data:EventData;constructor(data:EventData){this.data=data;}}
class D08Created implements IEventPayload {kind="D08Created";data:EventData;constructor(data:EventData){this.data=data;}}
class D08Renamed implements IEventPayload {kind="D08Renamed";data:EventData;constructor(data:EventData){this.data=data;}}
class D08BalanceAdded implements IEventPayload {kind="D08BalanceAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D08ItemAppended implements IEventPayload {kind="D08ItemAppended";data:EventData;constructor(data:EventData){this.data=data;}}
class D08FlagToggled implements IEventPayload {kind="D08FlagToggled";data:EventData;constructor(data:EventData){this.data=data;}}
class D08CountAdded implements IEventPayload {kind="D08CountAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D08TagChanged implements IEventPayload {kind="D08TagChanged";data:EventData;constructor(data:EventData){this.data=data;}}
class D08Seen implements IEventPayload {kind="D08Seen";data:EventData;constructor(data:EventData){this.data=data;}}
class D08Revised implements IEventPayload {kind="D08Revised";data:EventData;constructor(data:EventData){this.data=data;}}
class D08Archived implements IEventPayload {kind="D08Archived";data:EventData;constructor(data:EventData){this.data=data;}}
class D09Created implements IEventPayload {kind="D09Created";data:EventData;constructor(data:EventData){this.data=data;}}
class D09Renamed implements IEventPayload {kind="D09Renamed";data:EventData;constructor(data:EventData){this.data=data;}}
class D09BalanceAdded implements IEventPayload {kind="D09BalanceAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D09ItemAppended implements IEventPayload {kind="D09ItemAppended";data:EventData;constructor(data:EventData){this.data=data;}}
class D09FlagToggled implements IEventPayload {kind="D09FlagToggled";data:EventData;constructor(data:EventData){this.data=data;}}
class D09CountAdded implements IEventPayload {kind="D09CountAdded";data:EventData;constructor(data:EventData){this.data=data;}}
class D09TagChanged implements IEventPayload {kind="D09TagChanged";data:EventData;constructor(data:EventData){this.data=data;}}
class D09Seen implements IEventPayload {kind="D09Seen";data:EventData;constructor(data:EventData){this.data=data;}}
class D09Revised implements IEventPayload {kind="D09Revised";data:EventData;constructor(data:EventData){this.data=data;}}
class D09Archived implements IEventPayload {kind="D09Archived";data:EventData;constructor(data:EventData){this.data=data;}}
class State{id:number;balance=0;counter=0;seen=0;revision=0;name="";exists=false;flag=false;archived=false;items:number[]=[];tags=new Map<number,number>();constructor(id:number){this.id=id;}}
class Store{map=new Map<number,State>();list:State[]=[];ignored=0;}
function create(i:number,data:EventData):IEventPayload{switch(i){
case 0:return new D00Created(data);
case 1:return new D00Renamed(data);
case 2:return new D00BalanceAdded(data);
case 3:return new D00ItemAppended(data);
case 4:return new D00FlagToggled(data);
case 5:return new D00CountAdded(data);
case 6:return new D00TagChanged(data);
case 7:return new D00Seen(data);
case 8:return new D00Revised(data);
case 9:return new D00Archived(data);
case 10:return new D01Created(data);
case 11:return new D01Renamed(data);
case 12:return new D01BalanceAdded(data);
case 13:return new D01ItemAppended(data);
case 14:return new D01FlagToggled(data);
case 15:return new D01CountAdded(data);
case 16:return new D01TagChanged(data);
case 17:return new D01Seen(data);
case 18:return new D01Revised(data);
case 19:return new D01Archived(data);
case 20:return new D02Created(data);
case 21:return new D02Renamed(data);
case 22:return new D02BalanceAdded(data);
case 23:return new D02ItemAppended(data);
case 24:return new D02FlagToggled(data);
case 25:return new D02CountAdded(data);
case 26:return new D02TagChanged(data);
case 27:return new D02Seen(data);
case 28:return new D02Revised(data);
case 29:return new D02Archived(data);
case 30:return new D03Created(data);
case 31:return new D03Renamed(data);
case 32:return new D03BalanceAdded(data);
case 33:return new D03ItemAppended(data);
case 34:return new D03FlagToggled(data);
case 35:return new D03CountAdded(data);
case 36:return new D03TagChanged(data);
case 37:return new D03Seen(data);
case 38:return new D03Revised(data);
case 39:return new D03Archived(data);
case 40:return new D04Created(data);
case 41:return new D04Renamed(data);
case 42:return new D04BalanceAdded(data);
case 43:return new D04ItemAppended(data);
case 44:return new D04FlagToggled(data);
case 45:return new D04CountAdded(data);
case 46:return new D04TagChanged(data);
case 47:return new D04Seen(data);
case 48:return new D04Revised(data);
case 49:return new D04Archived(data);
case 50:return new D05Created(data);
case 51:return new D05Renamed(data);
case 52:return new D05BalanceAdded(data);
case 53:return new D05ItemAppended(data);
case 54:return new D05FlagToggled(data);
case 55:return new D05CountAdded(data);
case 56:return new D05TagChanged(data);
case 57:return new D05Seen(data);
case 58:return new D05Revised(data);
case 59:return new D05Archived(data);
case 60:return new D06Created(data);
case 61:return new D06Renamed(data);
case 62:return new D06BalanceAdded(data);
case 63:return new D06ItemAppended(data);
case 64:return new D06FlagToggled(data);
case 65:return new D06CountAdded(data);
case 66:return new D06TagChanged(data);
case 67:return new D06Seen(data);
case 68:return new D06Revised(data);
case 69:return new D06Archived(data);
case 70:return new D07Created(data);
case 71:return new D07Renamed(data);
case 72:return new D07BalanceAdded(data);
case 73:return new D07ItemAppended(data);
case 74:return new D07FlagToggled(data);
case 75:return new D07CountAdded(data);
case 76:return new D07TagChanged(data);
case 77:return new D07Seen(data);
case 78:return new D07Revised(data);
case 79:return new D07Archived(data);
case 80:return new D08Created(data);
case 81:return new D08Renamed(data);
case 82:return new D08BalanceAdded(data);
case 83:return new D08ItemAppended(data);
case 84:return new D08FlagToggled(data);
case 85:return new D08CountAdded(data);
case 86:return new D08TagChanged(data);
case 87:return new D08Seen(data);
case 88:return new D08Revised(data);
case 89:return new D08Archived(data);
case 90:return new D09Created(data);
case 91:return new D09Renamed(data);
case 92:return new D09BalanceAdded(data);
case 93:return new D09ItemAppended(data);
case 94:return new D09FlagToggled(data);
case 95:return new D09CountAdded(data);
case 96:return new D09TagChanged(data);
case 97:return new D09Seen(data);
case 98:return new D09Revised(data);
case 99:return new D09Archived(data);
default:throw Error("Unknown");}}
function route(domain:number,p:IEventPayload,stores:Store[],mode:string){switch(domain){
case 0:apply0(p,stores[0],mode);break;
case 1:apply1(p,stores[1],mode);break;
case 2:apply2(p,stores[2],mode);break;
case 3:apply3(p,stores[3],mode);break;
case 4:apply4(p,stores[4],mode);break;
case 5:apply5(p,stores[5],mode);break;
case 6:apply6(p,stores[6],mode);break;
case 7:apply7(p,stores[7],mode);break;
case 8:apply8(p,stores[8],mode);break;
case 9:apply9(p,stores[9],mode);break;
}}
function apply0(p:IEventPayload,s:Store,mode:string):void {switch(p.kind){
case "D00Created": update(s,EventDataSchema.parse(p.data),0,mode); break;
case "D00Renamed": update(s,EventDataSchema.parse(p.data),1,mode); break;
case "D00BalanceAdded": update(s,EventDataSchema.parse(p.data),2,mode); break;
case "D00ItemAppended": update(s,EventDataSchema.parse(p.data),3,mode); break;
case "D00FlagToggled": update(s,EventDataSchema.parse(p.data),4,mode); break;
case "D00CountAdded": update(s,EventDataSchema.parse(p.data),5,mode); break;
case "D00TagChanged": update(s,EventDataSchema.parse(p.data),6,mode); break;
case "D00Seen": update(s,EventDataSchema.parse(p.data),7,mode); break;
case "D00Revised": update(s,EventDataSchema.parse(p.data),8,mode); break;
case "D00Archived": update(s,EventDataSchema.parse(p.data),9,mode); break;
default:s.ignored++;}}
function apply1(p:IEventPayload,s:Store,mode:string):void {switch(p.kind){
case "D01Created": update(s,EventDataSchema.parse(p.data),0,mode); break;
case "D01Renamed": update(s,EventDataSchema.parse(p.data),1,mode); break;
case "D01BalanceAdded": update(s,EventDataSchema.parse(p.data),2,mode); break;
case "D01ItemAppended": update(s,EventDataSchema.parse(p.data),3,mode); break;
case "D01FlagToggled": update(s,EventDataSchema.parse(p.data),4,mode); break;
case "D01CountAdded": update(s,EventDataSchema.parse(p.data),5,mode); break;
case "D01TagChanged": update(s,EventDataSchema.parse(p.data),6,mode); break;
case "D01Seen": update(s,EventDataSchema.parse(p.data),7,mode); break;
case "D01Revised": update(s,EventDataSchema.parse(p.data),8,mode); break;
case "D01Archived": update(s,EventDataSchema.parse(p.data),9,mode); break;
default:s.ignored++;}}
function apply2(p:IEventPayload,s:Store,mode:string):void {switch(p.kind){
case "D02Created": update(s,EventDataSchema.parse(p.data),0,mode); break;
case "D02Renamed": update(s,EventDataSchema.parse(p.data),1,mode); break;
case "D02BalanceAdded": update(s,EventDataSchema.parse(p.data),2,mode); break;
case "D02ItemAppended": update(s,EventDataSchema.parse(p.data),3,mode); break;
case "D02FlagToggled": update(s,EventDataSchema.parse(p.data),4,mode); break;
case "D02CountAdded": update(s,EventDataSchema.parse(p.data),5,mode); break;
case "D02TagChanged": update(s,EventDataSchema.parse(p.data),6,mode); break;
case "D02Seen": update(s,EventDataSchema.parse(p.data),7,mode); break;
case "D02Revised": update(s,EventDataSchema.parse(p.data),8,mode); break;
case "D02Archived": update(s,EventDataSchema.parse(p.data),9,mode); break;
default:s.ignored++;}}
function apply3(p:IEventPayload,s:Store,mode:string):void {switch(p.kind){
case "D03Created": update(s,EventDataSchema.parse(p.data),0,mode); break;
case "D03Renamed": update(s,EventDataSchema.parse(p.data),1,mode); break;
case "D03BalanceAdded": update(s,EventDataSchema.parse(p.data),2,mode); break;
case "D03ItemAppended": update(s,EventDataSchema.parse(p.data),3,mode); break;
case "D03FlagToggled": update(s,EventDataSchema.parse(p.data),4,mode); break;
case "D03CountAdded": update(s,EventDataSchema.parse(p.data),5,mode); break;
case "D03TagChanged": update(s,EventDataSchema.parse(p.data),6,mode); break;
case "D03Seen": update(s,EventDataSchema.parse(p.data),7,mode); break;
case "D03Revised": update(s,EventDataSchema.parse(p.data),8,mode); break;
case "D03Archived": update(s,EventDataSchema.parse(p.data),9,mode); break;
default:s.ignored++;}}
function apply4(p:IEventPayload,s:Store,mode:string):void {switch(p.kind){
case "D04Created": update(s,EventDataSchema.parse(p.data),0,mode); break;
case "D04Renamed": update(s,EventDataSchema.parse(p.data),1,mode); break;
case "D04BalanceAdded": update(s,EventDataSchema.parse(p.data),2,mode); break;
case "D04ItemAppended": update(s,EventDataSchema.parse(p.data),3,mode); break;
case "D04FlagToggled": update(s,EventDataSchema.parse(p.data),4,mode); break;
case "D04CountAdded": update(s,EventDataSchema.parse(p.data),5,mode); break;
case "D04TagChanged": update(s,EventDataSchema.parse(p.data),6,mode); break;
case "D04Seen": update(s,EventDataSchema.parse(p.data),7,mode); break;
case "D04Revised": update(s,EventDataSchema.parse(p.data),8,mode); break;
case "D04Archived": update(s,EventDataSchema.parse(p.data),9,mode); break;
default:s.ignored++;}}
function apply5(p:IEventPayload,s:Store,mode:string):void {switch(p.kind){
case "D05Created": update(s,EventDataSchema.parse(p.data),0,mode); break;
case "D05Renamed": update(s,EventDataSchema.parse(p.data),1,mode); break;
case "D05BalanceAdded": update(s,EventDataSchema.parse(p.data),2,mode); break;
case "D05ItemAppended": update(s,EventDataSchema.parse(p.data),3,mode); break;
case "D05FlagToggled": update(s,EventDataSchema.parse(p.data),4,mode); break;
case "D05CountAdded": update(s,EventDataSchema.parse(p.data),5,mode); break;
case "D05TagChanged": update(s,EventDataSchema.parse(p.data),6,mode); break;
case "D05Seen": update(s,EventDataSchema.parse(p.data),7,mode); break;
case "D05Revised": update(s,EventDataSchema.parse(p.data),8,mode); break;
case "D05Archived": update(s,EventDataSchema.parse(p.data),9,mode); break;
default:s.ignored++;}}
function apply6(p:IEventPayload,s:Store,mode:string):void {switch(p.kind){
case "D06Created": update(s,EventDataSchema.parse(p.data),0,mode); break;
case "D06Renamed": update(s,EventDataSchema.parse(p.data),1,mode); break;
case "D06BalanceAdded": update(s,EventDataSchema.parse(p.data),2,mode); break;
case "D06ItemAppended": update(s,EventDataSchema.parse(p.data),3,mode); break;
case "D06FlagToggled": update(s,EventDataSchema.parse(p.data),4,mode); break;
case "D06CountAdded": update(s,EventDataSchema.parse(p.data),5,mode); break;
case "D06TagChanged": update(s,EventDataSchema.parse(p.data),6,mode); break;
case "D06Seen": update(s,EventDataSchema.parse(p.data),7,mode); break;
case "D06Revised": update(s,EventDataSchema.parse(p.data),8,mode); break;
case "D06Archived": update(s,EventDataSchema.parse(p.data),9,mode); break;
default:s.ignored++;}}
function apply7(p:IEventPayload,s:Store,mode:string):void {switch(p.kind){
case "D07Created": update(s,EventDataSchema.parse(p.data),0,mode); break;
case "D07Renamed": update(s,EventDataSchema.parse(p.data),1,mode); break;
case "D07BalanceAdded": update(s,EventDataSchema.parse(p.data),2,mode); break;
case "D07ItemAppended": update(s,EventDataSchema.parse(p.data),3,mode); break;
case "D07FlagToggled": update(s,EventDataSchema.parse(p.data),4,mode); break;
case "D07CountAdded": update(s,EventDataSchema.parse(p.data),5,mode); break;
case "D07TagChanged": update(s,EventDataSchema.parse(p.data),6,mode); break;
case "D07Seen": update(s,EventDataSchema.parse(p.data),7,mode); break;
case "D07Revised": update(s,EventDataSchema.parse(p.data),8,mode); break;
case "D07Archived": update(s,EventDataSchema.parse(p.data),9,mode); break;
default:s.ignored++;}}
function apply8(p:IEventPayload,s:Store,mode:string):void {switch(p.kind){
case "D08Created": update(s,EventDataSchema.parse(p.data),0,mode); break;
case "D08Renamed": update(s,EventDataSchema.parse(p.data),1,mode); break;
case "D08BalanceAdded": update(s,EventDataSchema.parse(p.data),2,mode); break;
case "D08ItemAppended": update(s,EventDataSchema.parse(p.data),3,mode); break;
case "D08FlagToggled": update(s,EventDataSchema.parse(p.data),4,mode); break;
case "D08CountAdded": update(s,EventDataSchema.parse(p.data),5,mode); break;
case "D08TagChanged": update(s,EventDataSchema.parse(p.data),6,mode); break;
case "D08Seen": update(s,EventDataSchema.parse(p.data),7,mode); break;
case "D08Revised": update(s,EventDataSchema.parse(p.data),8,mode); break;
case "D08Archived": update(s,EventDataSchema.parse(p.data),9,mode); break;
default:s.ignored++;}}
function apply9(p:IEventPayload,s:Store,mode:string):void {switch(p.kind){
case "D09Created": update(s,EventDataSchema.parse(p.data),0,mode); break;
case "D09Renamed": update(s,EventDataSchema.parse(p.data),1,mode); break;
case "D09BalanceAdded": update(s,EventDataSchema.parse(p.data),2,mode); break;
case "D09ItemAppended": update(s,EventDataSchema.parse(p.data),3,mode); break;
case "D09FlagToggled": update(s,EventDataSchema.parse(p.data),4,mode); break;
case "D09CountAdded": update(s,EventDataSchema.parse(p.data),5,mode); break;
case "D09TagChanged": update(s,EventDataSchema.parse(p.data),6,mode); break;
case "D09Seen": update(s,EventDataSchema.parse(p.data),7,mode); break;
case "D09Revised": update(s,EventDataSchema.parse(p.data),8,mode); break;
case "D09Archived": update(s,EventDataSchema.parse(p.data),9,mode); break;
default:s.ignored++;}}
function update(store:Store,e:EventData,kind:number,mode:string){
 let s=mode==="linear-append"?store.list.find(v=>v.id===e.id):store.map.get(e.id);
 if(!s){s=new State(e.id);if(mode==="linear-append")store.list.push(s);else store.map.set(e.id,s);}
 switch(kind){case 0:s.exists=true;break;case 1:s.name=e.label;break;case 2:s.balance+=e.value;break;case 3:if(mode==="indexed-copy")s.items=s.items.concat(e.value);else s.items.push(e.value);break;case 4:s.flag=!s.flag;break;case 5:s.counter+=e.value;break;case 6:s.tags.set(e.value%16,e.value);break;case 7:s.seen=e.value;break;case 8:s.revision=e.value;break;case 9:s.archived=true;break;}
}
console.log(JSON.stringify({event:"ready"}));
const events=Number(process.argv[2]),aggregates=Number(process.argv[3]),mode=process.argv[4],routing=process.argv[5];
const stores=Array.from({length:10},()=>new Store());console.log(JSON.stringify({event:"prepared"}));const start=performance.now();
for(let i=0;i<events;i++){const p=create(i%100,{id:Math.floor(i/100)%aggregates,value:i+1,label:i%10===1?`name-${i+1}`:""});if(routing==="fanout")for(let d=0;d<10;d++)route(d,p,stores,mode);else route(Math.floor((i%100)/10),p,stores,mode);}
for(let d=0;d<10;d++)route(d,new Unknown(),stores,mode);const elapsed=(performance.now()-start)/1000;
let sum=0,items=0,states=0,maxHistory=0,ignored=0;
stores.forEach((store,d)=>{ignored+=store.ignored;for(const s of mode==="linear-append"?store.list:store.map.values()){states++;items+=s.items.length;maxHistory=Math.max(maxHistory,s.items.length);sum+=s.id+d+s.balance+s.counter+s.seen+s.revision+s.name.length+Number(s.exists)+Number(s.flag)+Number(s.archived)+s.items.reduce((a,b)=>a+b,0)+s.items.length+[...s.tags].reduce((a,[k,v])=>a+k+v,0);}});
console.log(JSON.stringify({roots:events,samples:1,sink:sum,stateCount:states,retainedItems:items,maxHistoryLength:maxHistory,ignored,measurements:[{encodeSeconds:0,decodeSeconds:0,switchSeconds:elapsed,totalSeconds:elapsed,bytes:Math.max(1,items*8)}]}));
