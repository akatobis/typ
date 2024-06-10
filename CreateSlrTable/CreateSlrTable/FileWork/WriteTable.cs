using CreateSlrTable.Models;

namespace CreateSlrTable.FileWork;

public class WriteTable
{
    private const string FilePath = "../../../../../SLR/file.csv";

    public void Write(List<string> typeSymbols, List<Row> table)
    {
        using StreamWriter writer = new StreamWriter(FilePath);
        
        var firstStr = ";" + string.Join(";", typeSymbols);
        writer.WriteLine(firstStr);

        foreach (var row in table)
        {
            var str = string.Join("/", row.IndexCurrRowList.Select(_ => _.RowNum + "." + _.SymbolNum));
            str += ";" + string.Join(";", row.IndexToRowList.Select(_ => string.Join("/", _.Select(i => i.RowNum + "." + i.SymbolNum))));
            writer.WriteLine(str);
        }
        
        writer.Close();
    }
}