using System.Text.RegularExpressions;
using CreateLL1Table.Models;

namespace CreateLL1Table.FileWork;

public class ReadGrammar
{
    public class FileParser(string fileName, bool directionSymbolsExistsInFile)
    {
        public readonly List<Rule> GrammarRules = [];

        private const char StartTokenCh = '<';
        private const char EndTokenCh = '>';
        private const int LineSeparationLength = 3;
        private const string EmptySymbol = "e";
        private const string EndSymbol = "#";
        private const string ReplacedMark = "1";

        private readonly string[] _lines = ReadFile(fileName);

        private readonly List<string> _tokens = [];

        private static string[] ReadFile(string fileName)
        {
            var fileStream = File.OpenRead(fileName);
            List<string> result = [];
            string? line;
            using var reader = new StreamReader(fileStream);
            while ((line = reader.ReadLine()) != null)
            {
                result.Add(line);
            }
            return result.ToArray();
        }

        private bool IsNonTerminal(string element)
        {
            var pattern = @"^1*[A-Z]+1*$";

            return Regex.IsMatch(element, pattern);
        }

        private string AddBracketsToString(string str)
        {
            return $"<{str}>";
        }
        
        public void AddBracketsToNonTerminals(ref List<Rule> grammar)
        {
            foreach (var rule in grammar)
            {
                if (IsNonTerminal(rule.Symbol)) rule.Symbol = AddBracketsToString(rule.Symbol);

                for (int i = 0; i < rule.RightPart.Count; i++)
                {
                    if (IsNonTerminal(rule.RightPart[i]))
                    {
                        rule.RightPart[i] = AddBracketsToString(rule.RightPart[i]);
                    }
                }
            }
        }

        public void ParseLinesToGrammarRules()
        {
            ParseTokens();

            for (int i = 0; i < _lines.Length; i++)
            {
                Rule grammarRule = new(_tokens[i], [], []);

                int startPos = _tokens[i].Length + 2 + LineSeparationLength;
                string line = _lines[i][startPos..];

                grammarRule.RightPart = ParseChainSymbols(line);
                
                GrammarRules.Add(grammarRule);
            }
            GrammarRules.Insert(0, new Rule(ReplacedMark + GrammarRules[0].Symbol, [GrammarRules[0].Symbol, EndSymbol], []));

            FixLeftRecursive();
            
            if (!directionSymbolsExistsInFile)
            {
                FindDirectionSymbolsByRules();
            }
        }

        private void FixLeftRecursive()
        {
            List<Rule> ruleыWithLeftRecursion = GrammarRules.FindAll(HasLeftRecursion);
            List<Rule> rulesPassed = [];
            foreach (Rule grammarRule in ruleыWithLeftRecursion)
            {
                RemoveLeftRecursion(grammarRule, rulesPassed);
                rulesPassed.Add(grammarRule);
            }
        }

        public void RemoveLeftRecursion(Rule rule, List<Rule> rulesPassed)
        {
            if (HasLeftRecursion(rule))
            {
                string newToken = rule.Symbol + ReplacedMark;

                var rules = GrammarRules.FindAll(x => x.Symbol == rule.Symbol && !HasLeftRecursion(x));

                if(rules.Count == 0)
                {
                    throw new Exception("Can't remove left recursion");
                }

                Rule newRuleForRemoveLeftRecursion = new(newToken, new(rule.RightPart.GetRange(1, rule.RightPart.Count - 1)), new(rule.GuideSet));
                newRuleForRemoveLeftRecursion.RightPart.Add(newToken);

                GrammarRules[GrammarRules.IndexOf(rule)] = newRuleForRemoveLeftRecursion;

                if(rulesPassed.FindAll(x => x.Symbol == rule.Symbol).Count > 0)
                {
                    return;
                }

                Rule ruleWithoutLeftRecursion = new (rules[0].Symbol, [], new(rule.GuideSet));
                for (int i = 0; i < rules.Count; i++)
                {
                    ruleWithoutLeftRecursion = rules[i];

                    if (ruleWithoutLeftRecursion.RightPart.Count == 0 )
                    {
                        continue;
                    }

                    Rule newRule;
                    if (ruleWithoutLeftRecursion.RightPart[0] == EmptySymbol)
                    {
                        newRule = new(rule.Symbol, [], new(rule.GuideSet));
                        newRule.RightPart.AddRange(newRuleForRemoveLeftRecursion.RightPart);

                        if (rules.FindAll(x => x.RightPart[0] != EmptySymbol).Count == 0)
                        {
                            GrammarRules.Insert(GrammarRules.IndexOf(ruleWithoutLeftRecursion) + 1, newRule);
                        }

                        continue;
                    }

                    newRule = new(ruleWithoutLeftRecursion.Symbol, new(ruleWithoutLeftRecursion.RightPart), new(rule.GuideSet)); 
                    newRule.RightPart.Add(newToken);

                    GrammarRules[GrammarRules.IndexOf(ruleWithoutLeftRecursion)] = newRule;
                }

                Rule epsilonRule = new(newToken, ["e"], new(rule.GuideSet));

                GrammarRules.Insert(GrammarRules.IndexOf(GrammarRules.FindLast(x => x.Symbol == newToken))+1, epsilonRule);
            }
        }

