using SLR;

public class CsvParser
{
    private Dictionary<string, Dictionary<string, string>> table;
    private Stack<string> stack;
    private readonly StreamWriter _outputWriter;
    private readonly StreamWriter _stackTraceWriter;

    public CsvParser(string filePath, string stackTraceFileName, string outputFileName)
    {
        table = ReadCsvFile(filePath);
        stack = new Stack<string>();
        _outputWriter = new StreamWriter(outputFileName);
        _stackTraceWriter = new StreamWriter(stackTraceFileName);
    }

    public void ProcessInputSequence(Lexer lexer, List<Rule> rules)
    {
         var resultMsg = ProcessInput(lexer, rules, _stackTraceWriter) ? "Ok" : "False";
         
        _stackTraceWriter.Close();
        _outputWriter.WriteLine(resultMsg);
        _outputWriter.Close();
    }

    private void WriteStackTrace(StreamWriter stackTraceWriter, Lexer lexer)
    {
        stackTraceWriter.Write("Remaining tokens:");
        var remainingTokens = lexer.GetRemainingTokens(); 
        foreach (var remainingToken in remainingTokens)
        {
            stackTraceWriter.Write($" '{remainingToken}'");
        }
        stackTraceWriter.WriteLine();
        stackTraceWriter.Write("Stack:");
        foreach (var item in stack)
        {
            stackTraceWriter.Write($" '{item}'");
        }
        stackTraceWriter.WriteLine();
    }
    
    private bool ProcessInput(Lexer lexer, List<Rule> rules, StreamWriter stackTraceWriter)
    {
        const string ok = "-2.-2";
        const string firstRow = "0.0";
        const string convolution = "-1";
        const string end = "#";
        const string empty = "e";
        
        stack.Push(firstRow);
        while (true)
        {
            WriteStackTrace(stackTraceWriter, lexer);

            if (stack.Count == 0)
            {
                return false;
            }
            var currState = stack.Peek();
            var token = lexer.PeekCurrToken();
            
            if (token == null)
            {
                return false;
            }
            if (table[currState].TryGetValue(token, out string? value))
            {
                if (value.Length == 0)
                {
                    return false;
                }
                if (value == ok)
                {
                    return true;
                }
                if (value.Contains(convolution))
                {
                    var firstValue = value.Split('/').First();
                    if (firstValue == "")
                    {
                        return false;
                    }
                    var leftRightValues = firstValue.Split('.');
                    if (leftRightValues.Length < 2)
                    {
                        return false;
                    }

                    var rowNumber = leftRightValues[1];
                    var rule = rules[int.Parse(rowNumber)];
                    
                    for (int i = 0; i < rule.RightPart.Count; ++i)
                    {
                        if (rule.RightPart[i] == empty || rule.RightPart[i] == end)
                        {
                            break;
                        }
                        if (stack.Count == 0)
                        {
                            return false;
                        }
                        stack.Pop();
                    }

                    lexer.AddToken(rule.Symbol);
                    continue;
                }
                
                lexer.GetNextToken();
                stack.Push(value);
                continue;
            }

            return false;
        }
    }

    private Dictionary<string, Dictionary<string, string>> ReadCsvFile(string filePath)
    {
        var csvData = new Dictionary<string, Dictionary<string, string>>();

        try
        {
            using (var reader = new StreamReader(filePath))
            {
                // Read the header line
                var headerLine = reader.ReadLine();
                if (headerLine == null)
                {
                    throw new InvalidDataException("The CSV file is empty.");
                }

                // Split the header line into column headers
                var headers = ParseCsvLine(headerLine);

                // Initialize the dictionary with empty dictionaries for each row key
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line != null)
                    {
                        var values = ParseCsvLine(line);

                        var rowKey = values[0]; // Assuming the first value in the row is the key
                        csvData[rowKey] = new Dictionary<string, string>();

                        // Fill the inner dictionary with column headers and their respective values
                        for (int i = 1; i < headers.Count; i++)
                        {
                            csvData[rowKey][headers[i]] = values[i];
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading CSV file: {ex.Message}");
        }

        return csvData;
    }

    private List<string> ParseCsvLine(string line)
    {
        // This method splits a CSV line considering the quoted values
        var values = new List<string>();
        bool inQuotes = false;
        string currentValue = "";

        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ';' && !inQuotes)
            {
                values.Add(currentValue);
                currentValue = "";
            }
            else
            {
                currentValue += c;
            }
        }

        // Add the last value
        values.Add(currentValue);

        return values;
    }

    private int ParseIndex(string input)
    {
        // Parse the index from the input string
        if (int.TryParse(input, out int index))
        {
            return index;
        }
        else
        {
            return -1; // Return -1 if parsing fails
        }
    }
}
