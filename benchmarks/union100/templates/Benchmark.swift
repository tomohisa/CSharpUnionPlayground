import Foundation
struct Payload: Codable {let id:Int;let label:String;let values:[Int];let tags:[String:String];let next:Payment?}
@@TYPES@@
indirect enum Payment: Codable {
@@CASES@@
}
func create(_ seed:Int,_ depth:Int)->Payment{
    let data=Payload(id:seed,label:"item-\(seed)",values:[seed,seed+1,seed+2,seed+3],tags:["group":"g\(seed%7)","note":"bench"],next:depth>1 ? create(seed+37,depth-1):nil)
    switch seed%100{
@@CREATE@@
    default:fatalError("Unknown case")
    }
}
func score(_ p:Payment)->Int64{
    switch p{
@@DISPATCH@@
    }
}
func payloadScore(_ data:Payload,_ index:Int)->Int64{
    return Int64(index+data.id+data.label.count+data.values.reduce(0,+)+data.tags.reduce(0){$0+$1.key.count+$1.value.count})+(data.next.map(score) ?? 0)
}
func now()->Double{Double(DispatchTime.now().uptimeNanoseconds)/1e9}
FileHandle.standardOutput.write(Data("{\"event\":\"ready\"}\n".utf8))
let roots=Int(CommandLine.arguments[1])!,samples=Int(CommandLine.arguments[2])!
let encoder=JSONEncoder();encoder.outputFormatting=[.sortedKeys];let decoder=JSONDecoder()
let warm=(0..<min(roots,1000)).map{create($0,3)}
_ = try decoder.decode([Payment].self,from:encoder.encode(warm))
let original=(0..<roots).map{create($0,3)},expected=original.reduce(Int64(0)){$0+score($1)}
var sink:Int64=0;var measurements:[[String:Any]]=[]
FileHandle.standardOutput.write(Data("{\"event\":\"prepared\"}\n".utf8))
for trial in 0..<samples{
    let start=now();let json=try encoder.encode(original);let encoded=now()
    let copy=try decoder.decode([Payment].self,from:json);let decoded=now();let sum=copy.reduce(Int64(0)){$0+score($1)};let end=now()
    precondition(sum==expected && copy.count==roots,"Incorrect round trip")
    if trial==0{let again=try encoder.encode(copy);precondition(again==json,"Data changed")};sink+=sum
    measurements.append(["encodeSeconds":encoded-start,"decodeSeconds":decoded-encoded,"switchSeconds":end-decoded,"totalSeconds":end-start,"bytes":json.count])
}
let result=try JSONSerialization.data(withJSONObject:["roots":roots,"samples":samples,"sink":sink,"measurements":measurements],options:[.sortedKeys])
FileHandle.standardOutput.write(result)
FileHandle.standardOutput.write(Data("\n".utf8))
