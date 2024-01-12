open System.IO
open WarAndPeaceFProg.FileOperations
open WarAndPeaceFProg.ContentParsing

let getBookContent =
    let bookStartFlag = "CHAPTER 1"
    let bookEndFlag = " END OF THE PROJECT GUTENBERG EBOOK WAR AND PEACE "
    extractFormattedFileContent (extractFileContent bookStartFlag bookEndFlag)

[<EntryPoint>]
let main (args: string array) =
    let formattedBookContent = getBookContent

    0