namespace Runerok.FileWork;

public class ReadExpression
{
    private const string Path = "../../../../Files/expression.txt";
    
    public List<string> Read()
    {
        using var reader = new StreamReader(Path);

        var expression = reader.ReadLine();
        
        reader.Close();

        return string.IsNullOrEmpty(expression) ? new List<string>() : expression.Trim().Split(" ").ToList();
    }
}