        private static bool HasLeftRecursion(Rule rule)
        {
            return rule.RightPart.Count > 0 && rule.RightPart[0] == rule.Symbol;
        }

        private void FindDirectionSymbolsByRules()
        {
            for (int index = 0; index < GrammarRules.Count; index++)
            {
                var grammarRule = GrammarRules[index];
                if (0 == grammarRule.GuideSet.Count)
                {
                    grammarRule.GuideSet.AddRange(FindDirectionSymbolsForToken(index));
                }
            }
        }

        private List<string> FindDirectionSymbolsForToken(int tokenIdx)
        {
            var grammarRule = GrammarRules[tokenIdx];
            var firstChainCharacter = grammarRule.RightPart[0];

            if (TokenIsNonTerminal(firstChainCharacter))
            {
                List<string> result = [];
                for (int i = 0; i < GrammarRules.Count; i++)
                {
                    if (GrammarRules[i].Symbol == firstChainCharacter && i != tokenIdx)
                    {
                        result.AddRange(FindDirectionSymbolsForToken(i));
                    }
                }
                
                return result.Distinct().ToList();
            }
           
            
            return grammarRule.RightPart.Contains(EmptySymbol) ? Follow(grammarRule.Symbol).Distinct().ToList() : [firstChainCharacter];
        }

        List<string> Follow(string token)
        {
            List<string> dirSymbols = [];

            List<Rule> grammarRules = GrammarRules.FindAll(x => x.RightPart.Contains(token) && x.Symbol != token);

            for (int i = 0; i < grammarRules.Count; i++)
            {
                var grammarRule = grammarRules[i];

                int idx = grammarRule.RightPart.IndexOf(token);

                if (idx == grammarRule.RightPart.Count - 1 || ((idx == grammarRule.RightPart.Count - 2) && (GrammarRules.IndexOf(grammarRule) == 0)))
                {
                    if (token != grammarRule.Symbol)
                    {
                        dirSymbols.AddRange(Follow(grammarRule.Symbol));
                        if ((idx == grammarRule.RightPart.Count - 2) && (GrammarRules.IndexOf(grammarRule) == 0))
                            dirSymbols.Add(EndSymbol);

                        continue;
                    }
                }
                
                if (idx != grammarRule.RightPart.Count - 1)
                {
                    string symbol = grammarRule.RightPart[idx + 1];
                    if (TokenIsNonTerminal(symbol))
                    {
                        List<Rule> gramRules = GrammarRules.FindAll(x => x.Symbol == symbol && x.Symbol != grammarRule.Symbol);
                        for (int j = 0; j < gramRules.Count; j++)
                            dirSymbols.AddRange(FindDirectionSymbolsForToken(GrammarRules.IndexOf(gramRules[j])));
                    }
                    else if(symbol == EmptySymbol)
                    {
                        dirSymbols.AddRange(Follow(grammarRule.Symbol));
                    }
                    else
                    {
                        dirSymbols.Add(symbol);
                    }
                }
            }

            return dirSymbols;
        }

        bool TokenIsNonTerminal(string token)
        {
            foreach (Rule grammarRule in GrammarRules)
            {
                if (grammarRule.Symbol == token)
                {
                    return true;
                }
            }
            return false;
        }

        private List<string> ParseChainSymbols(string str)
        {
            List<string> result = [];

            string accumulated = "";
            foreach (char ch in str)
            {
                if ((ch == ' ' || ch == StartTokenCh) && accumulated.Length > 0)
                {
                    result.Add(accumulated);
                    accumulated = ch == StartTokenCh ? ch.ToString() : "";
                }
                else if (ch == EndTokenCh && accumulated.Length > 1 && _tokens.Contains(accumulated[1..]))
                {
                    result.Add(accumulated[1..]);
                    accumulated = "";
                }
                else if (ch != ' ')
                {
                    accumulated += ch;
                }
            }

            if(accumulated != "")
            {
                result.Add(accumulated);
            }

            return result;
        }
        private void ParseTokens()
        {
            foreach (string line in _lines)
            {
                int tokenEndPos = line.IndexOf(EndTokenCh);
                if (!line.StartsWith(StartTokenCh) || tokenEndPos <= 1)
                {
                    throw new Exception("Wrong token format");
                }
                string token = line[1..tokenEndPos];
                _tokens.Add(token);
            }
        }
    }
}
