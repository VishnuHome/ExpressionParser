namespace NetEti.ExpressionParser
{
    /// <summary>
    /// Ein Parser für boolesche Ausdrücke.
    /// Erstellt aus einem Textausdruck einen äquivalenten Baum.
    /// </summary>
    public class BooleanParser : ExpressionParser
  {
    /// <summary>
    /// Standard Konstruktor.
    /// </summary>
    public BooleanParser()
    {
      this.Token = new Dictionary<string, List<string>>()
      {
        {"GROUP", new List<string>() {"(","[","{"}},
        {"UNGROUP", new List<string>() {")","]","}"}},
        {"IS", new List<string>() {":", "IS"}},
        {"NOT", new List<string>() {"!", "NOT"}},
        {"AND", new List<string>() {"&","AND"}},
        {"XOR", new List<string>() {"><","XOR"}},
        {"OR", new List<string>() {"|","OR"}},
        {"IMP", new List<string>() {"=>","IMP"}}
        ,{"NAND", new List<string>() {"!&","NAND"}}
        ,{"NOR", new List<string>() {"!|","NOR"}}
        ,{"XNOR", new List<string>() {"!><","XNOR"}}
      };
      this.Operators = new Dictionary<string, List<SyntaxElement>>()
      {
        {"GROUP", new List<SyntaxElement>() {SyntaxElement.GROUP}},
        {"UNGROUP", new List<SyntaxElement>() {SyntaxElement.UNGROUP}},
        {"IS", new List<SyntaxElement>() {SyntaxElement.RIGHT}},
        {"NOT", new List<SyntaxElement>() {SyntaxElement.RIGHT}},
        {"AND", new List<SyntaxElement>() {SyntaxElement.LEFT, SyntaxElement.RIGHT}},
        {"XOR", new List<SyntaxElement>() {SyntaxElement.LEFT, SyntaxElement.RIGHT}},
        {"OR", new List<SyntaxElement>() {SyntaxElement.LEFT, SyntaxElement.RIGHT}},
        {"IMP", new List<SyntaxElement>() {SyntaxElement.LEFT, SyntaxElement.RIGHT}}
        ,{"NAND", new List<SyntaxElement>() {SyntaxElement.LEFT, SyntaxElement.RIGHT}}
        ,{"NOR", new List<SyntaxElement>() {SyntaxElement.LEFT, SyntaxElement.RIGHT}}
        ,{"XNOR", new List<SyntaxElement>() {SyntaxElement.LEFT, SyntaxElement.RIGHT}}
      };
      this.OperatorPriority = new Dictionary<string, int>() { { "IS", 5 }, { "NOT", 5 }, { "AND", 21 }, { "XOR", 22 }, { "OR", 23 }, { "IMP", 24 }, { "NAND", 25 }, { "XNOR", 26 }, { "NOR", 27 } };
      this.MetaRules = new Dictionary<string, string>()
      {
        {"IMP",  "!LEFT | RIGHT"}
        ,{"NAND",  "!(LEFT & RIGHT)"}
        ,{"NOR",  "!(LEFT | RIGHT)"}
        ,{"XNOR",  "!(LEFT >< RIGHT)"}
      };
    }
  } // public class SyntaxParser : SyntaxParser
} // namespace Parser
