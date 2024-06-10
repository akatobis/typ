using System.Text.RegularExpressions;
using CreateLL1Table.Models;

namespace CreateLL1Table.FileWork;

public class ReadGrammar
{
    public class FileParser(string fileName, bool directionSymbolsExistsInFile)
    {
        public readonly List<Rule> GrammarRules = new List<Rule>();

        private const char StartTokenCh = '<';
        private const char EndTokenCh = '>';
        private const int LineSeparationLength = 3;
        private const string EmptySymbol = "e";
        private const string EndSymbol = "#";
        private const string ReplacedMark = "1";

        private readonly string[] _lines = ReadFile(fileName);

        private readonly List<string> _tokens = new ();

        private static string[] ReadFile(string fileName)
        {
            var fileStream = File.OpenRead(fileName);
            var result = new List<string>();
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

            for (var i = 0; i < _lines.Length; i++)
            {
                Rule grammarRule = new Rule(_tokens[i], new List<string>(), new List<string>());

                var startPos = _tokens[i].Length + 3 + LineSeparationLength;
                var line = _lines[i][startPos..];

                grammarRule.RightPart = ParseRightPart(line);
                
                GrammarRules.Add(grammarRule);
            }
            GrammarRules.Insert(0, 
                new Rule(ReplacedMark + GrammarRules[0].Symbol,
                    new List<string>{GrammarRules[0].Symbol, EndSymbol}, 
                    new List<string>{}));

            FixLeftRecursive();
            
            if (!directionSymbolsExistsInFile)
            {
                FindDirectionSymbolsByRules();
            }
        }

        private void FixLeftRecursive()
        {
            var rulesWithLeftRecursion = GrammarRules.FindAll(HasLeftRecursion);
            var rulesPassed = new List<Rule>();
            foreach (var grammarRule in rulesWithLeftRecursion)
            {
                RemoveLeftRecursion(grammarRule, rulesPassed);
                rulesPassed.Add(grammarRule);
            }
        }

        private void RemoveLeftRecursion(Rule rule, List<Rule> rulesPassed)
        {
            if (!HasLeftRecursion(rule)) 
                return;
            
            var newToken = rule.Symbol + ReplacedMark;

            var rules = GrammarRules.FindAll(x => x.Symbol == rule.Symbol && !HasLeftRecursion(x));

            if (rules.Count == 0)
            {
                throw new Exception("Не убирается левая рекурсия. Сам делай");
            }

            var newRuleForRemoveLeftRecursion = new Rule(newToken, new(rule.RightPart.GetRange(1, rule.RightPart.Count - 1)), new List<string>());
            newRuleForRemoveLeftRecursion.RightPart.Add(newToken);

            GrammarRules[GrammarRules.IndexOf(rule)] = newRuleForRemoveLeftRecursion;

            if(rulesPassed.FindAll(x => x.Symbol == rule.Symbol).Count > 0)
            {
                return;
            }

            var ruleWithoutLeftRecursion = new Rule(rules[0].Symbol, new List<string>(), new List<string>());
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
                    newRule = new Rule(rule.Symbol, new List<string>(), new List<string>());
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

            Rule epsilonRule = new Rule(newToken, new List<string>(){"e"}, new List<string>());

            GrammarRules.Insert(GrammarRules.IndexOf(GrammarRules.FindLast(x => x.Symbol == newToken))+1, epsilonRule);
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
                    grammarRule.GuideSet.AddRange(FindGuideSetForToken(index));
                }
            }
        }

        private List<string> FindGuideSetForToken(int tokenIdx)
        {
            var grammarRule = GrammarRules[tokenIdx];
            var firstChainCharacter = grammarRule.RightPart[0];

            if (TokenIsNonTerminal(firstChainCharacter))
            {
                List<string> result = new List<string>();
                for (int i = 0; i < GrammarRules.Count; i++)
                {
                    if (GrammarRules[i].Symbol == firstChainCharacter && i != tokenIdx)
                    {
                        result.AddRange(FindGuideSetForToken(i));
                    }
                }
                
                return result.Distinct().ToList();
            }
           
            
            return grammarRule.RightPart.Contains(EmptySymbol) 
                ? Follow(grammarRule.Symbol).Distinct().ToList()
                : new List<string>{firstChainCharacter};
        }

        List<string> Follow(string token)
        {
            var dirSymbols = new List<string>();

            var grammarRules = GrammarRules.FindAll(x => x.RightPart.Contains(token) && x.Symbol != token);

            foreach (var grammarRule in grammarRules)
            {
                var idx = grammarRule.RightPart.IndexOf(token);

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
                    var symbol = grammarRule.RightPart[idx + 1];
                    if (TokenIsNonTerminal(symbol))
                    {
                        var rule = grammarRule;
                        var gramRules = GrammarRules.FindAll(x => x.Symbol == symbol && x.Symbol != rule.Symbol);
                        foreach (var gramRule in gramRules)
                            dirSymbols.AddRange(FindGuideSetForToken(GrammarRules.IndexOf(gramRule)));
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
            foreach (var grammarRule in GrammarRules)
            {
                if (grammarRule.Symbol == token)
                {
                    return true;
                }
            }
            return false;
        }

        private List<string> ParseRightPart(string str)
        {
            var result = new List<string>();

            var accumulated = "";
            foreach (var ch in str)
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
            foreach (var line in _lines)
            {
                var tokenEndPos = line.IndexOf(EndTokenCh);
                if (!line.StartsWith(StartTokenCh) || tokenEndPos <= 1)
                {
                    throw new Exception("Не верный формат токена");
                }
                var token = line[1..tokenEndPos];
                _tokens.Add(token);
            }
        }
    }
}
