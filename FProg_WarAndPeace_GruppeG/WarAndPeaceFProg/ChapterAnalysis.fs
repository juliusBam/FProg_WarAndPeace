namespace WarAndPeaceFProg

module ChapterAnalysis =
    let curriedFindWordMatch warTerms peaceTerms word =
        if (warTerms |> Set.contains word) then 1
        else if (peaceTerms |> Set.contains word) then -1
        else 0

    let curriedAnalyseWords warTerms peaceTerms chapterContent =
        let applyFindWordMatch = curriedFindWordMatch warTerms peaceTerms
        chapterContent |> List.map applyFindWordMatch |> List.sum

    let calculateChapterArgument (formattedBookContent: string list list) (warTerms: string Set) (peaceTerms: string Set) =
        let analyseChapterContent = curriedAnalyseWords warTerms peaceTerms
        formattedBookContent |> List.map analyseChapterContent