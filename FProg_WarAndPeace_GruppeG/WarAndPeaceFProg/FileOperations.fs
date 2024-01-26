namespace WarAndPeaceFProg

module FileOperations =
    open System.IO
    
    //Line sanitization leads to worse results
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

    //Used to remove everything from the file until specific string is found --> E.g. Chapter 1, the match is
    //included in the output
    let truncateUntilMatch (sequence: seq<string>) (specificString: string) =
        sequence
            |> Seq.skipWhile (fun line -> line <> specificString) //skip elements which fulfill the predicate

    //Used to remove everything afterwards a specific string is found --> E.g. used to remove the bottom of the
    //book, match is removed from the output as well
    let truncateAfterMatch (sequence: seq<string>) (specificString: string) =
        sequence
            |> Seq.takeWhile (fun line -> line <> specificString) //we take everything that is NOT the last line
            |> Seq.skip 1 //we remove the string we matched against
    
    //combines readFileContent with truncate to extract only the book content (padding text removed)
    let extractFileContent (filePath: string) (bookStartFlag: string) (bookEndFlag: string) = 
        let content = readFileContent filePath
        truncateAfterMatch (truncateUntilMatch content bookStartFlag) bookEndFlag
        
    let writeStringsToFile (filePath: string) (content: seq<string>) =
        use writer = new StreamWriter(filePath, false) // false for overwriting existing content
        content |> Seq.iter writer.WriteLine