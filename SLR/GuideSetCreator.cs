namespace SLR;

public class Creator(string inputFileName, string outputFileName)
{
    private const string Arrow = "->";
    private const string Space = " ";
    private const char LessSymbol = '<';
    private const char MoreSymbol = '>';
    private const string EmptyTransition = "e";
    private const string EndOfStringMark = "#";
    private string? _axiom;
    
    
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
            if (!guideSet.Contains(firstElement) && firstElement != EmptyTransition)
            {
                guideSet.Add(firstElement);
                if (IsNonTerminal(firstElement)) FindFirst(firstElement, ref guideSet);
            }
        }
    }

    public List<Rule> GetGrammar()
    {
        return _grammar;
    }

    private List<Rule> DeleteEmptyTransitions(List<Rule> grammar)
    {
        var grammarWithoutEmptyTransitions = new List<Rule>();
        var symbolsWithEmptyTransitions = new List<string>();

        foreach (var rule in grammar)
        {
            if (rule.RightPart.Contains(EmptyTransition)) symbolsWithEmptyTransitions.Add(rule.Symbol);
        }

        foreach (var rule in grammar)
        {
            if (!rule.RightPart.Contains(EmptyTransition))
            {
                grammarWithoutEmptyTransitions.Add(new Rule(rule));
            }
        }

        foreach (var symbolWithEmptyTransition in symbolsWithEmptyTransitions)
        {
            var newRules = new List<Rule>();
            
            foreach (var rule in grammarWithoutEmptyTransitions)
            {
                var ruleT = new Rule(rule);

                while (ruleT.RightPart.Contains(symbolWithEmptyTransition))
                {
                    ruleT.RightPart.Remove(symbolWithEmptyTransition);

                    if (ruleT.RightPart.Count == 0 && ruleT.Symbol == _axiom)
                    {
                        ruleT.RightPart.Add(EmptyTransition);
                    }

                    if (ruleT.RightPart.Count != 0) newRules.Add(new Rule(ruleT));
                }
            }
            
            grammarWithoutEmptyTransitions.AddRange(newRules);
        }
        
        return grammarWithoutEmptyTransitions;
    }

    public void AddGuideSetToGrammar()
    {
        ReadGrammar();
        if (_grammar.Count == 0) return;
        
        _axiom = _grammar[0].Symbol;
        
        _grammar = DeleteEmptyTransitions(_grammar);

        var rightPartForFirstRule = new List<string>
        {
            _axiom,
            EndOfStringMark
        };
        var symbolForFirstRule = _axiom.Insert(_axiom.IndexOf(MoreSymbol),"1");
        var firstRule = new Rule(symbolForFirstRule, rightPartForFirstRule);
        
        _grammar.Insert(0, firstRule);
        
        var grammarWithGuideSet = new List<Rule>();
        foreach (var rule in _grammar)
        {
            var guideSet = new List<string>();

            var firstRightPartItem = rule.RightPart[0];
            if (firstRightPartItem != EmptyTransition) guideSet.Add(firstRightPartItem);

            if (IsNonTerminal(firstRightPartItem))
            {
                FindFirst(firstRightPartItem, ref guideSet);
            }
            
            grammarWithGuideSet.Add(rule);
            grammarWithGuideSet.Last().GuideSet = guideSet;
        }

        _grammar = grammarWithGuideSet;
        
        PrintGrammar(_grammar);
        
        CloseFiles();
    }

    private void PrintGrammar(List<Rule> grammar)
    {
        foreach (var rule in grammar)
        {
            Console.Write($"{rule.Symbol} -> ");

            foreach (var rightPartToken in rule.RightPart)
            {
                Console.Write($"{rightPartToken} ");
            }

            if (rule.GuideSet.Count == 0)
            {
                Console.WriteLine();
                continue;
            }

            Console.Write("/ ");
            foreach (var guideSetToken in rule.GuideSet)
            {
                Console.Write(guideSetToken);
                if (rule.GuideSet.Last() != guideSetToken) Console.Write(", ");
            }

            Console.WriteLine();
        }
    }
}