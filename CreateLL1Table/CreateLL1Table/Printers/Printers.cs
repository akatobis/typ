using CreateLL1Table.Models;

namespace CreateLL1Table.Printers;

public static class Printers
{
    public static void PrintTableLL1(string filePath, List<RowTableLL1> tableLL1)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            var firstStr = "Index;Symbol;GuideSet;Error;Pointer;Shift;ToStack;End";
            writer.WriteLine(firstStr);
            
            for (int it = 0; it < tableLL1.Count(); it++)
            {
                RowTableLL1 row = tableLL1[it];
                var strToWrite = it + "; ";
                strToWrite += row.Symbol + "; ";
    
                for (int igs = 0; igs < row.GuideSet.Count(); igs++)
                {
                    var guideSymbol = row.GuideSet[igs];
                    strToWrite += guideSymbol;
                    strToWrite += igs == row.GuideSet.Count() - 1
                        ? "; "
                        : ", ";
                }

                strToWrite += row.Error + "; ";
                strToWrite += row.Pointer + "; ";
                strToWrite += row.Shift + "; ";
                strToWrite += row.ToStack + "; ";
                strToWrite += row.End + "; ";
    
                writer.WriteLine(strToWrite);
            }
        }
    }

    public static void PrintGrammar(List<Rule> grammar)
    {
        var filePath = "../../../../Files/outputBEBRA.txt";
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int ig = 0; ig < grammar.Count(); ig++)
            {
                var row = grammar[ig];
                var strToWrite = "";

                strToWrite += $"{row.Symbol} -> ";
                for (int irp = 0; irp < row.RightPart.Count(); irp++)
                {
                    strToWrite += row.RightPart[irp];
                    strToWrite += irp == row.RightPart.Count() - 1
                        ? " / "
                        : " ";
                }
    
                for (int igs = 0; igs < row.GuideSet.Count(); igs++)
                {
                    strToWrite += row.GuideSet[igs];
                    strToWrite += igs == row.GuideSet.Count() - 1
                        ? ""
                        : ", ";
                }
    
                writer.WriteLine(strToWrite);
            }
        }
    }
}