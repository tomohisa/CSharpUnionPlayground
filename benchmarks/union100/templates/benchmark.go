package main
import("bytes";"encoding/json";"fmt";"os";"strconv";"time")
type Payment interface{isPayment()}
type Payload struct{Id int;Label string;Values []int;Tags map[string]string;Next Payment}
@@TYPES@@
func create(seed,depth int)Payment{
    var next Payment;if depth>1{next=create(seed+37,depth-1)}
    data:=Payload{seed,fmt.Sprintf("item-%d",seed),[]int{seed,seed+1,seed+2,seed+3},map[string]string{"group":fmt.Sprintf("g%d",seed%7),"note":"bench"},next}
    switch seed%100{
@@CREATE@@
    };panic("Unknown case")
}
func score(p Payment)int64{switch v:=p.(type){
@@DISPATCH@@
};panic("Unknown case")}
func payloadScore(d Payload,index int)int64{
    sum:=int64(index+d.Id+len(d.Label));for _,v:=range d.Values{sum+=int64(v)};for k,v:=range d.Tags{sum+=int64(len(k)+len(v))};if d.Next!=nil{sum+=score(d.Next)};return sum
}
type WirePayload struct{Id int `json:"id"`;Label string `json:"label"`;Values []int `json:"values"`;Tags map[string]string `json:"tags"`;Next json.RawMessage `json:"next"`}
type WireCase struct{Type string `json:"$type"`;Data WirePayload `json:"data"`}
func encode(p Payment)(json.RawMessage,error){
    if p==nil{return json.RawMessage("null"),nil};var name string;var data Payload
    switch v:=p.(type){
@@ENCODE@@
    default:return nil,fmt.Errorf("Unknown case")}
    next,err:=encode(data.Next);if err!=nil{return nil,err}
    return json.Marshal(WireCase{name,WirePayload{data.Id,data.Label,data.Values,data.Tags,next}})
}
func decode(raw json.RawMessage)(Payment,error){
    if bytes.Equal(raw,[]byte("null")){return nil,nil};var wire WireCase;if err:=json.Unmarshal(raw,&wire);err!=nil{return nil,err}
    next,err:=decode(wire.Data.Next);if err!=nil{return nil,err};d:=wire.Data;data:=Payload{d.Id,d.Label,d.Values,d.Tags,next}
    switch wire.Type{
@@DECODE@@
    };return nil,fmt.Errorf("Unknown tag")
}
func encodeBatch(batch []Payment)[]byte{raw:=make([]json.RawMessage,len(batch));for i,p:=range batch{var err error;raw[i],err=encode(p);if err!=nil{panic(err)}};b,err:=json.Marshal(raw);if err!=nil{panic(err)};return b}
func decodeBatch(b []byte)[]Payment{var raw []json.RawMessage;if err:=json.Unmarshal(b,&raw);err!=nil{panic(err)};result:=make([]Payment,len(raw));for i,r:=range raw{var err error;result[i],err=decode(r);if err!=nil{panic(err)}};return result}
func batch(roots int)[]Payment{r:=make([]Payment,roots);for i:=range r{r[i]=create(i,3)};return r}
func main(){
    fmt.Println(`{"event":"ready"}`)
    roots,_:=strconv.Atoi(os.Args[1]);samples,_:=strconv.Atoi(os.Args[2]);decodeBatch(encodeBatch(batch(min(roots,1000))))
    original:=batch(roots);var expected,sink int64;for _,p:=range original{expected+=score(p)};measurements:=make([]map[string]any,0,samples)
    fmt.Println(`{"event":"prepared"}`)
    for trial:=0;trial<samples;trial++{
        start:=time.Now();b:=encodeBatch(original);encoded:=time.Now();copy:=decodeBatch(b);decoded:=time.Now();var sum int64;for _,p:=range copy{sum+=score(p)};end:=time.Now()
        if sum!=expected||len(copy)!=roots{panic("Incorrect round trip")};if trial==0&&!bytes.Equal(b,encodeBatch(copy)){panic("Data changed")};sink+=sum
        measurements=append(measurements,map[string]any{"encodeSeconds":encoded.Sub(start).Seconds(),"decodeSeconds":decoded.Sub(encoded).Seconds(),"switchSeconds":end.Sub(decoded).Seconds(),"totalSeconds":end.Sub(start).Seconds(),"bytes":len(b)})
    }
    result,_:=json.Marshal(map[string]any{"roots":roots,"samples":samples,"sink":sink,"measurements":measurements});fmt.Println(string(result))
}
