module FProg_WarAndPeace_GruppeGTests.ContentParsingTests

open FsCheck.FSharp
open NUnit.Framework
open System
open WarAndPeaceFProg.ContentParsing
open WarAndPeaceFProg.FileOperations

let generateRandomLine () =
    let rnd = Random()
    let length = rnd.Next(40, 200)
    let alphanumericChars = ['a'..'z'] @ ['A'..'Z'] @ ['0'..'9']
    let getChar () =
        if (rnd.Next(1, 8) < 2) then //force adding spaces to simulate words
            ' '
        else
            alphanumericChars.[rnd.Next(alphanumericChars.Length)]
    let randomString = List.init length (fun _ -> getChar ()) |> String.Concat
    randomString

//hardcoded words inserted for properties
let firstWord = "FIRST"
let secondWord = "WORDS"
let truncateAfterMatchString = "*MYMATCH*"

let generateListOfChapterContent =
    Gen.choose(30, 50)
    |> Gen.bind (fun lineNumb ->
        Gen.elements [1..lineNumb]
        |> Gen.map (fun i -> //First words are hardcoded to test if order is right, truncateaftermatchstring as well, so that we have a unique string (not only alphanumeric)
            [String.Concat(firstWord, " ", secondWord)] @ List.init i (fun _ -> generateRandomLine()) @ [truncateAfterMatchString] @ List.init i (fun _ -> generateRandomLine()) 
        )
    )


//Hard to do with property based testing --> would have to store results
[<Test>]
let chapterSplitting () =
    
    let firstLineChapter1 = "first line chapter 1"
    let firstLineChapter2 = "first line chapter 2"
    
    let bookContent = ["CHAPTER 1"; firstLineChapter1; "second line chapter 1"; "CHAPTER 2"; firstLineChapter2; "second line chapter 2"]
    
    let result = partitionContentByChapter bookContent
    
    //book content not yet reverted
    Assert.AreEqual(firstLineChapter1, result[0][0])
    Assert.AreEqual(firstLineChapter2, result[1][0])
    
[<Test>]
let lineOnlyChapterNoChapter () =
    let isChapter = isLineNewChapter "Chapter"
    Assert.False(isChapter)
    
[<Test>]
let validLineIsChapter () =
    let isChapter = isLineNewChapter "CHAPTER 2"
    Assert.True(isChapter)

//Test chapter lines correctly split
[<Test>]
let splitChapterIntoWords () =
    let chapterContent = ["This is a line, of words"; "This is 89 bla: line, of words"]
    let result = splitSingleLine chapterContent
    
    //Test words are present
    Assert.AreEqual("This", result[0])
    Assert.AreEqual("line,", result[3])
    Assert.AreEqual("89", result[8])
    Assert.AreEqual("bla:", result[9])
    //Test length is right
    Assert.AreEqual(13, result.Length)

let areEmptySpacesRemoved (chapterContent: string list) =
    chapterContent
    |> List.forall
           (fun s ->
                not (s.Contains ' ')
           )

let areFirstWordsFirstAndWords (chapterContent: string list) =
    chapterContent[0] = "FIRST" && chapterContent[1] = "WORDS"

//Chapter words not containing empty spaces and size remaining the same
//As well as order not changed
[<Test>]
let testSplittingOfGeneratedWords () =
    let numbOfChapters = 5
    let splittedChapters = splitLinesIntoWords ((Gen.sample numbOfChapters generateListOfChapterContent) |> Array.toList)
    let noEmptySpaces = List.forall areEmptySpacesRemoved splittedChapters
    let correctFirstWords = List.forall areFirstWordsFirstAndWords splittedChapters
    
    Assert.True(noEmptySpaces)
    Assert.True(correctFirstWords)
    Assert.AreEqual(numbOfChapters, splittedChapters.Length)

let truncateUntilMatchIncludesMatchProperty (random: Random) (chapterContent: string list) =
    if (not chapterContent.IsEmpty) then
        let indexOfMatch = random.Next(0, chapterContent.Length - 1)
        let valueOfMatch = chapterContent[indexOfMatch]
        let truncatedList = truncateUntilMatch (chapterContent |> List.toSeq) valueOfMatch |> Seq.toList
        truncatedList[0] = valueOfMatch
    else
        true
        
//avoid instantiating random for every try
let partiallyAppliedTruncateUntilMatchIncludesMatchProperty (chapterContent: string list) =
    let random = Random()
    truncateUntilMatchIncludesMatchProperty random chapterContent

//Usage of truncate untilMatch does not remove the match from the list
[<Test>]
let testTruncateUntilMatchIncludesMatch () =
    let numbOfChapters = 5
    let splittedChapters = splitLinesIntoWords ((Gen.sample numbOfChapters generateListOfChapterContent) |> Array.toList)
    let matchIncluded = splittedChapters |> List.forall partiallyAppliedTruncateUntilMatchIncludesMatchProperty
    
    Assert.True(matchIncluded)

let truncateAfterMatchDoesNotIncludeMatchProperty (chapterContent: string list) =
    if (not chapterContent.IsEmpty) then
        let truncatedList = truncateAfterMatch (chapterContent |> List.toSeq) truncateAfterMatchString
        not (truncatedList |> Seq.contains (truncateAfterMatchString))
    else
        true

//Usage of truncate afterMatch removes the match from the list
[<Test>]
let testTruncateAfterMatchNoIncludesMatch () =
    let numbOfChapters = 5
    let splittedChapters = splitLinesIntoWords ((Gen.sample numbOfChapters generateListOfChapterContent) |> Array.toList)
    let matchNotPresent = splittedChapters |> List.forall truncateAfterMatchDoesNotIncludeMatchProperty 
    
    Assert.True(matchNotPresent)