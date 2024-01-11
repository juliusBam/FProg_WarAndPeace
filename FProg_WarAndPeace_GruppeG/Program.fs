open System.IO
open WarAndPeaceFProg.FileOperations
open WarAndPeaceFProg.ContentParsing

[<EntryPoint>]
let main (args: string array) =
    let bookStartFlag = "CHAPTER 1"
    let bookEndFlag = " END OF THE PROJECT GUTENBERG EBOOK WAR AND PEACE "
    let formattedFileContent = extractFormattedFileContent (extractFileContent bookStartFlag bookEndFlag)
    printfn $"%A{formattedFileContent}"
    printfn $"%A{formattedFileContent |> Seq.last}"
    0