using CreateLL1Table.Models;

namespace CreateLL1Table.FileWork;

public class ReadGrammar
{
    private const string FilePath = "../../../../Files/test2.txt";
    
    public List<Rule> Read()
    {
        var grammar = new List<Rule>();
        var reader = new StreamReader(FilePath);
        
        while (reader.ReadLine() is { } line)
        {
            var symbol = line.Split("->")[0].Trim();
            var rightPart = line.Split("->")[1].Split("/");//[0].Trim().Split(" ").ToList();
            var valueGrammar = new List<string>();
            for (var i = 0; i < rightPart.Length - 1; i++)
            {
                var rightPartElem = rightPart[i];
                if (i > 0) { valueGrammar.Add("/"); }
                valueGrammar.AddRange(rightPartElem.Trim().Split(" "));
            }
            
            var guideSet = line.Split("->")[1].Split("/")[rightPart.Length - 1].Trim().Split(", ").ToList();
            var rule = new Rule(symbol, valueGrammar, guideSet);
            grammar.Add(rule);
        }
        
        reader.Close();
        return grammar;
    }
}