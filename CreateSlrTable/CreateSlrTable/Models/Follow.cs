namespace CreateSlrTable.Models;

public class Follow
{
    public Follow(IndexSymbol indexSymbol, string nameSymbol)
    {
        IndexSymbol = indexSymbol;
        NameSymbol = nameSymbol;
    }

    public IndexSymbol IndexSymbol { get; set; }
    public string NameSymbol { get; set; }

    private static bool FollowHasInList(Follow searchFollow, List<Follow> followList)
    {
        return followList
            .Any(follow => follow.NameSymbol == searchFollow.NameSymbol 
                           && follow.IndexSymbol.SymbolNum == searchFollow.IndexSymbol.SymbolNum 
                           && follow.IndexSymbol.RowNum == searchFollow.IndexSymbol.RowNum);
    }

    private static List<Follow> FindAllFollow(string searchSymbol, List<Rule> grammar, int convolutionNum)
    {
        var followList = new List<Follow>();

        foreach (var rule in grammar)
        {
            for (var i = 0; i < rule.RightPart.Count; i++)
            {
                var symbol = rule.RightPart[i];

                if (symbol != searchSymbol)
                {
                    continue;
                }

                if (i + 1 == rule.RightPart.Count)
                {
                    foreach (var newFollow in FindAllFollow(rule.Symbol, grammar, convolutionNum))
                    {
                        if (!FollowHasInList(newFollow, followList))
                        {
                            followList.Add(newFollow);
                        }
                    }
                }
                else
                {
                    var newFollow = new Follow(new IndexSymbol(-1, convolutionNum), rule.RightPart[i + 1]);
                    if (!FollowHasInList(newFollow, followList))
                    {
                        followList.Add(newFollow);    
                    }

                    if (Rule.IsNotTerminal(newFollow.NameSymbol))
                    {
                        var firstList = First.FindAllFirst(newFollow.NameSymbol, grammar);
                        foreach (var first in firstList)
                        {
                            newFollow = new Follow(new IndexSymbol(-1, convolutionNum), first.NameSymbol);
                            if (!FollowHasInList(newFollow, followList))
                            {
                                followList.Add(newFollow);
                            }
                        }
                    }
                }
            }
        }

        return followList;
    }

    public static List<Follow> FindFollow(IndexSymbol indexSymbol, List<Rule> grammar)
    {
        var followList = new List<Follow>();
        var row = grammar[indexSymbol.RowNum];

        if (row.RightPart.Count == indexSymbol.SymbolNum)
        {
            followList.AddRange(FindAllFollow(row.Symbol, grammar, indexSymbol.RowNum));
        }
        else
        {
            var indexSymbolFollow = new IndexSymbol(indexSymbol.RowNum, indexSymbol.SymbolNum + 1);
            if (row.RightPart[indexSymbol.SymbolNum] == "#")
            {
                indexSymbolFollow = new IndexSymbol(-1, indexSymbol.RowNum);
            }
            var follow = new Follow(indexSymbolFollow, row.RightPart[indexSymbol.SymbolNum]);
            followList.Add(follow);
        }

        return followList;
    }
}