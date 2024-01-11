namespace WarAndPeaceFProg

open System.Text.RegularExpressions

// let readFileSeparateIntoChapters (fileName: string) =
//          use stream = new StreamReader(fileName)
//          // Continue reading while valid lines.
//          // let mutable chapterIndex = 0
//          // let mutable chaptersAndContent : List<string> = []
//          let mutable valid = true
//          let mutable chapterCount = 0
//          let mutable actualChapterContent: string = ""
//          let mutable chapters: List<string> = []
                 
         // while (valid) do
         //    let line = stream.ReadLine()
         //    if (line = null) then
         //          valid <- false
         //    else
         //        let extractedChap = extractChapter line
         //        if extractedChap.IsSome then
         //            chapters <- chapters @ [actualChapterContent]
         //            //printfn $"Found chapter with number %A{extractedChap.Value}"
         //            chapterCount <- chapterCount + 1
         //        else
         //            actualChapterContent <- actualChapterContent + line
         //        // else
         //        //     printfn $"%A{line}"
         //
         // printfn $"%A{chapterCount}"
         // chapters
         
         // while (valid) do
         //     let line = stream.ReadLine()
         //     if (line = null) then
         //         valid <- false
         //     else
         //         //do pattern matching to check if new chapter
         //         let extractedChap = extractChapter line
         //         if extractedChap.IsSome then
         //             chapterIndex <- extractedChap.Value
         //         else
         //             chaptersAndContent[chapterIndex] <- chaptersAndContent[chapterIndex]

module FileOperations =
    open System.IO
    
    let sanitizeLine (line: string) =
        let pattern: string = "[^a-zA-Z0-9\s]"
        Regex(pattern).Replace(line, "") 
    //use sequences in order to avoid loading all values with lazy loading
    let readFileContent (fileName: string) =
        seq {
            use reader = new StreamReader(fileName)
            while not reader.EndOfStream do
                let cleanLine = reader.ReadLine() |> sanitizeLine
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
        