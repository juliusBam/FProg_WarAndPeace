namespace WarAndPeaceFProg

module ChapterAnalysis =
    //returns list containing the indexes of the words contained in the defined string set
    let indexOfMatchesWithSet (termsToMatch: Set<string>)  (chapterContent: string list) = 
        chapterContent
        |> List.indexed
        |> List.filter
               (fun (_: int, value: string) ->
                    termsToMatch |> Set.contains value
               )
        |> List.map fst  // fst returns first element of (index, value) tuple
        
    let calculateDifference (first: int) (second: int) =
        let diff = second - first
        match diff with
        | diff when diff < 0 -> 0
        | _ -> diff
    
    //calculate the distance between the matches -> used by the density
    let calculateDistance (indexList: int List) =
        if (indexList.Length = 1 || indexList.Length = 0) then
            [] //!important if list only has 1 element pairwise.map returns null
        else
            indexList
            |> List.pairwise
            |> List.map
                   (fun (first: int, second: int) ->
                        calculateDifference first second
                   )
    
    let calculateDensityAvgRelativeDistance (chapterLength: int) (matchesIndexedList: int list) =
        //25 without sanitize line, with compare densities and with punctuation
        //57 with sanitize line, with compare densities and without punctuation
        let distanceList = calculateDistance matchesIndexedList
        
        if (distanceList.IsEmpty || chapterLength = 0) then
            double 0
        else
            double distanceList.Length / ((distanceList |> List.averageBy double) / double chapterLength)
    
    let calculateDensityNumberOfMatches (chapterLength: int) (matchesIndexedList: int list) =
        //15 without sanitize line, with compare densities and with punctuation
        //39 with sanitize line, with compare densities and without punctuation
        if (matchesIndexedList.IsEmpty || chapterLength = 0) then
            double 0
        else
            matchesIndexedList.Length
           
    let filterDistances (distanceList: int list) (windowSize: int) =
        distanceList
        |> List.filter
               (fun (value: int) ->
                    value < windowSize
               )
              
    let calculateDensityFilteredDistances (chapterLength: int) (distanceList: int list) =
        if (distanceList.IsEmpty || chapterLength = 0) then double 0
        else
            let size = 999999999
            let filteredDistance = filterDistances distanceList size
            if (filteredDistance.Length = 0) then double 0
            else double filteredDistance.Length / ((filteredDistance |> List.averageBy double) / double chapterLength)
        
    //if true war, else peace
    let compareDensities (chapterLength: int) (warMatchesIndexes: int list) (peaceMatchesIndexes: int list) (densityFunction: int -> int list -> double) =
        if (warMatchesIndexes.Length = 0 && peaceMatchesIndexes.Length >= 1) then
            false
        else if (warMatchesIndexes.Length >= 1 && peaceMatchesIndexes.Length = 0) then
            true
        else
            let partiallyAppliedDensity = densityFunction chapterLength
            partiallyAppliedDensity warMatchesIndexes > partiallyAppliedDensity peaceMatchesIndexes     
    
    //calculates number of matches via the index of the matches with the terms for each chapter
    //compares the results for each topic
    //1 means war
    //-1 means peace
    let parseChapterReturnTopic (warTerms: string Set) (peaceTerms: string Set) (acc: int list) (chapterContent: string list) =
        
        let warIndexFilter = indexOfMatchesWithSet warTerms chapterContent        
        let peaceIndexFilter = indexOfMatchesWithSet peaceTerms chapterContent
        
        if (compareDensities chapterContent.Length warIndexFilter peaceIndexFilter calculateDensityAvgRelativeDistance) then
            1 :: acc //faster then adding at the end of the list --> have to revert
        else
            -1 :: acc
       
    
    let calculateChapterArgument (formattedBookContent: string list list) (warTerms: string Set) (peaceTerms: string Set) =
        let curriedParseChapterReturnTopic = parseChapterReturnTopic warTerms peaceTerms
        List.fold curriedParseChapterReturnTopic [] formattedBookContent |> List.rev //we use list rev because it is faster to append the elements at the beginning of the list
        //and then invert it, List.fold is already parallel
    
    let formatChapterTopicResults (chapterScores: int list) =
        chapterScores
        |> List.indexed
        |> List.map
               (fun el ->
                    let relationValue = snd el
                    match relationValue with
                    | relationValue when relationValue > 0 -> $"Chapter %A{fst el + 1}: war-related"
                    | relationValue when relationValue < 0 -> $"Chapter %A{fst el + 1}: peace-related"
                    | _ -> $"Chapter %A{fst el + 1}: no idea"
               )