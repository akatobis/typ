using CreateSlrTable.FileWork;
using CreateSlrTable.Models;


var readGrammar = new ReadGrammar();
var grammar = readGrammar.Read();

var symbolList = Rule.GetSymbolList(grammar);

var table = new List<Row>();
var row = new Row();
row.IndexCurrRowList.Add(new IndexSymbol(0, 0));
row.IndexToRowList.Add(new List<IndexSymbol>{new (-2,-2)});

var nextSymbol = grammar[0].RightPart[0];

if (Rule.IsNotTerminal(nextSymbol))
{
    var firstList = First.FindAllFirst(nextSymbol, grammar);

    for (var i = 1; i < symbolList.Count; i++)
    {
        var symbol = symbolList[i];
        row.IndexToRowList.Add(new List<IndexSymbol>());
        
        if (symbol == nextSymbol)
        {
            row.IndexToRowList[i].Add(new IndexSymbol(0, 1));
        }
        
        foreach (var first in firstList)
        {
            if (symbol == first.NameSymbol)
            {
                row.IndexToRowList[i].Add(first.IndexSymbol);
            }
        }
    }
}

table.Add(row);

Row.AddRow(row, table, grammar, symbolList);

var writeTable = new WriteTable();
writeTable.Write(symbolList, table);