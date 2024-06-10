namespace CreateSlrTable.Models;

public class First
{
    public First(IndexSymbol indexSymbol, string nameSymbol)
    {
        IndexSymbol = indexSymbol;
        NameSymbol = nameSymbol;
    }

    public IndexSymbol IndexSymbol { get; set; }
    public string NameSymbol { get; set; }
    
    private static bool FirstHasInList(First searchFirst, List<First> firstList)
    {
        return firstList
            .Any(first => first.NameSymbol == searchFirst.NameSymbol 
                          && first.IndexSymbol.SymbolNum == searchFirst.IndexSymbol.SymbolNum 
                          && first.IndexSymbol.RowNum == searchFirst.IndexSymbol.RowNum);
    }
    

    public static List<First> FindAllFirst(string notTerminal, List<Rule> grammar)
    {
        var firstList = new List<First>();
    
        for (var i = 0; i < grammar.Count; i++)
        {
            var rule = grammar[i];
            if (rule.Symbol == notTerminal)
            {
                var first = rule.RightPart[0];

                if (first == "e")
                {
                    var indexSymbol = new IndexSymbol(-1, 0);
                    var newFirst = new First(indexSymbol, "#");
                    if (!FirstHasInList(newFirst, firstList))
                        firstList.Add(newFirst);
                }
                else
                {
                    var indexSymbol = new IndexSymbol(i, 1);
                    var newFirst = new First(indexSymbol, first);
                    
                    if (!FirstHasInList(newFirst, firstList))
                        firstList.Add(newFirst);
                
                    if (Rule.IsNotTerminal(first) && first != notTerminal)
                    {
                        foreach (var first1 in FindAllFirst(first, grammar))
                        {
                            if (!FirstHasInList(first1, firstList))
                                firstList.Add(first1);
                        }
                    }
                }
            }
        }

        return firstList;
    } 
}