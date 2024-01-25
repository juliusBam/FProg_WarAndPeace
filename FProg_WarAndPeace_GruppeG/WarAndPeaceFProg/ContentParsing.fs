namespace WarAndPeaceFProg

module ContentParsing =
    //separate into chapters
    let isLineNewChapter (actualLine: string) =
        let lineParts = actualLine.Split " "
        lineParts.Length = 2 && lineParts[0].ToLower().Equals "chapter"
    
    //iterates content to create a 2D list, where each entry is a chapter
    //inserts entries always at the beginning of array, because it is faster
    let splitByChapter (accumulationState: string list list) (line: string) =
        if isLineNewChapter line then
            [] :: accumulationState //for a new chapter we insert an empty list
        else
            match accumulationState with
            | [] -> [[line]] //if actual accumulationState is empty just add the new string
            | currentChapter :: last -> (line :: currentChapter) :: last //append the current line to the last element of the chapter 2D list
    
    //reverts the sequence of chapters as well as the lines in the chapter, in order to restore the
    //original sequence.
    let revertChapters (parsedChapters: string list list) =
        parsedChapters |> List.map(fun element -> element |> List.rev) |> List.rev
    
    let partitionContentByChapter (content: string seq) =
        content |> Seq.fold splitByChapter [] |> revertChapters
    
    //output is the list of words contained in the line
    let splitSingleLine (chapterContent: string list) =
        //let wordSplitters = [|',';'.'; '-'; ' '; '''; '"'|]
        let wordSplitters = [|' '|]
        chapterContent |> List.map (fun line -> line.Split(wordSplitters) |> Array.toList) |> List.concat
    
    //output is a 2D list where each entry is a chapter and each entry in the chapter is a word
    let splitLinesIntoWords (partitionedContent: string list list) =
        partitionedContent |> List.map splitSingleLine
    
    let extractFormattedFileContent (filteredContent: string seq) =
            let partitionedContent = partitionContentByChapter filteredContent
            splitLinesIntoWords partitionedContent //split every line in words
             
    