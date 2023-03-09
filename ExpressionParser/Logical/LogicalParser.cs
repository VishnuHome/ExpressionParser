using System.Collections.Generic;
using System.Linq;

namespace NetEti.ExpressionParser
{
    /// <summary>
    /// Ein Parser für erweiterte boolesche Ausdrücke.
    /// Es werden zusätzlich zu den in BooleanParser definierten
    /// Operatoren Operatoren für 'kleiner', 'gleich' und 'größer' berücksichtigt.
    /// Aus Bequemlichkeitsgründen werden darüber hinaus auch noch Operatoren
    /// für 'ungleich', 'kleiner gleich' und 'größer gleich' geparst.
    /// Erstellt aus einem Textausdruck einen äquivalenten Baum.
    /// </summary>
    public class LogicalParser : BooleanParser
    {
        /// <summary>
        /// Standard Konstruktor.
        /// </summary>
        public LogicalParser()
          : base()
        {
            if (this.Token != null)
            {
                this.Token.Add("GE", new List<string>() { ">=" });
                this.Token.Add("EQ", new List<string>() { "==", "=" });
                this.Token.Add("LT", new List<string>() { "<" });
                this.Token.Add("GT", new List<string>() { ">" });
                this.Token.Add("NE", new List<string>() { "<>", "!=" });
                this.Token.Add("LE", new List<string>() { "<=" });
            }
            if (this.Operators != null)
            {
                this.Operators.Add("LT", new List<SyntaxElement>() { SyntaxElement.LEFT, SyntaxElement.RIGHT });
                this.Operators.Add("LE", new List<SyntaxElement>() { SyntaxElement.LEFT, SyntaxElement.RIGHT });
                this.Operators.Add("NE", new List<SyntaxElement>() { SyntaxElement.LEFT, SyntaxElement.RIGHT });
                this.Operators.Add("EQ", new List<SyntaxElement>() { SyntaxElement.LEFT, SyntaxElement.RIGHT });
                this.Operators.Add("GE", new List<SyntaxElement>() { SyntaxElement.LEFT, SyntaxElement.RIGHT });
                this.Operators.Add("GT", new List<SyntaxElement>() { SyntaxElement.LEFT, SyntaxElement.RIGHT });
            }
            Dictionary<string, int> additionalOperatorsPriorities
              = new Dictionary<string, int>() { { "LT", 10 }, { "LE", 10 }, { "GE", 10 }, { "GT", 10 }, { "NE", 11 }, { "EQ", 11 } };
            // es gibt leider kein AddRange:
            additionalOperatorsPriorities.ToList<KeyValuePair<string, int>>().FirstOrDefault(kv => { this.OperatorPriority?.Add(kv.Key, kv.Value); return false; });
            //this.MetaRules.Add("IMP", "!LEFT | RIGHT");
        }

    } // public class LogicalParser : BooleanParser
} // namespace Parser
