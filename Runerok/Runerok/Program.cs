using Runerok;
using Runerok.FileWork;

const int nullPointerLL1 = -1;

var readTable = new ReadTable();

var writeResult = new WriteResult();

/*if (args.Length != 1)
{
    Console.WriteLine($"Invalid parameters.\nUsage: Runerok.exe <inputExpression.txt>");
    return 1;
}*/

var lexer = new Lexer("../../../../Files/expression.txt");
var tableLL1 = readTable.Read();

var currRowIndex = 0;
var stack = new Stack<int>();

var rowTableLL1 = tableLL1[currRowIndex];

var currPartExpressionIndex = lexer.GetTokenIndex();
var currPartExpression = lexer.GetNextToken();

const string pathTraceWriter = "../../../../Files/trace2.txt"; 
using var traceWriter = new StreamWriter(pathTraceWriter);
traceWriter.WriteLine($"{currRowIndex} Current symbol: {rowTableLL1.Symbol} Stack: {string.Join(" ", stack.ToArray())}");

while (rowTableLL1.End != true)
{
    if (!rowTableLL1.GuideSet.Contains(currPartExpression))
    {
        if (rowTableLL1.Error)
        {
            Console.WriteLine("error");
            writeResult.WriteError(++currPartExpressionIndex, currPartExpression, rowTableLL1.GuideSet);
            break;
        }
        
        currRowIndex++;
        
        rowTableLL1 = tableLL1[currRowIndex];
        continue;
    }
    
    if (rowTableLL1.ToStack)
    {
        stack.Push(currRowIndex + 1);
    }
    
    if (rowTableLL1.Shift)
    {
        if (currPartExpressionIndex >= lexer.GetExpressionCount())
        {
            Console.WriteLine("error");
            writeResult.WriteError(++currPartExpressionIndex, currPartExpression, rowTableLL1.GuideSet);
            break;
        }
        
        currPartExpression = lexer.GetNextToken();
        currPartExpressionIndex = lexer.GetTokenIndex();
    }
    
    currRowIndex = rowTableLL1.Pointer != nullPointerLL1 ? rowTableLL1.Pointer : stack.Pop();
    
    rowTableLL1 = tableLL1[currRowIndex];
    
    traceWriter.WriteLine($"{currPartExpression} {currRowIndex} Current symbol: {rowTableLL1.Symbol} Stack: {string.Join(" ", stack.ToArray())}");
}

traceWriter.Close();

if (rowTableLL1.End)
{
    Console.WriteLine("ok");
    writeResult.WriteSuccess();   
}

return 0;