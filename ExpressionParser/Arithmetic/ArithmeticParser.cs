using System.Collections.Generic;

namespace NetEti.ExpressionParser
{
    /// <summary>
    /// Ein Parser für arithmetische Ausdrücke (experimentell).
    /// Erstellt aus einem Textausdruck einen äquivalenten Baum.
    /// </summary>
    public class ArithmeticParser : ExpressionParser
  {
    /// <summary>
    /// Standard Konstruktor.
    /// </summary>
    public ArithmeticParser()
    {
      this.Token = new Dictionary<string, List<string>>()
      {
        {"GROUP", new List<string>() {"(","[","{"}},
        {"UNGROUP", new List<string>() {")","]","}"}},
        {"NEGATIVE", new List<string>() {"!","NEGATIVE"}}, // für Vorzeichenumkehr
        {"PLUS", new List<string>() {"+","PLUS"}},
        {"MINUS", new List<string>() {"-","MINUS"}},
        {"MULT", new List<string>() {"*","MULTIPLIED"}},
        {"DIV", new List<string>() {"/","DIVIDED"}},
        {"POW", new List<string>() {"^","POWERED"}},
        {"SQR", new List<string>() {"v","SQARED"}}
      };
      this.Operators = new Dictionary<string, List<SyntaxElement>>()
      {
        {"GROUP", new List<SyntaxElement>() {SyntaxElement.GROUP}},
        {"UNGROUP", new List<SyntaxElement>() {SyntaxElement.UNGROUP}},
        {"NEGATIVE", new List<SyntaxElement>() {SyntaxElement.RIGHT}},
        {"PLUS", new List<SyntaxElement>() {SyntaxElement.LEFT, SyntaxElement.RIGHT}},
        {"MINUS", new List<SyntaxElement>() {SyntaxElement.LEFT, SyntaxElement.RIGHT}},
        {"MULT", new List<SyntaxElement>() {SyntaxElement.LEFT, SyntaxElement.RIGHT}},
        {"DIV", new List<SyntaxElement>() {SyntaxElement.LEFT, SyntaxElement.RIGHT}},
        {"POW", new List<SyntaxElement>() {SyntaxElement.LEFT, SyntaxElement.RIGHT}},
        {"SQR", new List<SyntaxElement>() {SyntaxElement.LEFT, SyntaxElement.RIGHT}}
      };
      this.OperatorPriority = new Dictionary<string, int>() { {"NEGATIVE", 1},  {"SQR", 2}, {"POW", 2}, {"DIV", 3}, {"MULT", 3}, {"MINUS", 4}, {"PLUS", 4} };
      this.MetaRules = new Dictionary<string, string>()
      {
        // {"BLA",  "LEFT PLUS RIGHT"}
      };
    }

  } // public class SyntaxParser : SyntaxParser
} // namespace Parser
