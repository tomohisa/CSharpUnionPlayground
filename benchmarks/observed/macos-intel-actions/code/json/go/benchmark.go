package main
import("bytes";"encoding/json";"fmt";"os";"strconv";"time")
type Payment interface{isPayment()}
type Payload struct{Id int;Label string;Values []int;Tags map[string]string;Next Payment}
type Case000 struct { Data Payload `json:"data"` }
func (Case000) isPayment() {}
type Case001 struct { Data Payload `json:"data"` }
func (Case001) isPayment() {}
type Case002 struct { Data Payload `json:"data"` }
func (Case002) isPayment() {}
type Case003 struct { Data Payload `json:"data"` }
func (Case003) isPayment() {}
type Case004 struct { Data Payload `json:"data"` }
func (Case004) isPayment() {}
type Case005 struct { Data Payload `json:"data"` }
func (Case005) isPayment() {}
type Case006 struct { Data Payload `json:"data"` }
func (Case006) isPayment() {}
type Case007 struct { Data Payload `json:"data"` }
func (Case007) isPayment() {}
type Case008 struct { Data Payload `json:"data"` }
func (Case008) isPayment() {}
type Case009 struct { Data Payload `json:"data"` }
func (Case009) isPayment() {}
type Case010 struct { Data Payload `json:"data"` }
func (Case010) isPayment() {}
type Case011 struct { Data Payload `json:"data"` }
func (Case011) isPayment() {}
type Case012 struct { Data Payload `json:"data"` }
func (Case012) isPayment() {}
type Case013 struct { Data Payload `json:"data"` }
func (Case013) isPayment() {}
type Case014 struct { Data Payload `json:"data"` }
func (Case014) isPayment() {}
type Case015 struct { Data Payload `json:"data"` }
func (Case015) isPayment() {}
type Case016 struct { Data Payload `json:"data"` }
func (Case016) isPayment() {}
type Case017 struct { Data Payload `json:"data"` }
func (Case017) isPayment() {}
type Case018 struct { Data Payload `json:"data"` }
func (Case018) isPayment() {}
type Case019 struct { Data Payload `json:"data"` }
func (Case019) isPayment() {}
type Case020 struct { Data Payload `json:"data"` }
func (Case020) isPayment() {}
type Case021 struct { Data Payload `json:"data"` }
func (Case021) isPayment() {}
type Case022 struct { Data Payload `json:"data"` }
func (Case022) isPayment() {}
type Case023 struct { Data Payload `json:"data"` }
func (Case023) isPayment() {}
type Case024 struct { Data Payload `json:"data"` }
func (Case024) isPayment() {}
type Case025 struct { Data Payload `json:"data"` }
func (Case025) isPayment() {}
type Case026 struct { Data Payload `json:"data"` }
func (Case026) isPayment() {}
type Case027 struct { Data Payload `json:"data"` }
func (Case027) isPayment() {}
type Case028 struct { Data Payload `json:"data"` }
func (Case028) isPayment() {}
type Case029 struct { Data Payload `json:"data"` }
func (Case029) isPayment() {}
type Case030 struct { Data Payload `json:"data"` }
func (Case030) isPayment() {}
type Case031 struct { Data Payload `json:"data"` }
func (Case031) isPayment() {}
type Case032 struct { Data Payload `json:"data"` }
func (Case032) isPayment() {}
type Case033 struct { Data Payload `json:"data"` }
func (Case033) isPayment() {}
type Case034 struct { Data Payload `json:"data"` }
func (Case034) isPayment() {}
type Case035 struct { Data Payload `json:"data"` }
func (Case035) isPayment() {}
type Case036 struct { Data Payload `json:"data"` }
func (Case036) isPayment() {}
type Case037 struct { Data Payload `json:"data"` }
func (Case037) isPayment() {}
type Case038 struct { Data Payload `json:"data"` }
func (Case038) isPayment() {}
type Case039 struct { Data Payload `json:"data"` }
func (Case039) isPayment() {}
type Case040 struct { Data Payload `json:"data"` }
func (Case040) isPayment() {}
type Case041 struct { Data Payload `json:"data"` }
func (Case041) isPayment() {}
type Case042 struct { Data Payload `json:"data"` }
func (Case042) isPayment() {}
type Case043 struct { Data Payload `json:"data"` }
func (Case043) isPayment() {}
type Case044 struct { Data Payload `json:"data"` }
func (Case044) isPayment() {}
type Case045 struct { Data Payload `json:"data"` }
func (Case045) isPayment() {}
type Case046 struct { Data Payload `json:"data"` }
func (Case046) isPayment() {}
type Case047 struct { Data Payload `json:"data"` }
func (Case047) isPayment() {}
type Case048 struct { Data Payload `json:"data"` }
func (Case048) isPayment() {}
type Case049 struct { Data Payload `json:"data"` }
func (Case049) isPayment() {}
type Case050 struct { Data Payload `json:"data"` }
func (Case050) isPayment() {}
type Case051 struct { Data Payload `json:"data"` }
func (Case051) isPayment() {}
type Case052 struct { Data Payload `json:"data"` }
func (Case052) isPayment() {}
type Case053 struct { Data Payload `json:"data"` }
func (Case053) isPayment() {}
type Case054 struct { Data Payload `json:"data"` }
func (Case054) isPayment() {}
type Case055 struct { Data Payload `json:"data"` }
func (Case055) isPayment() {}
type Case056 struct { Data Payload `json:"data"` }
func (Case056) isPayment() {}
type Case057 struct { Data Payload `json:"data"` }
func (Case057) isPayment() {}
type Case058 struct { Data Payload `json:"data"` }
func (Case058) isPayment() {}
type Case059 struct { Data Payload `json:"data"` }
func (Case059) isPayment() {}
type Case060 struct { Data Payload `json:"data"` }
func (Case060) isPayment() {}
type Case061 struct { Data Payload `json:"data"` }
func (Case061) isPayment() {}
type Case062 struct { Data Payload `json:"data"` }
func (Case062) isPayment() {}
type Case063 struct { Data Payload `json:"data"` }
func (Case063) isPayment() {}
type Case064 struct { Data Payload `json:"data"` }
func (Case064) isPayment() {}
type Case065 struct { Data Payload `json:"data"` }
func (Case065) isPayment() {}
type Case066 struct { Data Payload `json:"data"` }
func (Case066) isPayment() {}
type Case067 struct { Data Payload `json:"data"` }
func (Case067) isPayment() {}
type Case068 struct { Data Payload `json:"data"` }
func (Case068) isPayment() {}
type Case069 struct { Data Payload `json:"data"` }
func (Case069) isPayment() {}
type Case070 struct { Data Payload `json:"data"` }
func (Case070) isPayment() {}
type Case071 struct { Data Payload `json:"data"` }
func (Case071) isPayment() {}
type Case072 struct { Data Payload `json:"data"` }
func (Case072) isPayment() {}
type Case073 struct { Data Payload `json:"data"` }
func (Case073) isPayment() {}
type Case074 struct { Data Payload `json:"data"` }
func (Case074) isPayment() {}
type Case075 struct { Data Payload `json:"data"` }
func (Case075) isPayment() {}
type Case076 struct { Data Payload `json:"data"` }
func (Case076) isPayment() {}
type Case077 struct { Data Payload `json:"data"` }
func (Case077) isPayment() {}
type Case078 struct { Data Payload `json:"data"` }
func (Case078) isPayment() {}
type Case079 struct { Data Payload `json:"data"` }
func (Case079) isPayment() {}
type Case080 struct { Data Payload `json:"data"` }
func (Case080) isPayment() {}
type Case081 struct { Data Payload `json:"data"` }
func (Case081) isPayment() {}
type Case082 struct { Data Payload `json:"data"` }
func (Case082) isPayment() {}
type Case083 struct { Data Payload `json:"data"` }
func (Case083) isPayment() {}
type Case084 struct { Data Payload `json:"data"` }
func (Case084) isPayment() {}
type Case085 struct { Data Payload `json:"data"` }
func (Case085) isPayment() {}
type Case086 struct { Data Payload `json:"data"` }
func (Case086) isPayment() {}
type Case087 struct { Data Payload `json:"data"` }
func (Case087) isPayment() {}
type Case088 struct { Data Payload `json:"data"` }
func (Case088) isPayment() {}
type Case089 struct { Data Payload `json:"data"` }
func (Case089) isPayment() {}
type Case090 struct { Data Payload `json:"data"` }
func (Case090) isPayment() {}
type Case091 struct { Data Payload `json:"data"` }
func (Case091) isPayment() {}
type Case092 struct { Data Payload `json:"data"` }
func (Case092) isPayment() {}
type Case093 struct { Data Payload `json:"data"` }
func (Case093) isPayment() {}
type Case094 struct { Data Payload `json:"data"` }
func (Case094) isPayment() {}
type Case095 struct { Data Payload `json:"data"` }
func (Case095) isPayment() {}
type Case096 struct { Data Payload `json:"data"` }
func (Case096) isPayment() {}
type Case097 struct { Data Payload `json:"data"` }
func (Case097) isPayment() {}
type Case098 struct { Data Payload `json:"data"` }
func (Case098) isPayment() {}
type Case099 struct { Data Payload `json:"data"` }
func (Case099) isPayment() {}
func create(seed,depth int)Payment{
    var next Payment;if depth>1{next=create(seed+37,depth-1)}
    data:=Payload{seed,fmt.Sprintf("item-%d",seed),[]int{seed,seed+1,seed+2,seed+3},map[string]string{"group":fmt.Sprintf("g%d",seed%7),"note":"bench"},next}
    switch seed%100{
    case 0: return Case000{data}
    case 1: return Case001{data}
    case 2: return Case002{data}
    case 3: return Case003{data}
    case 4: return Case004{data}
    case 5: return Case005{data}
    case 6: return Case006{data}
    case 7: return Case007{data}
    case 8: return Case008{data}
    case 9: return Case009{data}
    case 10: return Case010{data}
    case 11: return Case011{data}
    case 12: return Case012{data}
    case 13: return Case013{data}
    case 14: return Case014{data}
    case 15: return Case015{data}
    case 16: return Case016{data}
    case 17: return Case017{data}
    case 18: return Case018{data}
    case 19: return Case019{data}
    case 20: return Case020{data}
    case 21: return Case021{data}
    case 22: return Case022{data}
    case 23: return Case023{data}
    case 24: return Case024{data}
    case 25: return Case025{data}
    case 26: return Case026{data}
    case 27: return Case027{data}
    case 28: return Case028{data}
    case 29: return Case029{data}
    case 30: return Case030{data}
    case 31: return Case031{data}
    case 32: return Case032{data}
    case 33: return Case033{data}
    case 34: return Case034{data}
    case 35: return Case035{data}
    case 36: return Case036{data}
    case 37: return Case037{data}
    case 38: return Case038{data}
    case 39: return Case039{data}
    case 40: return Case040{data}
    case 41: return Case041{data}
    case 42: return Case042{data}
    case 43: return Case043{data}
    case 44: return Case044{data}
    case 45: return Case045{data}
    case 46: return Case046{data}
    case 47: return Case047{data}
    case 48: return Case048{data}
    case 49: return Case049{data}
    case 50: return Case050{data}
    case 51: return Case051{data}
    case 52: return Case052{data}
    case 53: return Case053{data}
    case 54: return Case054{data}
    case 55: return Case055{data}
    case 56: return Case056{data}
    case 57: return Case057{data}
    case 58: return Case058{data}
    case 59: return Case059{data}
    case 60: return Case060{data}
    case 61: return Case061{data}
    case 62: return Case062{data}
    case 63: return Case063{data}
    case 64: return Case064{data}
    case 65: return Case065{data}
    case 66: return Case066{data}
    case 67: return Case067{data}
    case 68: return Case068{data}
    case 69: return Case069{data}
    case 70: return Case070{data}
    case 71: return Case071{data}
    case 72: return Case072{data}
    case 73: return Case073{data}
    case 74: return Case074{data}
    case 75: return Case075{data}
    case 76: return Case076{data}
    case 77: return Case077{data}
    case 78: return Case078{data}
    case 79: return Case079{data}
    case 80: return Case080{data}
    case 81: return Case081{data}
    case 82: return Case082{data}
    case 83: return Case083{data}
    case 84: return Case084{data}
    case 85: return Case085{data}
    case 86: return Case086{data}
    case 87: return Case087{data}
    case 88: return Case088{data}
    case 89: return Case089{data}
    case 90: return Case090{data}
    case 91: return Case091{data}
    case 92: return Case092{data}
    case 93: return Case093{data}
    case 94: return Case094{data}
    case 95: return Case095{data}
    case 96: return Case096{data}
    case 97: return Case097{data}
    case 98: return Case098{data}
    case 99: return Case099{data}
    };panic("Unknown case")
}
func score(p Payment)int64{switch v:=p.(type){
    case Case000: return payloadScore(v.Data, 0)
    case Case001: return payloadScore(v.Data, 1)
    case Case002: return payloadScore(v.Data, 2)
    case Case003: return payloadScore(v.Data, 3)
    case Case004: return payloadScore(v.Data, 4)
    case Case005: return payloadScore(v.Data, 5)
    case Case006: return payloadScore(v.Data, 6)
    case Case007: return payloadScore(v.Data, 7)
    case Case008: return payloadScore(v.Data, 8)
    case Case009: return payloadScore(v.Data, 9)
    case Case010: return payloadScore(v.Data, 10)
    case Case011: return payloadScore(v.Data, 11)
    case Case012: return payloadScore(v.Data, 12)
    case Case013: return payloadScore(v.Data, 13)
    case Case014: return payloadScore(v.Data, 14)
    case Case015: return payloadScore(v.Data, 15)
    case Case016: return payloadScore(v.Data, 16)
    case Case017: return payloadScore(v.Data, 17)
    case Case018: return payloadScore(v.Data, 18)
    case Case019: return payloadScore(v.Data, 19)
    case Case020: return payloadScore(v.Data, 20)
    case Case021: return payloadScore(v.Data, 21)
    case Case022: return payloadScore(v.Data, 22)
    case Case023: return payloadScore(v.Data, 23)
    case Case024: return payloadScore(v.Data, 24)
    case Case025: return payloadScore(v.Data, 25)
    case Case026: return payloadScore(v.Data, 26)
    case Case027: return payloadScore(v.Data, 27)
    case Case028: return payloadScore(v.Data, 28)
    case Case029: return payloadScore(v.Data, 29)
    case Case030: return payloadScore(v.Data, 30)
    case Case031: return payloadScore(v.Data, 31)
    case Case032: return payloadScore(v.Data, 32)
    case Case033: return payloadScore(v.Data, 33)
    case Case034: return payloadScore(v.Data, 34)
    case Case035: return payloadScore(v.Data, 35)
    case Case036: return payloadScore(v.Data, 36)
    case Case037: return payloadScore(v.Data, 37)
    case Case038: return payloadScore(v.Data, 38)
    case Case039: return payloadScore(v.Data, 39)
    case Case040: return payloadScore(v.Data, 40)
    case Case041: return payloadScore(v.Data, 41)
    case Case042: return payloadScore(v.Data, 42)
    case Case043: return payloadScore(v.Data, 43)
    case Case044: return payloadScore(v.Data, 44)
    case Case045: return payloadScore(v.Data, 45)
    case Case046: return payloadScore(v.Data, 46)
    case Case047: return payloadScore(v.Data, 47)
    case Case048: return payloadScore(v.Data, 48)
    case Case049: return payloadScore(v.Data, 49)
    case Case050: return payloadScore(v.Data, 50)
    case Case051: return payloadScore(v.Data, 51)
    case Case052: return payloadScore(v.Data, 52)
    case Case053: return payloadScore(v.Data, 53)
    case Case054: return payloadScore(v.Data, 54)
    case Case055: return payloadScore(v.Data, 55)
    case Case056: return payloadScore(v.Data, 56)
    case Case057: return payloadScore(v.Data, 57)
    case Case058: return payloadScore(v.Data, 58)
    case Case059: return payloadScore(v.Data, 59)
    case Case060: return payloadScore(v.Data, 60)
    case Case061: return payloadScore(v.Data, 61)
    case Case062: return payloadScore(v.Data, 62)
    case Case063: return payloadScore(v.Data, 63)
    case Case064: return payloadScore(v.Data, 64)
    case Case065: return payloadScore(v.Data, 65)
    case Case066: return payloadScore(v.Data, 66)
    case Case067: return payloadScore(v.Data, 67)
    case Case068: return payloadScore(v.Data, 68)
    case Case069: return payloadScore(v.Data, 69)
    case Case070: return payloadScore(v.Data, 70)
    case Case071: return payloadScore(v.Data, 71)
    case Case072: return payloadScore(v.Data, 72)
    case Case073: return payloadScore(v.Data, 73)
    case Case074: return payloadScore(v.Data, 74)
    case Case075: return payloadScore(v.Data, 75)
    case Case076: return payloadScore(v.Data, 76)
    case Case077: return payloadScore(v.Data, 77)
    case Case078: return payloadScore(v.Data, 78)
    case Case079: return payloadScore(v.Data, 79)
    case Case080: return payloadScore(v.Data, 80)
    case Case081: return payloadScore(v.Data, 81)
    case Case082: return payloadScore(v.Data, 82)
    case Case083: return payloadScore(v.Data, 83)
    case Case084: return payloadScore(v.Data, 84)
    case Case085: return payloadScore(v.Data, 85)
    case Case086: return payloadScore(v.Data, 86)
    case Case087: return payloadScore(v.Data, 87)
    case Case088: return payloadScore(v.Data, 88)
    case Case089: return payloadScore(v.Data, 89)
    case Case090: return payloadScore(v.Data, 90)
    case Case091: return payloadScore(v.Data, 91)
    case Case092: return payloadScore(v.Data, 92)
    case Case093: return payloadScore(v.Data, 93)
    case Case094: return payloadScore(v.Data, 94)
    case Case095: return payloadScore(v.Data, 95)
    case Case096: return payloadScore(v.Data, 96)
    case Case097: return payloadScore(v.Data, 97)
    case Case098: return payloadScore(v.Data, 98)
    case Case099: return payloadScore(v.Data, 99)
};panic("Unknown case")}
func payloadScore(d Payload,index int)int64{
    sum:=int64(index+d.Id+len(d.Label));for _,v:=range d.Values{sum+=int64(v)};for k,v:=range d.Tags{sum+=int64(len(k)+len(v))};if d.Next!=nil{sum+=score(d.Next)};return sum
}
type WirePayload struct{Id int `json:"id"`;Label string `json:"label"`;Values []int `json:"values"`;Tags map[string]string `json:"tags"`;Next json.RawMessage `json:"next"`}
type WireCase struct{Type string `json:"$type"`;Data WirePayload `json:"data"`}
func encode(p Payment)(json.RawMessage,error){
    if p==nil{return json.RawMessage("null"),nil};var name string;var data Payload
    switch v:=p.(type){
    case Case000: name, data = "Case000", v.Data
    case Case001: name, data = "Case001", v.Data
    case Case002: name, data = "Case002", v.Data
    case Case003: name, data = "Case003", v.Data
    case Case004: name, data = "Case004", v.Data
    case Case005: name, data = "Case005", v.Data
    case Case006: name, data = "Case006", v.Data
    case Case007: name, data = "Case007", v.Data
    case Case008: name, data = "Case008", v.Data
    case Case009: name, data = "Case009", v.Data
    case Case010: name, data = "Case010", v.Data
    case Case011: name, data = "Case011", v.Data
    case Case012: name, data = "Case012", v.Data
    case Case013: name, data = "Case013", v.Data
    case Case014: name, data = "Case014", v.Data
    case Case015: name, data = "Case015", v.Data
    case Case016: name, data = "Case016", v.Data
    case Case017: name, data = "Case017", v.Data
    case Case018: name, data = "Case018", v.Data
    case Case019: name, data = "Case019", v.Data
    case Case020: name, data = "Case020", v.Data
    case Case021: name, data = "Case021", v.Data
    case Case022: name, data = "Case022", v.Data
    case Case023: name, data = "Case023", v.Data
    case Case024: name, data = "Case024", v.Data
    case Case025: name, data = "Case025", v.Data
    case Case026: name, data = "Case026", v.Data
    case Case027: name, data = "Case027", v.Data
    case Case028: name, data = "Case028", v.Data
    case Case029: name, data = "Case029", v.Data
    case Case030: name, data = "Case030", v.Data
    case Case031: name, data = "Case031", v.Data
    case Case032: name, data = "Case032", v.Data
    case Case033: name, data = "Case033", v.Data
    case Case034: name, data = "Case034", v.Data
    case Case035: name, data = "Case035", v.Data
    case Case036: name, data = "Case036", v.Data
    case Case037: name, data = "Case037", v.Data
    case Case038: name, data = "Case038", v.Data
    case Case039: name, data = "Case039", v.Data
    case Case040: name, data = "Case040", v.Data
    case Case041: name, data = "Case041", v.Data
    case Case042: name, data = "Case042", v.Data
    case Case043: name, data = "Case043", v.Data
    case Case044: name, data = "Case044", v.Data
    case Case045: name, data = "Case045", v.Data
    case Case046: name, data = "Case046", v.Data
    case Case047: name, data = "Case047", v.Data
    case Case048: name, data = "Case048", v.Data
    case Case049: name, data = "Case049", v.Data
    case Case050: name, data = "Case050", v.Data
    case Case051: name, data = "Case051", v.Data
    case Case052: name, data = "Case052", v.Data
    case Case053: name, data = "Case053", v.Data
    case Case054: name, data = "Case054", v.Data
    case Case055: name, data = "Case055", v.Data
    case Case056: name, data = "Case056", v.Data
    case Case057: name, data = "Case057", v.Data
    case Case058: name, data = "Case058", v.Data
    case Case059: name, data = "Case059", v.Data
    case Case060: name, data = "Case060", v.Data
    case Case061: name, data = "Case061", v.Data
    case Case062: name, data = "Case062", v.Data
    case Case063: name, data = "Case063", v.Data
    case Case064: name, data = "Case064", v.Data
    case Case065: name, data = "Case065", v.Data
    case Case066: name, data = "Case066", v.Data
    case Case067: name, data = "Case067", v.Data
    case Case068: name, data = "Case068", v.Data
    case Case069: name, data = "Case069", v.Data
    case Case070: name, data = "Case070", v.Data
    case Case071: name, data = "Case071", v.Data
    case Case072: name, data = "Case072", v.Data
    case Case073: name, data = "Case073", v.Data
    case Case074: name, data = "Case074", v.Data
    case Case075: name, data = "Case075", v.Data
    case Case076: name, data = "Case076", v.Data
    case Case077: name, data = "Case077", v.Data
    case Case078: name, data = "Case078", v.Data
    case Case079: name, data = "Case079", v.Data
    case Case080: name, data = "Case080", v.Data
    case Case081: name, data = "Case081", v.Data
    case Case082: name, data = "Case082", v.Data
    case Case083: name, data = "Case083", v.Data
    case Case084: name, data = "Case084", v.Data
    case Case085: name, data = "Case085", v.Data
    case Case086: name, data = "Case086", v.Data
    case Case087: name, data = "Case087", v.Data
    case Case088: name, data = "Case088", v.Data
    case Case089: name, data = "Case089", v.Data
    case Case090: name, data = "Case090", v.Data
    case Case091: name, data = "Case091", v.Data
    case Case092: name, data = "Case092", v.Data
    case Case093: name, data = "Case093", v.Data
    case Case094: name, data = "Case094", v.Data
    case Case095: name, data = "Case095", v.Data
    case Case096: name, data = "Case096", v.Data
    case Case097: name, data = "Case097", v.Data
    case Case098: name, data = "Case098", v.Data
    case Case099: name, data = "Case099", v.Data
    default:return nil,fmt.Errorf("Unknown case")}
    next,err:=encode(data.Next);if err!=nil{return nil,err}
    return json.Marshal(WireCase{name,WirePayload{data.Id,data.Label,data.Values,data.Tags,next}})
}
func decode(raw json.RawMessage)(Payment,error){
    if bytes.Equal(raw,[]byte("null")){return nil,nil};var wire WireCase;if err:=json.Unmarshal(raw,&wire);err!=nil{return nil,err}
    next,err:=decode(wire.Data.Next);if err!=nil{return nil,err};d:=wire.Data;data:=Payload{d.Id,d.Label,d.Values,d.Tags,next}
    switch wire.Type{
    case "Case000": return Case000{data}, nil
    case "Case001": return Case001{data}, nil
    case "Case002": return Case002{data}, nil
    case "Case003": return Case003{data}, nil
    case "Case004": return Case004{data}, nil
    case "Case005": return Case005{data}, nil
    case "Case006": return Case006{data}, nil
    case "Case007": return Case007{data}, nil
    case "Case008": return Case008{data}, nil
    case "Case009": return Case009{data}, nil
    case "Case010": return Case010{data}, nil
    case "Case011": return Case011{data}, nil
    case "Case012": return Case012{data}, nil
    case "Case013": return Case013{data}, nil
    case "Case014": return Case014{data}, nil
    case "Case015": return Case015{data}, nil
    case "Case016": return Case016{data}, nil
    case "Case017": return Case017{data}, nil
    case "Case018": return Case018{data}, nil
    case "Case019": return Case019{data}, nil
    case "Case020": return Case020{data}, nil
    case "Case021": return Case021{data}, nil
    case "Case022": return Case022{data}, nil
    case "Case023": return Case023{data}, nil
    case "Case024": return Case024{data}, nil
    case "Case025": return Case025{data}, nil
    case "Case026": return Case026{data}, nil
    case "Case027": return Case027{data}, nil
    case "Case028": return Case028{data}, nil
    case "Case029": return Case029{data}, nil
    case "Case030": return Case030{data}, nil
    case "Case031": return Case031{data}, nil
    case "Case032": return Case032{data}, nil
    case "Case033": return Case033{data}, nil
    case "Case034": return Case034{data}, nil
    case "Case035": return Case035{data}, nil
    case "Case036": return Case036{data}, nil
    case "Case037": return Case037{data}, nil
    case "Case038": return Case038{data}, nil
    case "Case039": return Case039{data}, nil
    case "Case040": return Case040{data}, nil
    case "Case041": return Case041{data}, nil
    case "Case042": return Case042{data}, nil
    case "Case043": return Case043{data}, nil
    case "Case044": return Case044{data}, nil
    case "Case045": return Case045{data}, nil
    case "Case046": return Case046{data}, nil
    case "Case047": return Case047{data}, nil
    case "Case048": return Case048{data}, nil
    case "Case049": return Case049{data}, nil
    case "Case050": return Case050{data}, nil
    case "Case051": return Case051{data}, nil
    case "Case052": return Case052{data}, nil
    case "Case053": return Case053{data}, nil
    case "Case054": return Case054{data}, nil
    case "Case055": return Case055{data}, nil
    case "Case056": return Case056{data}, nil
    case "Case057": return Case057{data}, nil
    case "Case058": return Case058{data}, nil
    case "Case059": return Case059{data}, nil
    case "Case060": return Case060{data}, nil
    case "Case061": return Case061{data}, nil
    case "Case062": return Case062{data}, nil
    case "Case063": return Case063{data}, nil
    case "Case064": return Case064{data}, nil
    case "Case065": return Case065{data}, nil
    case "Case066": return Case066{data}, nil
    case "Case067": return Case067{data}, nil
    case "Case068": return Case068{data}, nil
    case "Case069": return Case069{data}, nil
    case "Case070": return Case070{data}, nil
    case "Case071": return Case071{data}, nil
    case "Case072": return Case072{data}, nil
    case "Case073": return Case073{data}, nil
    case "Case074": return Case074{data}, nil
    case "Case075": return Case075{data}, nil
    case "Case076": return Case076{data}, nil
    case "Case077": return Case077{data}, nil
    case "Case078": return Case078{data}, nil
    case "Case079": return Case079{data}, nil
    case "Case080": return Case080{data}, nil
    case "Case081": return Case081{data}, nil
    case "Case082": return Case082{data}, nil
    case "Case083": return Case083{data}, nil
    case "Case084": return Case084{data}, nil
    case "Case085": return Case085{data}, nil
    case "Case086": return Case086{data}, nil
    case "Case087": return Case087{data}, nil
    case "Case088": return Case088{data}, nil
    case "Case089": return Case089{data}, nil
    case "Case090": return Case090{data}, nil
    case "Case091": return Case091{data}, nil
    case "Case092": return Case092{data}, nil
    case "Case093": return Case093{data}, nil
    case "Case094": return Case094{data}, nil
    case "Case095": return Case095{data}, nil
    case "Case096": return Case096{data}, nil
    case "Case097": return Case097{data}, nil
    case "Case098": return Case098{data}, nil
    case "Case099": return Case099{data}, nil
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
