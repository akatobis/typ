using SLR;

class Program
{
    static void Main(string[] args)
    {
        const string filePath = "file.csv";
        const string stackTraceFileName = "stackTrace.txt";
        const string outputFileName = "result.txt";
        
        CsvParser csvParser = new CsvParser(filePath, stackTraceFileName, outputFileName);

        Lexer lexer = new Lexer("inputLexer.txt");
        Creator creator = new Creator("inputGrammar.txt", "outputGrammar.txt");
        
        creator.AddGuideSetToGrammar();
        var grammar = creator.GetGrammar();
            //csvParser.ProcessInputSequence(lexer, grammar);
    }
}