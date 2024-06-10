namespace CreateSlrTable.Models;

public class Rule
{
    public Rule(string symbol, List<string> rightPart, List<string> guideSet)
    {
        Symbol = symbol;
        RightPart = rightPart;
        GuideSet = guideSet;
    }
    
    public string Symbol { get; set; }
    public List<string> RightPart { get; set; }
    public List<string> GuideSet { get; set; }
    
    public static bool IsNotTerminal(string symbol)
    {
        return symbol[0] == '<' && symbol[^1] == '>';
    }

    public static List<string> GetSymbolList(List<Rule> grammar)
    {
        var symbolList = new List<string>();

        foreach (var rule in grammar)
        {
            if (!symbolList.Contains(rule.Symbol))
            {
                symbolList.Add(rule.Symbol);
            }

            foreach (var symbolRightPart in rule.RightPart)
            {
                if (!symbolList.Contains(symbolRightPart) && symbolRightPart != "e")
                {
                    symbolList.Add(symbolRightPart);
                }
            }
        }

        return symbolList;
    }
}