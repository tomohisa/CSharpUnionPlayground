import Foundation
if CommandLine.arguments.count==3 && CommandLine.arguments[1]=="--projection-scale"{projectionTests(Int(CommandLine.arguments[2])!)}else if CommandLine.arguments.contains("--domain-test"){domainTests()}else if CommandLine.arguments.contains("--self-test"){libraryTests();print("{\"library\":\"PASS\"}")}else{
struct Input:Codable {let history:[Recorded<Event>];let operations:[Operation]}
do{
    let input=try JSONDecoder().decode(Input.self,from:FileHandle.standardInput.readDataToEndOfFile())
    let output=runSchool(input.history,input.operations)
    print(String(decoding:try ProcessInfo.processInfo.environment["DECIDER_BENCHMARK_SUMMARY"]=="1" ? benchmarkOutput(output):JSONEncoder().encode(output),as:UTF8.self))
}catch{FileHandle.standardError.write(Data((String(describing:error)+"\n").utf8));exit(1)}

}
