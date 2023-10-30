using System.Text.RegularExpressions;

namespace NetEti.ExpressionParser
{
    /// <summary>
    /// Allgemeine Syntaxelemente, die in einem logischen Ausdruck
    /// vorkommen können.
    /// </summary>
    public enum SyntaxElement
    {
        /// <summary>Ein Operand</summary>
        NONE = 0,
        /// <summary>Eine öffnende Klammer</summary>
        GROUP = 1,
        /// <summary>Eine schließende Klammer</summary>
        UNGROUP = 2,
        /// <summary>Ein logischer oder arithmetischer Operator</summary>
        OPERATOR = 3,
        /// <summary>Linker Operand für einen Operator (Teil-Ausdruck)</summary>
        LEFT = 4,
        /// <summary>Rechter Operand für einen Operator (Teil-Ausdruck)</summary>
        RIGHT = 5,
        /// <summary>Priorisierter Unter-Ausdruck (geklammerter Ausdruck)</summary>
        STRUCT = 6
    }

    /// <summary>
    /// Ein Parser für allgemeine Ausdrücke.
    /// Erstellt aus einem Textausdruck einen äquivalenten Baum.
    /// Muss abgeleitet werden; in der Ableitung muss dann die Syntax definiert werden.
    /// <see cref="BooleanParser"/>
    /// <see cref="ArithmeticParser"/>
    /// <see cref="LogicalParser"/>
    /// </summary>
    /// <remarks>
    /// File: ExpressionParser.cs
    /// Autor: Erik Nagel, NetEti
    ///
    /// 10/2012 Erik Nagel: erstellt
    /// 03/2013 Erik Nagel: LogicalParser erstellt (boolesche plus Vergleichsoperatoren);
    ///                     Fehler beim Auswerten mehrwertiger Ausdrücke behoben.
    /// 06/2013 Erik Nagel: Endlosloop bei Einzelvariable ohne Operator abgefangen.
    /// 09.07.2016 Erik Nagel: Es werden jetzt auch User-Variablen, die "_" enthalten,
    /// korrekt verarbeitet.
    /// </remarks>
    public abstract class ExpressionParser
    {
        /// <summary>
        /// Liste von jeweils mehrern möglichen Text-Token mit jeweils
        /// einem zugeordneten internen Schlüssel. <see cref="BooleanParser"/>
        /// </summary>
        public Dictionary<string, List<string>>? Token { get; set; }

        /// <summary>
        /// Liste von jeweils ein oder zwei möglichen Operanden mit jeweils
        /// einem zugeordneten Operator. <see cref="BooleanParser"/>
        /// </summary>
        public Dictionary<string, List<SyntaxElement>>? Operators { get; set; }

        /// <summary>
        /// Liste von Operatoren mit ihren relativen Prioritäten.
        /// <see cref="BooleanParser"/>
        /// </summary>
        public Dictionary<string, int>? OperatorPriority { get; set; }


        /// <summary>
        /// Liste von höherwertigen Operatoren mit zugeordneten Unterausdrücken.
        /// <see cref="BooleanParser"/>
        /// </summary>
        public Dictionary<string, string>? MetaRules { get; set; }

        /// <summary>
        /// Der ursprünglich übergebene Text-Ausdruck.
        /// </summary>
        public string? ExpressionString { get; private set; }

        private SyntaxTree? syntaxTree;
        private KeyValuePair<string, List<string>>[]? preorderedToken = null;
        private Dictionary<string, List<SyntaxTree>>? preParsedMetaRules = null;

        /// <summary>
        /// Überführt einen Textausdruck anhand vorgegebener Regeln
        /// in einen äquivalenten Syntaxbaum.
        /// </summary>
        /// <param name="expr">Logischer Textausdruck.</param>
        /// <returns>SyntaxTree des geparsten logischen Textausdrucks.</returns>
        public SyntaxTree Parse(string expr)
        {
            if (this.preorderedToken == null)
            {
                this.preorderedToken = this.preorderToken();
            }
            if (this.preParsedMetaRules == null)
            {
                this.preParseMetaRules();
                if (this.preParsedMetaRules != null)
                {
                    foreach (List<SyntaxTree> children in this.preParsedMetaRules.Values)
                    {
                        this.priorityToChildren(children);
                    }
                }
            }
            this.ExpressionString = expr;
            this.syntaxTree = new SyntaxTree("ROOT",
                                             SyntaxElement.STRUCT,
                                             null,
                                             this.preParse(this.ExpressionString));
            if (this.syntaxTree.Children != null)
            {
                this.priorityToChildren(this.syntaxTree.Children);
                if (this.preParsedMetaRules != null)
                {
                    this.preparsedMetaRulesToChildren();
                }
            }
            this.syntaxTree.Parse();
            return this.syntaxTree;
        }

