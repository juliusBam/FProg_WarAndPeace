open System.IO
open WarAndPeaceFProg.FileOperations
open WarAndPeaceFProg.ContentParsing
open WarAndPeaceFProg.ChapterAnalysis

let getBookContent =
    let bookStartFlag = "CHAPTER 1"
    let bookEndFlag = " END OF THE PROJECT GUTENBERG EBOOK WAR AND PEACE "
    extractFormattedFileContent (extractFileContent bookStartFlag bookEndFlag)
    
let printResult (chapterScores: int list) =
        chapterScores |> List.indexed |> List.iter (fun el ->
                                                    let relationValue = snd el
                                                    match relationValue with
                                                    | 0 -> printfn $"Chapter %A{fst el + 1}: no idea"
                                                    | relationValue when relationValue > 0 -> printfn $"Chapter %A{fst el + 1}: war-related"
                                                    | relationValue when relationValue < 0 -> printfn $"Chapter %A{fst el + 1}: peace-related"
                                                )

[<EntryPoint>]
let main (args: string array) =
    let formattedBookContent = getBookContent
    let peaceTerms = Set.ofSeq (readFileContent "../../../peaceTerms.txt") //terms are saved in Sequence (like a Map but without values) for fast search operations O(log n) vs 0(n)
    let warTerms = Set.ofSeq (readFileContent "../../../warTerms.txt")
    calculateChapterArgument formattedBookContent warTerms peaceTerms |> printResult
    0