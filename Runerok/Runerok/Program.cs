using Runerok.FileWork;

const int nullPointerLL1 = -1;

var readTable = new ReadTable();
var readExpression = new ReadExpression();

var writeResult = new WriteResult();

var tableLL1 = readTable.Read();
var expression = readExpression.Read();

var currRowIndex = 0;
var stack = new Stack<int>();

var rowTableLL1 = tableLL1[currRowIndex];

var currPartExpressionIndex = 0;
var currPartExpression = expression[currPartExpressionIndex];

const string pathTraceWriter = "../../../../Files/trace.txt"; 
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
        if (currPartExpressionIndex + 1 >= expression.Count)
        {
            Console.WriteLine("error");
            writeResult.WriteError(++currPartExpressionIndex, currPartExpression, rowTableLL1.GuideSet);
            break;
        }
        
        currPartExpression = expression[++currPartExpressionIndex];
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