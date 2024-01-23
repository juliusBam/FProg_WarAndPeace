open System.IO
open WarAndPeaceFProg.FileOperations
open WarAndPeaceFProg.ContentParsing
open WarAndPeaceFProg.ChapterAnalysis

let getBookContent =
    let bookStartFlag = "CHAPTER 1"
    let bookEndFlag = "*** END OF THE PROJECT GUTENBERG EBOOK, WAR AND PEACE ***"
    //let bookEndFlagSanitized = " END OF THE PROJECT GUTENBERG EBOOK, WAR AND PEACE "
    extractFormattedFileContent (extractFileContent bookStartFlag bookEndFlag)
    
let mapResults (chapterScores: int list) =
        chapterScores |> List.indexed |> List.map (fun el ->
                                                    let relationValue = snd el
                                                    match relationValue with
                                                    | 0 -> $"Chapter %A{fst el + 1}: no idea"
                                                    | relationValue when relationValue > 0 -> $"Chapter %A{fst el + 1}: war-related"
                                                    | relationValue when relationValue < 0 -> $"Chapter %A{fst el + 1}: peace-related"
                                                )

let writeStringsToFile (filePath: string) (content: seq<string>) =
    use writer = new StreamWriter(filePath, false) // false for overwriting existing content
    content |> Seq.iter (fun line -> writer.WriteLine(line))

[<EntryPoint>]
let main (args: string array) =
    let stopWatch = System.Diagnostics.Stopwatch.StartNew()
    let formattedBookContent = getBookContent
    let peaceTerms = Set.ofSeq (readFileContent "../../../peaceTerms.txt") //terms are saved in Sequence (like a Map but without values) for fast search operations O(log n) vs 0(n)
    let warTerms = Set.ofSeq (readFileContent "../../../warTerms.txt")
    stopWatch.Stop()
    printfn $"Parsing of files done in: %f{stopWatch.Elapsed.TotalMilliseconds} ms"
    stopWatch.Restart()
    let result = calculateChapterArgument formattedBookContent warTerms peaceTerms |> mapResults |> Seq.ofList
    stopWatch.Stop()
    printfn $"Chapter parsing done in: %f{stopWatch.Elapsed.TotalMilliseconds} ms"
    writeStringsToFile "../../../output.txt" result
    0