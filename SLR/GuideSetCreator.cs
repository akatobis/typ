namespace SLR;

public class Creator(string inputFileName, string outputFileName)
{
    private class Rule
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

    private const string Arrow = "->";
    private const string Space = " ";
    private const char LessSymbol = '<';
    private const char MoreSymbol = '>';
    
    private readonly StreamReader _reader = new StreamReader(inputFileName);
    private readonly StreamWriter _writer = new StreamWriter(outputFileName);

    private List<Rule> _grammar = null!;

    private void ReadGrammar()
    {
        _grammar = new List<Rule>();

        while (_reader.ReadLine()! is { } line)
        {
            var parts = line.Split(Arrow);
            var symbol = parts[0].Trim();

            parts = parts[1].Split(Space);
            var rightPart = new List<string>();

            foreach (var word in parts)
            {
                if (word != Space && !string.IsNullOrEmpty(word)) rightPart.Add(word.Trim());
            }
            
            _grammar.Add(new Rule(symbol, rightPart));
        }
    }

    private void CloseFiles()
    {
        _reader.Close();
        _writer.Close();
    }

    private bool IsNonTerminal(string token)
    {
        return token.First() == LessSymbol && token.Last() == MoreSymbol;
    }

    private void FindFirst(string firstRightPartItem, ref List<string> guideSet)
    {
        foreach (var rule in _grammar)
        {
            if (rule.Symbol != firstRightPartItem) continue;

            var firstElement = rule.RightPart.First();
            if (!guideSet.Contains(firstElement))
            {
                guideSet.Add(firstElement);
                if (IsNonTerminal(firstElement)) FindFirst(firstElement, ref guideSet);
            }
        }
    }

    public void AddGuideSetToGrammar()
    {
        ReadGrammar();

        var grammarWithGuideSet = new List<Rule>();
        foreach (var rule in _grammar)
        {
            var guideSet = new List<string>();

            var firstRightPartItem = rule.RightPart[0];
            guideSet.Add(firstRightPartItem);

            if (IsNonTerminal(firstRightPartItem))
            {
                FindFirst(firstRightPartItem, ref guideSet);
            }
            
            grammarWithGuideSet.Add(rule);
            grammarWithGuideSet.Last().GuideSet = guideSet;
        }

        _grammar = grammarWithGuideSet;
        
        PrintGrammar();
        
        CloseFiles();
    }

    private void PrintGrammar()
    {
        foreach (var rule in _grammar)
        {
            _writer.Write($"{rule.Symbol} -> ");

            foreach (var rightPartToken in rule.RightPart)
            {
                _writer.Write($"{rightPartToken} ");
            }

            if (rule.GuideSet.Count == 0)
            {
                _writer.WriteLine();
                continue;
            }

            _writer.Write("/ ");
            foreach (var guideSetToken in rule.GuideSet)
            {
                _writer.Write(guideSetToken);
                if (rule.GuideSet.Last() != guideSetToken) _writer.Write(", ");
            }

            _writer.WriteLine();
        }
    }
}