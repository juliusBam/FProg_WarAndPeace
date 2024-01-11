open System.IO
open WarAndPeaceFProg.FileOperations
open WarAndPeaceFProg.ContentParsing

[<EntryPoint>]
let main (args: string array) =
    let content = readFileContent ("../../../test.txt" |> Path.GetFullPath)
    let bookStartFlag = "CHAPTER 1"
    let bookEndFlag = " END OF THE PROJECT GUTENBERG EBOOK WAR AND PEACE "
    let filteredContent = truncateAfterMatch (truncateUntilMatch content bookStartFlag) bookEndFlag
    let splitContent = partitionContentByChapter filteredContent
    let splitWordsContent = splitLines splitContent
    printfn $"%A{filteredContent}"
    printfn $"%A{filteredContent |> Seq.last}"
    0