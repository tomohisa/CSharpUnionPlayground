import Foundation
extension Rejection:Error {}
struct StudentId:Equatable,Sendable {
    let value:String
    private init(restored value:String){self.value=value}
    static func fromEvent(_ value:String)->StudentId{StudentId(restored:value)}
    init(validating value:String)throws{guard !value.trimmingCharacters(in:.whitespacesAndNewlines).isEmpty else{throw Rejection.invalidId};self.value=value}
}
struct ClassId:Equatable,Sendable {
    let value:String
    private init(restored value:String){self.value=value}
    static func fromEvent(_ value:String)->ClassId{ClassId(restored:value)}
    init(validating value:String)throws{guard !value.trimmingCharacters(in:.whitespacesAndNewlines).isEmpty else{throw Rejection.invalidId};self.value=value}
}
struct SeatCapacity:Equatable,Sendable {
    let value:Int
    private init(restored value:Int){self.value=value}
    static func fromEvent(_ value:Int)->SeatCapacity{SeatCapacity(restored:value)}
    init(validating value:Int)throws{guard (1...10).contains(value) else{throw Rejection.invalidCapacity};self.value=value}
}
struct AssessmentScore:Equatable,Sendable {
    let value:Int
    private init(restored value:Int){self.value=value}
    static func fromEvent(_ value:Int)->AssessmentScore{AssessmentScore(restored:value)}
    init(validating value:Int)throws{guard (0...100).contains(value) else{throw Rejection.invalidScore};self.value=value}
}
struct SchoolDate:Equatable,Sendable {
    let value:String
    private init(restored value:String){self.value=value}
    static func fromEvent(_ value:String)->SchoolDate{SchoolDate(restored:value)}
    init(validating value:String)throws{guard Self.validDate(value) else{throw Rejection.invalidDate};self.value=value}
    private static func validDate(_ value:String)->Bool{
        let b=Array(value.utf8)
        guard b.count==10,b[4]==45,b[7]==45,b.enumerated().allSatisfy({$0.offset==4 || $0.offset==7 || (48...57).contains($0.element)}),
              let y=Int(String(decoding:b[0..<4],as:UTF8.self)),let m=Int(String(decoding:b[5..<7],as:UTF8.self)),let d=Int(String(decoding:b[8..<10],as:UTF8.self)),y>0,(1...12).contains(m),d>0 else{return false}
        let leap=y%4==0 && (y%100 != 0 || y%400==0)
        return d <= [31,leap ? 29:28,31,30,31,30,31,31,30,31,30,31][m-1]
    }
}
struct ValidatedCommand {
    let value:Command
    private init(_ value:Command){self.value=value}
    static func create(_ command:Command)throws->ValidatedCommand{
        switch command{
        case .registerStudent(let id,_),.promoteStudent(let id,_):_ = try StudentId(validating:id)
        case .openClass(let id,_,let capacity):_ = try ClassId(validating:id);_ = try SeatCapacity(validating:capacity)
        case .enroll(let c,let s),.drop(let c,let s),.attend(let c,let s):_ = try ClassId(validating:c);_ = try StudentId(validating:s)
        case .startClass(let c,let date):_ = try ClassId(validating:c);_ = try SchoolDate(validating:date)
        case .assess(let c,let s,let score):_ = try ClassId(validating:c);_ = try StudentId(validating:s);_ = try AssessmentScore(validating:score)
        case .completeClass(let c):_ = try ClassId(validating:c)
        }
        return ValidatedCommand(command)
    }
    static func validate(_ command:Command)->[Rejection]{
        do{_ = try create(command);return []}catch let error as Rejection{return [error]}catch{preconditionFailure("unexpected validator error")}
    }
}
