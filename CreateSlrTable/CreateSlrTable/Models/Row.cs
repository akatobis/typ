namespace CreateSlrTable.Models;

public class Row
{
    public Row()
    {
        IndexCurrRowList = new List<IndexSymbol>();
        IndexToRowList = new List<List<IndexSymbol>>();
    }
    
    public List<IndexSymbol> IndexCurrRowList { get; set; }
    public List<List<IndexSymbol>> IndexToRowList { get; set; }

    public static bool TableHasIndexRow(List<IndexSymbol> indexRow, List<Row> table)
    {
        foreach (var row in table)
        {
            for (int i = 0; i < row.IndexCurrRowList.Count; i++)
            {
                var indexCurrRow = row.IndexCurrRowList[i];
                var index = indexRow[i];
                
                if (row.IndexCurrRowList.Count != indexRow.Count 
                    || indexCurrRow.RowNum != index.RowNum 
                    || indexCurrRow.SymbolNum != index.SymbolNum)
                {
                    break;
                }
                
                if (i + 1 == row.IndexCurrRowList.Count)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static void AddRow(Row row, List<Row> table, List<Rule> grammar, List<string> typeSymbols)
    {
        foreach (var indexToRowList in row.IndexToRowList)
        {
            if (indexToRowList.Count == 0 ||
                indexToRowList[0].RowNum == -2 ||
                indexToRowList[0].RowNum == -1 ||
                TableHasIndexRow(indexToRowList, table))
            {
                continue;
            }

            var newRow = new Row
            {
                IndexCurrRowList = new List<IndexSymbol>(indexToRowList)
            };

            foreach (var symbol in typeSymbols)
            {
                newRow.IndexToRowList.Add(new List<IndexSymbol>());
            }

            foreach (var indexCurrRow in newRow.IndexCurrRowList)
            {
                var firstList = new List<First>();
                var followList = Follow.FindFollow(indexCurrRow, grammar);
                if (followList[0].IndexSymbol.RowNum != -1 && Rule.IsNotTerminal(followList[0].NameSymbol))
                {
                    firstList = First.FindAllFirst(followList[0].NameSymbol, grammar);
                }

                for (int i = 0; i < typeSymbols.Count; i++)
                {
                    var symbol = typeSymbols[i];

                    foreach (var first in firstList)
                    {
                        if (symbol == first.NameSymbol)
                        {
                            newRow.IndexToRowList[i].Add(first.IndexSymbol);
                        }
                    }

                    foreach (var follow in followList)
                    {
                        if (symbol == follow.NameSymbol)
                        {
                            newRow.IndexToRowList[i].Add(follow.IndexSymbol);
                        }
                    }
                }
            }
            
            table.Add(newRow);

            AddRow(newRow, table, grammar, typeSymbols);
        }
    }
}