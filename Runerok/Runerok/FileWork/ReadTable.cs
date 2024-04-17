using Microsoft.VisualBasic.FileIO;
using Runerok.Models;

namespace Runerok.FileWork;

public class ReadTable
{
    private const string Path = "../../../../Files/output.csv";

    public List<RowTableLL1> Read()
    {
        var tableLL1 = new List<RowTableLL1>();
        
        using var reader = new StreamReader(Path);

        reader.ReadLine();

        string? line;
        var rowTableLL1List = new List<string>();
        
        while (!string.IsNullOrEmpty(line = reader.ReadLine()))
        {
            rowTableLL1List = line.Split("; ").ToList();
            
            var rowTableLL1 = new RowTableLL1(
                rowTableLL1List[1],
                rowTableLL1List[2].Split(", ").ToList(),
                bool.Parse(rowTableLL1List[3]),
                int.Parse(rowTableLL1List[4]),
                bool.Parse(rowTableLL1List[5]),
                bool.Parse(rowTableLL1List[6]),
                bool.Parse(rowTableLL1List[7]));
            
            tableLL1.Add(rowTableLL1);
        }
        
        reader.Close();

        return tableLL1;
    }
}