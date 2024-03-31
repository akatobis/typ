using CreateLL1Table.FileWork;
using CreateLL1Table.Printers;
using CreateLL1Table.TableWork;

var readGrammar = new ReadGrammar();
var grammar = readGrammar.Read();

var tableCreator = new TableCreator(grammar);
var tableLL1 = tableCreator.Create();

var filePath = "../../../../Files/output.csv";
Printers.PrintTableLL1(filePath, tableLL1);