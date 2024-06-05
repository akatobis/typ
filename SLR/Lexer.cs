namespace SLR;

public class Lexer
{
    private const string Space = " ";
    private List<string> _tokens;
    private int _tokenIndex;
    
    public Lexer(string inputFileName)
    {
        var reader = new StreamReader(inputFileName);

        _tokens = new List<string>();
        _tokenIndex = 0;
        
        while (reader.ReadLine() is { } line)
        {
            var words = line.Split(Space);
            foreach (var word in words)
            {
                if (!string.IsNullOrEmpty(word.Trim())) _tokens.Add(word.Trim());
            }
        }
    }

    public string? GetNextToken()
    {
        if (_tokenIndex == _tokens.Count) return null;
        return _tokens[_tokenIndex++];
    }
    
    public string? PickNextToken()
    {
        if (_tokenIndex + 1 == _tokens.Count) return null;
        return _tokens[_tokenIndex + 1];
    }
}