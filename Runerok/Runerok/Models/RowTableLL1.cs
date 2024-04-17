﻿namespace Runerok.Models;

public class RowTableLL1
{
    public RowTableLL1()
    {
        
    }
    
    public RowTableLL1(string symbol, List<string> guideSet, bool error, int pointer, bool shift, bool toStack, bool end)
    {
        Symbol = symbol;
        GuideSet = guideSet;
        Error = error;
        Pointer = pointer;
        Shift = shift;
        ToStack = toStack;
        End = end;
    }
    
    public string Symbol { get; set; }
    public List<string> GuideSet { get; set; }
    public bool Error { get; set; }
    public int Pointer { get; set; }
    public bool Shift { get; set; }
    public bool ToStack { get; set; }
    public bool End { get; set; }
}