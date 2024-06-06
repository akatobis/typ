namespace CreateSlrTable.Models;

public class IndexSymbol
{
    public IndexSymbol(int rowNum, int symbolNum)
    {
        RowNum = rowNum;
        SymbolNum = symbolNum;
    }

    public int RowNum { get; set; }
    public int SymbolNum { get; set; }
}