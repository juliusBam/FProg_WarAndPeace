open System.IO
open WarAndPeaceFProg.FileOperations
open WarAndPeaceFProg.ContentParsing
open WarAndPeaceFProg.ChapterAnalysis

let getBookContent (filePath: string) =
    let bookStartFlag = "CHAPTER 1"
    let bookEndFlag = "*** END OF THE PROJECT GUTENBERG EBOOK, WAR AND PEACE ***"
    let rawBookContent = extractFileContent filePath bookStartFlag bookEndFlag
    extractFormattedFileContent rawBookContent

[<EntryPoint>]
let main (args: string array) =
    let bookPath = System.Environment.GetEnvironmentVariable("BOOK_PATH")
    let peaceTermsPath = System.Environment.GetEnvironmentVariable("PEACE_TERMS")
    let warTermsPath = System.Environment.GetEnvironmentVariable("WAR_TERMS")
    let outputFilePath = System.Environment.GetEnvironmentVariable("OUTPUT_FILE")

    let stopWatch = System.Diagnostics.Stopwatch.StartNew()

    let formattedBookContent = getBookContent bookPath
    let peaceTerms = Set.ofSeq (readFileContent peaceTermsPath) //terms are saved in Sequence (like a Map but without values) for fast search operations O(log n) vs 0(n)
    let warTerms = Set.ofSeq (readFileContent warTermsPath)
    stopWatch.Stop()
    printfn $"Parsing of files done in: %f{stopWatch.Elapsed.TotalMilliseconds} ms"
    stopWatch.Restart()
    let result = calculateChapterArgument formattedBookContent warTerms peaceTerms |> formatChapterTopicResults |> Seq.ofList
    stopWatch.Stop()
    printfn $"Chapter parsing done in: %f{stopWatch.Elapsed.TotalMilliseconds} ms"
    writeStringsToFile outputFilePath result
    0