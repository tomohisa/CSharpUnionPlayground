import Foundation
if CommandLine.arguments.contains("--domain-test"){domainTests()}else if CommandLine.arguments.contains("--self-test"){libraryTests();print("{\"library\":\"PASS\"}")}else{
struct Input:Codable {let history:[Recorded<Event>];let operations:[Operation]}
do{
    let input=try JSONDecoder().decode(Input.self,from:FileHandle.standardInput.readDataToEndOfFile())
    print(String(decoding:try JSONEncoder().encode(runSchool(input.history,input.operations)),as:UTF8.self))
}catch{FileHandle.standardError.write(Data((String(describing:error)+"\n").utf8));exit(1)}

}
