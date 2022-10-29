using System.Collections.Generic;
using System.Linq;

namespace NetEti.ExpressionParser
{
    /// <summary>
    /// Ein Parser mit der Fähigkeit, Schwellenwert-Operatoren aufzulösen.
    /// Abgeleitet vom Parser für erweiterte boolesche Ausdrücke.
    /// Es werden zusätzlich zu den in LogicalParser definierten
    /// Operatoren neun Operatoren für 10%, 20%, ... , 80%, 90% Schwellenwert eingeführt.
    /// Die Schwellenwert-Logik (treshold) prüft, ob ein bestimmter Prozentsatz der einem Job untergeordneter
    /// Knoten das Ergebnis true geliefert hat. Wird der durch den gewählten Operator festgelegte
    /// Prozentsatz (Schwellenwert) erreicht oder überschritten, geht der gesamte Job auf true (grüne Anzeige).
    /// Erstellt aus einem Textausdruck einen äquivalenten Baum.
    /// </summary>
    public class TresholdParser : LogicalParser
    {
        /// <summary>
        /// Standard Konstruktor.
        /// </summary>
        public TresholdParser()
          : base()
        {
            this.Token.Add("TR10", new List<string>() { "TR10" });
            this.Token.Add("TR20", new List<string>() { "TR20" });
            this.Token.Add("TR30", new List<string>() { "TR30" });
            this.Token.Add("TR40", new List<string>() { "TR40" });
            this.Token.Add("TR50", new List<string>() { "TR50" });
            this.Token.Add("TR60", new List<string>() { "TR60" });
            this.Token.Add("TR70", new List<string>() { "TR70" });
            this.Token.Add("TR80", new List<string>() { "TR80" });
            this.Token.Add("TR90", new List<string>() { "TR90" });
            this.Operators.Add("TR10", new List<SyntaxElement>() { SyntaxElement.LEFT, SyntaxElement.RIGHT });
            this.Operators.Add("TR20", new List<SyntaxElement>() { SyntaxElement.LEFT, SyntaxElement.RIGHT });
            this.Operators.Add("TR30", new List<SyntaxElement>() { SyntaxElement.LEFT, SyntaxElement.RIGHT });
            this.Operators.Add("TR40", new List<SyntaxElement>() { SyntaxElement.LEFT, SyntaxElement.RIGHT });
            this.Operators.Add("TR50", new List<SyntaxElement>() { SyntaxElement.LEFT, SyntaxElement.RIGHT });
            this.Operators.Add("TR60", new List<SyntaxElement>() { SyntaxElement.LEFT, SyntaxElement.RIGHT });
            this.Operators.Add("TR70", new List<SyntaxElement>() { SyntaxElement.LEFT, SyntaxElement.RIGHT });
            this.Operators.Add("TR80", new List<SyntaxElement>() { SyntaxElement.LEFT, SyntaxElement.RIGHT });
            this.Operators.Add("TR90", new List<SyntaxElement>() { SyntaxElement.LEFT, SyntaxElement.RIGHT });

            Dictionary<string, int> additionalOperatorsPriorities
              = new Dictionary<string, int>() { { "TR10", 21 }, { "TR20", 21 }, { "TR30", 21 }, { "TR40", 21 }, { "TR50", 21 }, { "TR60", 21 }, { "TR70", 21 }, { "TR80", 21 }, { "TR90", 21 } };
            // es gibt leider kein AddRange:
            additionalOperatorsPriorities.ToList<KeyValuePair<string, int>>().FirstOrDefault(kv => { this.OperatorPriority.Add(kv.Key, kv.Value); return false; });
            //this.MetaRules.Add("IMP", "!LEFT | RIGHT");
        }

    } // public class TresholdParser : LogicalParser
} // namespace Parser
