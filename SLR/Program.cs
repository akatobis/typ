using SLR;

class Program
{
    static void Main(string[] args)
    {
        const string inputSequenceFileName = "../../../inputLexer.txt";
        Lexer lexer = new Lexer(inputSequenceFileName);
        
        const string inputGrammarFileName = "../../../inputGrammar.txt";
        const string outputGrammarFileName = "../../../outputGrammar.txt";
        Creator creator = new Creator(inputGrammarFileName, outputGrammarFileName);
        creator.AddGuideSetToGrammar();
        var grammar = creator.GetGrammar();
        
        const string tableFileName = "../../../file.csv";
        const string stackTraceFileName = "../../../stackTrace.txt";
        const string outputFileName = "../../../result.txt";
        CsvParser csvParser = new CsvParser(tableFileName, stackTraceFileName, outputFileName);
        csvParser.ProcessInputSequence(lexer, grammar);
    }
}