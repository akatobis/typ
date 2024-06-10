using CreateLL1Table.FileWork;
using CreateLL1Table.Printers;
using CreateLL1Table.TableWork;

var readGrammar = new ReadGrammar();
var fileParser = new ReadGrammar.FileParser("../../../../Files/test1.txt", false);
fileParser.ParseLinesToGrammarRules();

var grammar = fileParser.GrammarRules;
fileParser.AddBracketsToNonTerminals(ref grammar);

Printers.PrintGrammar(grammar);

var tableCreator = new TableCreator(grammar);
var tableLL1 = tableCreator.Create();

var filePath = "../../../../Files/output.csv";
Printers.PrintTableLL1(filePath, tableLL1);