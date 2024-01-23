namespace WarAndPeaceFProg

open System.Text.RegularExpressions

module FileOperations =
    open System.IO
    
    // let sanitizeLine (line: string) =
    //     let pattern: string = "[^a-zA-Z0-9\s]"
    //     Regex(pattern).Replace(line, "") 
    //use sequences in order to avoid loading all values with lazy loading
    let readFileContent (fileName: string) =
        seq {
            use reader = new StreamReader(fileName)
            while not reader.EndOfStream do
                //let cleanLine = reader.ReadLine() |> sanitizeLine
                let cleanLine = reader.ReadLine()
                if cleanLine <> "" then
                    yield cleanLine
        }

    let truncateUntilMatch (sequence: seq<string>) (specificString: string) =
        sequence
            |> Seq.skipWhile (fun line -> line <> specificString) //skip elements which fulfill the predicate

    let truncateAfterMatch (sequence: seq<string>) (specificString: string) =
        sequence
            |> Seq.takeWhile (fun line -> line <> specificString) //we take everything that is NOT the last line
            |> Seq.skip 1 //we remove the string we matched against
            
    let extractFileContent (bookStartFlag: string) (bookEndFlag: string) = 
        let content = readFileContent "../../../book.txt"
        truncateAfterMatch (truncateUntilMatch content bookStartFlag) bookEndFlag