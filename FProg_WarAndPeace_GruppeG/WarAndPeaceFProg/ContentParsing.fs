namespace WarAndPeaceFProg

module ContentParsing =
    //separate into chapters
    let isLineNewChapter (actualLine: string) =
        let lineParts = actualLine.Split " "
        lineParts.Length = 2 && lineParts[0].ToLower().Equals "chapter"
        
    let splitByChapter (accumulationState: string list list) (line: string) =
        if isLineNewChapter line then
            [] :: accumulationState //for a new chapter we insert an empty list
        else
            match accumulationState with
            | [] -> [[line]] //if actual accumulationState is empty just add the new string
            | currentChapter :: last -> (line :: currentChapter) :: last //append the current line to the last element of the chapter 2D list
            
    let partitionContentByChapter (content: string seq) =
        content |> Seq.fold splitByChapter [] |> List.map(fun element -> element |> List.rev) |> List.rev
            
    let splitSingleLine (chapterContent: string list) =
        //let wordSplitters = [|',';'.'; '-'; ' '; '''; '"'|]
        let wordSplitters = [|' '|]
        chapterContent |> List.map (fun line -> line.Split(wordSplitters) |> Array.toList) |> List.concat
            
    let splitLines (partitionedContent: string list list) =
        partitionedContent |> List.map splitSingleLine
        
    let extractFormattedFileContent (filteredContent: string seq) =
            let splitContent = partitionContentByChapter filteredContent
            splitLines splitContent
             
    