namespace CreateLL1Table.Models;

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
}