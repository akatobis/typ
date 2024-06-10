namespace SLR;

public class Rule
{
    public Rule(string symbol, List<string> rightPart, List<string> guideSet)
    {
        Symbol = symbol;
        RightPart = rightPart;
        GuideSet = guideSet;
    }
        
    public Rule(string symbol, List<string> rightPart)
    {
        Symbol = symbol;
        RightPart = rightPart;
        GuideSet = new List<string>();
    }
    
    public Rule(Rule rule)
    {
        Symbol = new string(rule.Symbol);
        RightPart = new List<string>(rule.RightPart);
        GuideSet = new List<string>(rule.GuideSet);
    }
    
    public string Symbol { get; set; }
    public List<string> RightPart { get; set; }
    public List<string> GuideSet { get; set; }
}