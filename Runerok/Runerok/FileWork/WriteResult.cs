namespace Runerok.FileWork;

public class WriteResult
{
    private const string Path = "../../../../Files/result.txt";
    
    public void WriteSuccess()
    {
        using var writer = new StreamWriter(Path);
        
        writer.WriteLine("Ок");
        
        writer.Close();
    }

    public void WriteError(int index, string getExpression, List<string> expectedExpressions)
    {
        using var writer = new StreamWriter(Path);

        writer.Write($"Ошибка на позиции {index}. Получено {getExpression}, а ожидалось {string.Join(", ", expectedExpressions)}");
        writer.WriteLine();
        
        writer.Close();
    }
}