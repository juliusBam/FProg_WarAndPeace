namespace WarAndPeaceFProg

open System.Security.Cryptography

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
            | currentChapter :: last -> (line :: currentChapter) :: last
            //| [] -> [line.Split([|' '|]) |> Array.toList |> List.rev] //if actual accumulationState is empty just add the new string
            //| currentChapter :: rest -> line.Split([|' '|]) |> Array.toList |> List.rev @ currentChapter :: rest
            
    let splitSingleLine (chapterContent: string list) =
        chapterContent |> List.map (fun line -> line.Split(" ") |> Array.toList) |> List.concat
            
    let splitLines (partitionedContent: string list list) =
        partitionedContent |> List.map splitSingleLine
        
    let partitionContentByChapter (content: string seq) =
        content |> Seq.fold splitByChapter [] |> List.map(fun element -> element |> List.rev) |> List.rev
        
    // let result =
    //     inputSequence
    //     |> Seq.fold splitByChapter []
    //     |> List.rev

        
        
    // //todo separate in chars
    // let separateIntoChapters (sequence: seq<string>) =
    //          let chaptersSeq: seq<seq<string>> = []
    //          let a = sequence |> Seq.takeWhile (fun line -> not (isLineNewChapter line)) |> Seq.skip 1
    //          chaptersSeq
             
    