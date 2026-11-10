import { performance } from "node:perf_hooks";
type Payload={id:number;label:string;values:number[];tags:Record<string,string>;next:Payment|null};
@@TYPES@@
type Payment=@@UNION@@;
function create(seed:number,depth:number):Payment {
  const data:Payload={id:seed,label:`item-${seed}`,values:[seed,seed+1,seed+2,seed+3],tags:{group:`g${seed%7}`,note:"bench"},next:depth>1?create(seed+37,depth-1):null};
  switch(seed%100){
@@CREATE@@
    default:throw Error("Unknown case");
  }
}
function score(payment:Payment):number {
  let index:number;
  switch(payment.$type){
@@DISPATCH@@
    default:{const exhaustive:never=payment;throw Error(String(exhaustive));}
  }
  const d=payment.data;
  return index+d.id+d.label.length+d.values.reduce((a,b)=>a+b,0)+Object.entries(d.tags).reduce((sum,[k,v])=>sum+k.length+v.length,0)+(d.next?score(d.next):0);
}
// JSON.parse alone does not restore a verified TS type: validate every node.
function parse(json:string):Payment[] {
  const nodes:unknown=JSON.parse(json);
  function validate(p:any):void {
    if(!p || typeof p.$type!=="string" || !/^Case\d{3}$/.test(p.$type) || Number(p.$type.slice(4))>=100)throw Error("Invalid case");
    const d=p.data;
    if(!d || !Number.isInteger(d.id)||typeof d.label!=="string"||!Array.isArray(d.values)||!d.values.every(Number.isInteger)||!d.tags||Array.isArray(d.tags)||typeof d.tags!=="object"||!Object.values(d.tags).every(v=>typeof v==="string"))throw Error("Invalid payload");
    if(d.next!==null)validate(d.next);
  }
  if(!Array.isArray(nodes))throw Error("Invalid batch");
  nodes.forEach(validate);return nodes as Payment[];
}
console.log(JSON.stringify({event:"ready"}));
const roots=Number(process.argv[2]),samples=Number(process.argv[3]);
parse(JSON.stringify(Array.from({length:Math.min(roots,1000)},(_,i)=>create(i,3))));
const original=Array.from({length:roots},(_,i)=>create(i,3));
const expected=original.reduce((s,p)=>s+score(p),0);let sink=0;const measurements=[];
console.log(JSON.stringify({event:"prepared"}));
for(let trial=0;trial<samples;trial++){
  const start=performance.now();const json=JSON.stringify(original);const encoded=performance.now();
  const copy=parse(json);const decoded=performance.now();const sum=copy.reduce((s,p)=>s+score(p),0);const end=performance.now();
  if(sum!==expected||copy.length!==roots)throw Error("Incorrect round trip");
  if(trial===0&&JSON.stringify(copy)!==json)throw Error("Data changed");sink+=sum;
  measurements.push({encodeSeconds:(encoded-start)/1000,decodeSeconds:(decoded-encoded)/1000,switchSeconds:(end-decoded)/1000,totalSeconds:(end-start)/1000,bytes:Buffer.byteLength(json)});
}
console.log(JSON.stringify({roots,samples,sink,measurements}));
