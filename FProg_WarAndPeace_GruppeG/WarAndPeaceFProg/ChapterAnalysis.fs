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
    
    //in combination with the 55 differences density returns 48 differences to the reference
    //scales the distance to increase the weight of relevant distances
    //not used atm leads to worse results
    let scaleDistance (first: int) (second: int) =
        let diff = second - first
        match diff with
        | diff when diff < 100 -> int (diff / 2)
        | diff when diff < 400 -> diff * 2
        | _ -> diff
        
    let calculateDifference (first: int) (second: int) =
        let diff = second - first
        match diff with
        | diff when diff < 0 -> 0
        | _ -> diff
    
    let calculateDistance (indexList: int List) =
        if (indexList.Length = 1 || indexList.Length = 0) then
            [] //!important if list only has 1 element pairwise.map returns null
        else indexList |> List.pairwise |> List.map (fun (first: int, second: int) -> calculateDifference first second)

    let filterDistances (distanceList: int list) (windowSize: int) =
        distanceList |> List.filter (fun (value: int) ->  value < windowSize)
    
    let calculateDensityAvgRelativeDistance (distanceList: int list) (chapterLength: int) =
        //25 without sanitize line, with compare densities and with punctuation
        //57 with sanitize line, with compare densities and without punctuation
        if (distanceList.IsEmpty || chapterLength = 0) then double 0
        else
            double distanceList.Length / ((distanceList |> List.averageBy double) / double chapterLength)
    
    let calculateDensityNumberOfMatches (distanceList: int list) (chapterLength: int) =
        //15 without sanitize line, with compare densities and with punctuation
        //39 with sanitize line, with compare densities and without punctuation
        if (distanceList.IsEmpty || chapterLength = 0) then double 0
        else
            distanceList.Length
            
                
    let calculateDensityFilteredDistances (distanceList: int list) (chapterLength: int) =
        if (distanceList.IsEmpty || chapterLength = 0) then double 0
        else
            let size = 999999999
            let filteredDistance = filterDistances distanceList size
            if (filteredDistance.Length = 0) then double 0
            else double filteredDistance.Length / ((filteredDistance |> List.averageBy double) / double chapterLength)
        
    //if true war, else peace
    let compareDensities (warDensity: double) (warDistance: int) (peaceDensity: double) (peaceDistance: int) =
        if ((warDistance = 0 || warDistance = 1) && peaceDistance > 1) then
            false
        else if (warDistance > 1 && (peaceDistance = 0 || peaceDistance = 1)) then
            true
        else
            warDensity > peaceDensity     
            
    //invert order, there is a chapter with 0 war and 0 peace --> considered as peace
    let parseChapterReturnTopic (warTerms: string Set) (peaceTerms: string Set) (acc: int list) (chapterContent: string list) =
        
        let warIndexFilter = indexWithFilter warTerms chapterContent
        let warDistance = calculateDistance warIndexFilter
        let warDensity = calculateDensityAvgRelativeDistance warDistance chapterContent.Length
        
        let peaceIndexFilter = indexWithFilter peaceTerms chapterContent
        let peaceDistance = calculateDistance peaceIndexFilter
        let peaceDensity = calculateDensityAvgRelativeDistance peaceDistance chapterContent.Length
      
        if compareDensities warDensity (warDistance |> List.length) peaceDensity (peaceDistance |> List.length) then
            1 :: acc //faster then adding at the end of the list --> have to revert
        else
            -1 :: acc
       
    
    let calculateChapterArgument (formattedBookContent: string list list) (warTerms: string Set) (peaceTerms: string Set) =
        let curriedParseChapterReturnTopic = parseChapterReturnTopic warTerms peaceTerms
        List.fold curriedParseChapterReturnTopic [] formattedBookContent |> List.rev //we use list rev because it is faster to append the elements at the beginning of the list
        //and then invert it, List.fold is already parallel
       