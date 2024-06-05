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
    
    public string Symbol { get; set; }
    public List<string> RightPart { get; set; }
    public List<string> GuideSet { get; set; }
}