        /// <summary>
        /// Parst und retourniert alle Namen, die nicht zu den für diesen Parsertyp definierten Operatoren gehören.
        /// </summary>
        /// <param name="expression">Der zu parsende Ausdruck.</param>
        /// <returns>Liste aller Operanden.</returns>
        public List<string> GetOperands(string expression)
        {
            KeyValuePair<string, List<string>>[]? preorderedToken = this.preorderToken();
            if (preorderedToken == null)
            {
                return new List<string>();
            }
            foreach (KeyValuePair<string, List<string>> token in preorderedToken)
            {
                foreach (string tokenValue in token.Value)
                {
                    if (Regex.Matches(tokenValue, "^[a-z0-9_ ]+$", RegexOptions.IgnoreCase).Count > 0)
                    {
                        expression = Regex.Replace(expression, @"\b" + tokenValue + @"\b", " ", RegexOptions.IgnoreCase);
                    }
                    else
                    {
                        if (Regex.Matches(tokenValue, "[a-z0-9_ ]+", RegexOptions.IgnoreCase).Count == 0)
                        {
                            string premaskedOp = Regex.Replace(tokenValue, "", "\\").TrimEnd('\\');
                            expression = Regex.Replace(expression, premaskedOp, " ", RegexOptions.IgnoreCase);
                        }
                        else
                        {
                            throw new ArgumentException("Operatoren dürfen nicht aus Sonderzeichen und [a-z0-9_ ] gemischt werden.");
                        }
                    }
                }
            }
            List<string> operands = new List<string>(Regex.Split(expression, @"\s+")).Where(a => { return !String.IsNullOrEmpty(a); }).ToList();
            return operands;
        }

        private void preParseMetaRules()
        {
            this.preParsedMetaRules = new Dictionary<string, List<SyntaxTree>>();
            if (this.MetaRules != null)
            {
                foreach (string key in this.MetaRules.Keys)
                {
                    if (this.Operators?.Keys.Contains(key) == true)
                    {
                        this.preParsedMetaRules.Add(key, this.preParse(this.MetaRules[key]));
                    }
                }
            }
        }

