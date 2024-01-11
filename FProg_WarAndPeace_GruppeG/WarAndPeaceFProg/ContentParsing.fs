namespace WarAndPeaceFProg

module ContentParsing =
    //separate into chapters
    let isLineNewChapter (actualLine: string) =
        let lineParts = actualLine.Split " "
        lineParts.Length = 2 && lineParts[0].ToLower().Equals "chapter"
        
        
    //todo separate in chars
    let separateIntoChapters (sequence: seq<string>) =
             let chaptersSeq: seq<seq<string>> = []
             let a = sequence |> Seq.takeWhile (fun line -> not (isLineNewChapter line)) |> Seq.skip 1
             chaptersSeq