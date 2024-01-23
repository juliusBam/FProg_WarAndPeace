namespace WarAndPeaceFProg

open MathNet.Numerics.Statistics
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
    let scaleDistance (first: int) (second: int) =
        let diff = second - first
        match diff with
        | diff when diff < 100 -> int (diff / 2)
        | diff when diff < 400 -> diff * 2
        | _ -> diff
        
    let calculateDifference (first: int) (second: int) =
        let diff = second - first
        match diff with
        // | diff when diff < 0 -> double 0
        // | 0 -> double 0
        // | _ -> (double) diff / (double) chapterLength
        | diff when diff < 0 -> 0
        | _ -> diff
    
    let calculateDistance (indexList: int List) =
        if (indexList.Length = 1 || indexList.Length = 0) then
            [] //!important if list only has 1 element pairwise.map returns null
        else indexList |> List.pairwise |> List.map (fun (first: int, second: int) -> calculateDifference first second)
        //else indexList |> List.pairwise |> List.map (fun (first: int, second: int) -> second - first)
        //else indexList |> List.pairwise |> List.map (fun (first: int, second: int) -> scaleDistance first second)

    let calculateDensity (distanceList: int list) (chapterLength: int) =
        //printfn $"Chapter contains: %i{distanceList.Length} matches for topic"
        //50 matches without scale and with compare densities --> with punctuation as words
        //24 matches without scale and with compare densities and without punctuation
        // if (distanceList.IsEmpty || chapterLength = 0) then double 0
        // else
        //     double distanceList.Length / (double) (distanceList |> List.map (fun (distance: int) -> ((double) distance / (double) chapterLength)) |> List.averageBy double)
        //24 without sanitize line, with compare densities and with punctuation
        if (distanceList.IsEmpty || chapterLength = 0) then double 0
        else
            double distanceList.Length / ((distanceList |> List.averageBy double) / double chapterLength)
        
        //if (distanceList.IsEmpty) then double 0 else double (distanceList.Length + 1) / (distanceList |> List.averageBy double) //we calculate the density as #matches/avg distance
         //68 differences to the reference
         //let distanceSum = double (distanceList |> List.sum)
         //if (distanceSum = 0) then double 0 else double (distanceList.Length + 1) / distanceSum
        //67 differences to reference
        // let distanceSum = double (distanceList |> List.sum)
        // if (distanceSum = 0) then double 0 else chapterLength / (distanceList |> List.averageBy double)
         //85 differences to the reference
         //double (distanceList |> List.sum) / double (distanceList.Length + 1)
         //79
         //double (distanceList.Length + 1) / double (distanceList |> List.sum)
         //81
         //if distanceList.IsEmpty then double 0 else distanceList |> List.averageBy double
        //50 with "compare densities" without scale distance /// 45 with scale distance
        // else
        //     let dsty = double distanceList.Length / (distanceList |> List.averageBy double)
        //     printfn $"Density is: %f{dsty}"
        //     dsty
        
        //88
        // else
        //     double (double 1 / double (distanceList |> List.sum)) / double chapterLength
            
        //63
        //distanceList.Length / chapterLength
        //65
         // else
         //     let stdDev = double (DescriptiveStatistics(distanceList |> List.map (fun (el: int) -> float el)).StandardDeviation)
         //     let dsty = double distanceList.Length / (double chapterLength / ((distanceList |> List.averageBy double) / stdDev))
         //     printfn $"Density is: %f{dsty}"
         //     dsty
    
        //64 with compare densities
        // else
        //     let dsty = double (distanceList.Length) * (distanceList |> List.averageBy double)
        //     printfn $"Density is: %f{dsty}"
        //     dsty 
        
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
        let warDensity = calculateDensity warDistance chapterContent.Length
        
        let peaceIndexFilter = indexWithFilter peaceTerms chapterContent
        let peaceDistance = calculateDistance peaceIndexFilter
        let peaceDensity = calculateDensity peaceDistance chapterContent.Length
      
        if compareDensities warDensity (warDistance |> List.length) peaceDensity (peaceDistance |> List.length) then
            1 :: acc //faster then adding at the end of the list --> have to revert
        else
            -1 :: acc
       
    
    let calculateChapterArgument (formattedBookContent: string list list) (warTerms: string Set) (peaceTerms: string Set) =
        let curriedParseChapterReturnTopic = parseChapterReturnTopic warTerms peaceTerms
        List.fold curriedParseChapterReturnTopic [] formattedBookContent |> List.rev //we use list rev because it is faster to append the elements at the beginning of the list
        //and then invert it, List.fold is already parallel
       