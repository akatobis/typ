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
    

    public static List<First> FindAllFirst(string notTerminal, List<Rule> grammar)
    {
        var firstList = new List<First>();
    
        for (var i = 0; i < grammar.Count; i++)
        {
            var rule = grammar[i];
            if (rule.Symbol == notTerminal)
            {
                var first = rule.RightPart[0];
                
                var indexSymbol = new IndexSymbol(i, 1);
                firstList.Add(new First(indexSymbol, first));
                
                if (Rule.IsNotTerminal(first) && first != notTerminal)
                {
                    firstList.AddRange(FindAllFirst(first, grammar));
                }
            }
        }

        return firstList;
    } 
}