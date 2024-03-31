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
            var rightPart = line.Split("->")[1].Split("/")[0].Trim().Split(" ").ToList();
            var guideSet = line.Split("->")[1].Split("/")[1].Trim().Split(", ").ToList();
            var rule = new Rule(symbol, rightPart, guideSet);
            grammar.Add(rule);
        }
        
        reader.Close();
        return grammar;
    }
}