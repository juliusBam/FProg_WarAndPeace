module FProg_WarAndPeace_GruppeGTests.ChapterAnalysisTest

open System
open NUnit.Framework
open FsCheck.FSharp
open WarAndPeaceFProg.ChapterAnalysis

//no empty spaces to simulate single words
let generateRandomWord () =
    let rnd = Random()
    let length = rnd.Next(2, 12)
    let alphanumericChars = ['a'..'z'] @ ['A'..'Z'] @ ['0'..'9']
    let getChar () = alphanumericChars.[rnd.Next(alphanumericChars.Length)]
    let randomString = List.init length (fun _ -> getChar ()) |> String.Concat
    randomString

//takes a random "match" from the specified list
let takeRandomElFromList (list: string list) =
    let rnd = Random()
    let index = rnd.Next(0, list.Length) //rnd.Next upper bound is exclusive
    list[index]

//takes x random elements from the specified list
let takeFromList (list: string list) (numberOfTimes: int) =
    List.init numberOfTimes (fun _ -> takeRandomElFromList list)
    
//returns concatenation of matches for A, random words and matches for B
//no shuffling, no decent library found including it, implementation from
//scratch would not introduce desired randomness
let mixListsGenerator numberOfElementsA elementsA numberOfElementsB elementsB =
    Gen.choose (numberOfElementsA + numberOfElementsB + 1, 5 * (numberOfElementsA + numberOfElementsB) ) // Change the range as needed for the random number of words
    |> Gen.map (fun randomCount ->
        let words = List.init randomCount (fun _ -> generateRandomWord ())
        let elementsA = takeFromList elementsA numberOfElementsA
        let elementsB = takeFromList elementsB numberOfElementsB
        elementsA @ words @ elementsB 
    )

let numberOfMatchesProperty (matchesA: int) (listMatchesTopicA: string list) (matchesB: int) (listMatchesTopicB: string list) (words: string list) =
    let calculatedNumberOfMatchesA = (indexOfMatchesWithSet (Set.ofList listMatchesTopicA) words).Length
    let calculatedNumberOfMatchesB = (indexOfMatchesWithSet (Set.ofList listMatchesTopicB) words).Length
    calculatedNumberOfMatchesA = matchesA && calculatedNumberOfMatchesB = matchesB

//test that the counted number of matches are right
[<Test>]
let testNumberOfMatches () =
    
    let listMatchesTopicA = ["MATCHING-";"MAKES-";"FUN-"]
    
    let listMatchesTopicB = ["ICE-";"CREAM-";"GOOD-"]
    
    let matchesA = 5
    let matchesB = 10
    let partiallyAppliedGen = mixListsGenerator matchesA listMatchesTopicA matchesB listMatchesTopicB
    let text = Gen.sample 10 partiallyAppliedGen |> Array.toList
    
    let partiallyAppliedProperty = numberOfMatchesProperty matchesA listMatchesTopicA matchesB listMatchesTopicB
    let result = text |> List.forall partiallyAppliedProperty
    
    Assert.True(result)
    
let densityNumberOfMatchesProp (matchesA: int) (listMatchesTopicA: string list) (matchesB: int) (listMatchesTopicB: string list) (words: string list) =
    let calculatedMatchesA = (indexOfMatchesWithSet (Set.ofList listMatchesTopicA) words)
    let calculatedMatchesB = (indexOfMatchesWithSet (Set.ofList listMatchesTopicB) words)
    
    let calculatedDensityA = calculateDensityNumberOfMatches words.Length calculatedMatchesA
    let calculatedDensityB = calculateDensityNumberOfMatches words.Length calculatedMatchesB
    
    matchesA = (int)calculatedDensityA && matchesB = (int)calculatedDensityB 
    
let compareDensityCorrectProp (matchesA: int) (listMatchesTopicA: string list) (matchesB: int) (listMatchesTopicB: string list) (words: string list) =
    
    let calculatedMatchesA = (indexOfMatchesWithSet (Set.ofList listMatchesTopicA) words)
    let calculatedMatchesB = (indexOfMatchesWithSet (Set.ofList listMatchesTopicB) words)
    
    let comparedDensity = compareDensities words.Length calculatedMatchesA calculatedMatchesB calculateDensityNumberOfMatches
    
    matchesA > matchesB = comparedDensity

//test that the density as number of matches corresponds to the number of matches
//test that the recognized argument is right
[<Test>]
let densityNumberOfMatches () =
    
    let listMatchesTopicA = ["MATCHING-";"MAKES-";"FUN-"]
    let listMatchesTopicB = ["ICE-";"CREAM-";"GOOD-"]
    
    let matchesA = 20
    let matchesB = 5
    let partiallyAppliedGen = mixListsGenerator matchesA listMatchesTopicA matchesB listMatchesTopicB
    let text = Gen.sample 10 partiallyAppliedGen |> Array.toList
    
    let partiallyDensityNumberOfMatchesProp = densityNumberOfMatchesProp matchesA listMatchesTopicA matchesB listMatchesTopicB
    let resDensityValueCorrect = text |> List.forall partiallyDensityNumberOfMatchesProp
    
    Assert.True(resDensityValueCorrect)
    
    let partiallyCompareDensityCorrectProp = densityNumberOfMatchesProp matchesA listMatchesTopicA matchesB listMatchesTopicB
    let relatedTopicRes = text |> List.forall partiallyCompareDensityCorrectProp
    
    Assert.True(relatedTopicRes)