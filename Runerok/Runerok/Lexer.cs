namespace Runerok;

public class Lexer
{
    private const string Space = " ";
    private const string EndOfString = "#";
    private const char QuotationMark = '"';
    private const char Apostrophe = '\'';
    private List<string> _tokens;
    private int _tokenIndex;

    private const string NumberToken = "number";
    private const string IdToken = "id";
    private const string StringToken = "string";
    private const string CharToken = "char";
    
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
                if (string.IsNullOrEmpty(word.Trim())) continue;
                
                var wordT = new string(word);
                wordT = wordT.Trim();

                if (int.TryParse(wordT, out _) || float.TryParse(wordT, out _))
                {
                    _tokens.Add(NumberToken);
                    continue;
                }

                /*if (wordT.First() == '_' && wordT.Length > 1 ||
                    char.IsLetter(wordT.First()))
                {
                    _tokens.Add(IdToken);
                    continue;
                }*/

                if (wordT.Length >= 2 && wordT.First() == QuotationMark && wordT.Last() == QuotationMark)
                {
                    _tokens.Add(StringToken);
                    continue;
                }

                if (wordT.First() == Apostrophe && wordT.Last() == Apostrophe)
                {
                    if (wordT.Length != 3) throw new Exception("Тип char должен состоять из одного символа");
                    
                    _tokens.Add(CharToken);
                    continue;
                }
                
                if (!string.IsNullOrEmpty(word.Trim())) _tokens.Add(word.Trim());
            }
        }
        
        _tokens.Add(EndOfString);
    }

    public int GetTokenIndex()
    {
        return _tokenIndex;
    }

    public int GetExpressionCount()
    {
        return _tokens.Count;
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