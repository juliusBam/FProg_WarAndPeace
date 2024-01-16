namespace WarAndPeaceFProg

module ChapterAnalysis =
    let curriedFindWordMatch warTerms peaceTerms word =
        if (warTerms |> Set.contains word) then 1
        else if (peaceTerms |> Set.contains word) then -1
        else 0
    
    let analyseChapterContent warTerms peaceTerms chapterContent =
        let applyFindWordMatch = curriedFindWordMatch warTerms peaceTerms
        chapterContent |> List.map applyFindWordMatch |> List.sum
        
    let indexWithFilter (termsToMatch: Set<string>)  (chapterContent: string list) = 
        chapterContent 
        |> List.indexed
        |> List.filter (
            fun (_: int, value: string) -> termsToMatch |> Set.contains value)
        |> List.map fst // fst returns first element of (index, value) tuple
    
    let calculateDistance (indexList: int List) = 
        indexList 
        |> List.pairwise
        |> List.map (fun (first: int, second: int) -> second - first)
    let calculateDensity (distanceList: int list) =
         //73 differences to the reference
         let distanceSum = double (distanceList |> List.sum)
         if (distanceSum = 0) then double 0 else double (distanceList.Length + 1) / distanceSum
         //85 differences to the reference
         //double (distanceList |> List.sum) / double (distanceList.Length + 1)
         //84 differences to the reference
         //double (distanceList.Length + 1) / double (distanceList |> List.sum)
         //84 differences to the reference
         //if distanceList.IsEmpty then double 0 else distanceList |> List.averageBy double
        
    let parseChapterReturnTopic (warTerms: string Set) (peaceTerms: string Set) (acc: int list) (chapterContent: string list) =
        if (calculateDensity (calculateDistance (indexWithFilter peaceTerms chapterContent)) >
                calculateDensity (calculateDistance (indexWithFilter warTerms chapterContent))) then -1 :: acc //faster then adding at the end of the list --> have to revert
        else 1 :: acc
    
    let calculateChapterArgument (formattedBookContent: string list list) (warTerms: string Set) (peaceTerms: string Set) =
        let curriedParseChapterReturnTopic = parseChapterReturnTopic warTerms peaceTerms
        List.fold curriedParseChapterReturnTopic [] formattedBookContent |> List.rev //we use list rev because it is faster to append the elements at the beginning of the list
        //and then invert it, List.fold is already parallel
       