using CreateLL1Table.Models;
using Rule = CreateLL1Table.Models.Rule;

namespace CreateLL1Table.TableWork;

public class TableCreator
{
    private List<Rule> Grammar { get; set; }
    private const string EndSymbol = "#";
    private const char LessSymbol = '<';
    private const char MoreSymbol = '>';
    private const string EmptyTransition = "e";

    public TableCreator(List<Rule> grammar)
    {
        Grammar = grammar;
    }

    private bool IsNonTerminal(string elem)
    {
        if (elem[0] == LessSymbol && elem.EndsWith(MoreSymbol))
            return true;

        return false;
    }

    private void FillTableWithLeftSideOfGrammar(ref List<RowTableLL1> tableLL1)
    {
        int pointer = Grammar.Count();
        
        var shift = false;
        var toStack = false;
        var end = false;
        
        for (int grammarIndex = 0; grammarIndex < Grammar.Count(); grammarIndex++)
        {
            var error = true;
            var symbol = Grammar[grammarIndex].Symbol;
            var guideSet = Grammar[grammarIndex].GuideSet;

            var nextIndex = grammarIndex + 1;
            if (nextIndex < Grammar.Count() && Grammar[nextIndex].Symbol == symbol)
            {
                error = false;
            }

            var rowTableLL1 = new RowTableLL1(symbol, guideSet, error, pointer, shift, toStack, end);
            tableLL1.Add(rowTableLL1);
            
            pointer += Grammar[grammarIndex].RightPart.Count();
        }
    }

    private void FillGuideSetForNonTerminal(ref List<RowTableLL1> tableLL1, ref List<string> guideSet, string symbol)
    {
        for (int indexTableLL1 = 0; indexTableLL1 < Grammar.Count(); indexTableLL1++)
        {
            if (tableLL1[indexTableLL1].Symbol == symbol)
            {
                guideSet.AddRange(tableLL1[indexTableLL1].GuideSet);
            }
        }

        guideSet = guideSet.Distinct().ToList();
    }

    private void FillTableWithRightSideOfGrammar(ref List<RowTableLL1> tableLL1)
    {
        var error = true;
        
        for (int indexGrammar = 0; indexGrammar < Grammar.Count(); indexGrammar++)
        {
            var rightPart = Grammar[indexGrammar].RightPart;
            for (int indexRightPart = 0; indexRightPart < rightPart.Count(); indexRightPart++)
            {
                var symbol = rightPart[indexRightPart];
                List<string> guideSet = new List<string>();
                var pointer = -1;
                var shift = false;
                var toStack = false;
                var end = false;

                if (IsNonTerminal(symbol))
                {
                    FillGuideSetForNonTerminal(ref tableLL1, ref guideSet, symbol);
                    if (indexRightPart != rightPart.Count() - 1)
                    {
                        toStack = true;
                    }
                    
                    for (int indexTableLL1 = 0; indexTableLL1 < Grammar.Count(); indexTableLL1++)
                    {
                        if (tableLL1[indexTableLL1].Symbol == symbol)
                        {
                            pointer = indexTableLL1;
                            break;
                        }
                    }
                }
                else
                {
                    if (symbol != EmptyTransition)
                    {
                        guideSet.Add(symbol);
                        shift = true;
                    }
                    else
                    {
                        guideSet.AddRange(Grammar[indexGrammar].GuideSet);
                    }

                    if (symbol == EndSymbol)
                    {
                        end = true;
                    }
                    
                    if (indexRightPart != rightPart.Count() - 1)
                    {
                        pointer = tableLL1.Count() + 1;
                    }
                }

                var rowTableLL1 = new RowTableLL1(symbol, guideSet, error, pointer, shift, toStack, end);
                tableLL1.Add(rowTableLL1);
            }
        }
    }

    public List<RowTableLL1> Create()
    {
        var tableLL1 = new List<RowTableLL1>();
        
        FillTableWithLeftSideOfGrammar(ref tableLL1);
        FillTableWithRightSideOfGrammar(ref tableLL1);

        return tableLL1;
    }
}