        private void preparsedMetaRulesToChildren()
        {
            bool metaRuleFound;
            do
            {
                metaRuleFound = false;
                for (int i = 0; i < this.syntaxTree?.Children?.Count; i++)
                {
                    SyntaxTree child = this.syntaxTree.Children[i];
                    if (this.MetaRules?.ContainsKey(child.NodeName) == true)
                    {
                        metaRuleFound = true;
                        List<SyntaxTree> rulePart = this.preParsedMetaRules?[child.NodeName] ?? new List<SyntaxTree>();
                        List<SyntaxTree> replacement = new List<SyntaxTree>();
                        int leftReplaced = i;
                        int rightReplaced = i;
                        for (int j = 0; j < rulePart.Count; j++)
                        {
                            switch (rulePart[j].NodeType)
                            {
                                case SyntaxElement.LEFT:
                                    if (i > 0)
                                    {
                                        int bracketCount = 0;
                                        int k = i;
                                        while (--k >= 0)
                                        {
                                            if (this.syntaxTree.Children[k].NodeType == SyntaxElement.UNGROUP)
                                            {
                                                bracketCount++;
                                            }
                                            if (this.syntaxTree.Children[k].NodeType == SyntaxElement.GROUP)
                                            {
                                                bracketCount--;
                                            }
                                            if (bracketCount == 0)
                                            {
                                                List<SyntaxTree> left = this.syntaxTree.Children.GetRange(k, i - k);
                                                replacement.AddRange(left);
                                                leftReplaced = k;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        throw new FormatException("Es wurde kein linker Operand für den Operator " + child.NodeName + " gefunden.");
                                    }
                                    break;
                                case SyntaxElement.RIGHT:
                                    if (i < this.syntaxTree.Children.Count - 1)
                                    {
                                        int bracketCount = 0;
                                        int k = i;
                                        while (++k < this.syntaxTree.Children.Count)
                                        {
                                            if (this.syntaxTree.Children[k].NodeType == SyntaxElement.GROUP)
                                            {
                                                bracketCount++;
                                            }
                                            if (this.syntaxTree.Children[k].NodeType == SyntaxElement.UNGROUP)
                                            {
                                                bracketCount--;
                                            }
                                            if (bracketCount == 0)
                                            {
                                                List<SyntaxTree> right = this.syntaxTree.Children.GetRange(i + 1, k - i);
                                                replacement.AddRange(right);
                                                rightReplaced = k;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        throw new FormatException("Es wurde kein rechter Operand für den Operator " + child.NodeName + " gefunden.");
                                    }
                                    break;
                                default:
                                    replacement.Add(rulePart[j]);
                                    break;
                            }
                        } // for (int j = 0; j < rulePart.Count; j++)
                        this.syntaxTree.Children.RemoveRange(leftReplaced, rightReplaced - leftReplaced + 1);
                        // tiefe Kopie übertragen
                        i = leftReplaced + replacement.Count;
                        foreach (SyntaxTree elem in replacement)
                        {
                            this.syntaxTree.Children.Insert(leftReplaced++, new SyntaxTree(elem.NodeName, elem.NodeType, this.syntaxTree, null));
                        }
                        break;
                    } // if (this.MetaRules.ContainsKey(child.NodeType))
                } // for (int i = 0; i < children.Count; i++)
            } while (metaRuleFound);
        }

        private void priorityToChildren(List<SyntaxTree> children)
        {
            if (this.OperatorPriority != null)
            {
                foreach (KeyValuePair<string, int> prio in this.OperatorPriority.OrderBy(c => c.Value).ToList())
                {
                    for (int j = 0; j < children.Count; j++)
                    {
                        if (children[j].NodeName.Equals(prio.Key))
                        {
                            string actOperator = children[j].NodeName;
                            int leftBorder = j - 1;
                            int rightBorder = j + 1;
                            if (this.Operators?[actOperator].Contains(SyntaxElement.LEFT) == true)
                            {
                                leftBorder = findLeftBorder(children, j, prio.Value);
                            }
                            if (this.Operators?[actOperator].Contains(SyntaxElement.RIGHT) == true)
                            {
                                rightBorder = findRightBorder(children, j, prio.Value);
                            }
                            if (rightBorder > j + 1 || leftBorder < j - 1)
                            {
                                if (rightBorder > children.Count - 1)
                                {
                                    children.Add(new SyntaxTree("UNGROUP", SyntaxElement.UNGROUP, this.syntaxTree, null));
                                }
                                else
                                {
                                    children.Insert(rightBorder, new SyntaxTree("UNGROUP", SyntaxElement.UNGROUP, this.syntaxTree, null));
                                }
                                children.Insert(leftBorder + 1, new SyntaxTree("GROUP", SyntaxElement.GROUP, this.syntaxTree, null));
                                j++;
                            }
                        }
                    }
                }
            }
        }

        private int findLeftBorder(List<SyntaxTree> children, int position, int priority)
        {
            int border = position;
            int bracketCount = 0;
            do
            {
                if (border < 0)
                {
                    return border;
                }
                SyntaxElement nodeType = children[border].NodeType;
                string nodeName = children[border].NodeName;
                switch (nodeType)
                {
                    case SyntaxElement.GROUP:
                        if (--bracketCount < 0)
                        {
                            return border;
                        }
                        break;
                    case SyntaxElement.UNGROUP:
                        bracketCount++;
                        break;
                    default:
                        if (bracketCount == 0)
                        {
                            if ((this.OperatorPriority?.Keys.Contains(nodeName) == true) && this.OperatorPriority[nodeName] > priority)
                            {
                                return border;
                            }
                        }
                        break;
                }
                border--;
            } while (true);
        }

        private int findRightBorder(List<SyntaxTree> children, int position, int priority)
        {
            int border = position;
            int bracketCount = 0;
            do
            {
                if (border >= children.Count)
                {
                    return border;
                }
                SyntaxElement nodeType = children[border].NodeType;
                string nodeName = children[border].NodeName;
                switch (nodeType)
                {
                    case SyntaxElement.UNGROUP:
                        if (--bracketCount < 0)
                        {
                            return border;
                        }
                        break;
                    case SyntaxElement.GROUP:
                        bracketCount++;
                        break;
                    default:
                        if (bracketCount == 0)
                        {
                            if ((this.OperatorPriority?.Keys.Contains(nodeName) == true) && this.OperatorPriority[nodeName] > priority)
                            {
                                return border;
                            }
                        }
                        break;
                }
                border++;
            } while (true);
        }

        internal void show(string indent)
        {
            this.syntaxTree?.Show(indent);
        }

        internal void showFlat()
        {
            this.syntaxTree?.ShowFlat();
            Console.WriteLine();
        }

        private List<SyntaxTree> preParse(string expr)
        {
            List<SyntaxTree> children = new List<SyntaxTree>();
            // alle Token und Namen in eine Stringliste separieren
            string expr1 = expr;
            for (int i = 0; i < this.preorderedToken?.Length; i++)
            {
                string se = this.preorderedToken[i].Key;
                if (Token != null)
                {
                    foreach (string op in Token[se])
                    {
                        if (Regex.Matches(op, "^[a-z0-9_ ]+$", RegexOptions.IgnoreCase).Count > 0)
                        {
                            expr1 = Regex.Replace(expr1, @"\b" + op + @"\b", " _" + se.ToString() + "_ ", RegexOptions.IgnoreCase);
                        }
                        else
                        {
                            if (Regex.Matches(op, "[a-z0-9_ ]+", RegexOptions.IgnoreCase).Count == 0)
                            {
                                string premaskedOp = Regex.Replace(op, "", "\\").TrimEnd('\\');
                                expr1 = Regex.Replace(expr1, premaskedOp, " _" + se.ToString() + "_ ", RegexOptions.IgnoreCase);
                            }
                            else
                            {
                                throw new ArgumentException("Operatoren dürfen nicht aus Sonderzeichen und [a-z0-9_ ] gemischt werden.");
                            }
                        }
                    }
                }
            }
            //foreach (Match n in new Regex(@"\w+", RegexOptions.IgnoreCase).Matches(expr1))
            foreach (Match n in new Regex(@"\S+", RegexOptions.IgnoreCase).Matches(expr1))
            {
                foreach (Group g in n.Groups)
                {
                    string tokenName = g.Value.Trim('_');
                    SyntaxElement se;
                    if (!Enum.TryParse(tokenName, false, out se))
                    {
                        se = SyntaxElement.NONE;
                    }
                    if (this.Operators?.Keys.Contains(tokenName) == true)
                    {
                        if (this.Operators[tokenName][0] == SyntaxElement.GROUP || this.Operators[tokenName][0] == SyntaxElement.UNGROUP)
                        {
                            se = (SyntaxElement)this.Operators[tokenName][0];
                        }
                        else
                        {
                            se = SyntaxElement.OPERATOR;
                        }
                    }
                    else
                    {
                        // Es handelt sich um eine User-Variable => Originalname wiederherstellen.
                        tokenName = g.Value; // 09.07.2016 Nagel+-
                    }
                    children.Add(new SyntaxTree(tokenName, se, this.syntaxTree, null));
                }
            }
            return children;
        }

        // Sortiert alle Token so vor, dass zuerst die Token kommen,
        // die ihrerseits wieder andere Token enthalten.
        // Dies vermeidet Interpretationsfehler, z.B. muss das Token
        // '<=' vor den Token '<' und '=' gesucht und maskiert werden,
        // damit es nur als '<=' interpretiert wird und nicht zusätzlich
        // auch noch als '<' und/oder '='.
        private KeyValuePair<string, List<string>>[]? preorderToken()
        {
            if (this.Token == null)
            {
                return null;
            }
            KeyValuePair<string, List<string>>[] preorderedToken = this.Token.ToArray();
            bool swapped = false;
            do
            {
                for (int i = 0; i < preorderedToken.Length; i++)
                {
                    swapped = false;
                    Dictionary<string, bool> alreadySwapped = new Dictionary<string, bool>();
                    for (int j = i + 1; j < preorderedToken.Length; j++)
                    {
                        if (preorderedToken[j].Key != preorderedToken[i].Key)
                        {
                            for (int k = 0; k < preorderedToken[i].Value.Count; k++)
                            {
                                for (int l = 0; l < preorderedToken[j].Value.Count; l++)
                                {
                                    if (preorderedToken[j].Value[l].Contains(preorderedToken[i].Value[k]))
                                    {
                                        string key = preorderedToken[j].Key.CompareTo(preorderedToken[i].Key) < 0 ?
                                          preorderedToken[j].Key + ":" + preorderedToken[i].Key :
                                          preorderedToken[i].Key + ":" + preorderedToken[j].Key;
                                        if (alreadySwapped.ContainsKey(key))
                                        {
                                            throw new ArgumentException("Token-Strings containing each other vice versa!");
                                        }
                                        KeyValuePair<string, List<string>> swapper = preorderedToken[i];
                                        preorderedToken[i] = preorderedToken[j];
                                        preorderedToken[j] = swapper;
                                        swapped = true;
                                        j = i;
                                        alreadySwapped.Add(key, true);
                                    }
                                }
                            }
                        }
                    }
                }
            } while (swapped);
            return preorderedToken;
        } // private List<SyntaxTree> PreParse(string expr)
    } // public abstract class ExpressionParser
} // namespace NetEti.ExpressionParser
