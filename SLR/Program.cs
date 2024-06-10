using SLR;

class Program
{
    static void Main(string[] args)
    {
        const string inputSequenceFileName = "../../../inputLexer.txt";
        var lexer = new Lexer(inputSequenceFileName);
        
        const string inputGrammarFileName = "../../../inputGrammar.txt";
        const string outputGrammarFileName = "../../../outputGrammar.txt";
        var creator = new Creator(inputGrammarFileName, outputGrammarFileName);
        creator.AddGuideSetToGrammar();
        var grammar = creator.GetGrammar();
        
        const string tableFileName = "../../../file.csv";
        const string stackTraceFileName = "../../../stackTrace.txt";
        const string outputFileName = "../../../result.txt";
        var csvParser = new CsvParser(tableFileName, stackTraceFileName, outputFileName);
        csvParser.ProcessInputSequence(lexer, grammar);
    }
}