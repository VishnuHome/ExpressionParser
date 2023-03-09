using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NetEti.Globals;

namespace NetEti.ExpressionParser
{
    /// <summary>
    /// Generischer Tree mit geparstem logischem oder arithmetischen Ausdruck.
    /// Funktion: Wertet Ausdrücke aus und erstellt passende Syntax-Bäume.
    /// </summary>
    /// <remarks>
    /// File: SyntaxTree.cs
    /// Autor: Erik Nagel, NetEti
    ///
    /// 17.10.2012 Erik Nagel, NetEti: erstellt.
    /// 09.07.2016 Erik Nagel, NetEti: Bei fehlendem Operator wird der Identitätsoperator "IS" eingefügt.
    /// </remarks>
    public class SyntaxTree : GenericTree<SyntaxTree>
    {
        /// <summary>
        /// Der Name des Knotens.
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// Der Typ des Knotens.
        /// </summary>
        public SyntaxElement NodeType { get; private set; }

        private string indent = "";
        private string? flatFormula;
        private List<string> treeStringList = new List<string>();
        private int depth;

        internal SyntaxTree(string nodeName, SyntaxElement nodeType, SyntaxTree? mother, List<SyntaxTree>? children) : base(mother)
        {
            this.NodeName = nodeName;
            if (children != null)
            {
                this.Children = children;
            }
            this.NodeType = nodeType;
        }

        /// <summary>
        /// Erstellt aus einem Textausdruck mit vorgeparsten Token einen
        /// äquivalenten Syntaxbaum.
        /// </summary>
        public void Parse()
        {
            int bracketCount = 0;
            bool zeroBracketLevelInExpr = false;
            SyntaxElement firstToken = SyntaxElement.NONE;
            // TokenList in Logik-Tree abbilden
            do
            {
                if (this.Children == null || this.Children.Count < 1)
                {
                    throw new FormatException("Leerer Ausdruck");
                }
                int lastAnchor = -1;
                int i = 0;
                for (i = 0; i < this.Children?.Count; i++)
                {
                    SyntaxElement se = this.Children[i].NodeType;
                    string name = this.Children[i].NodeName;
                    if (i == 0)
                    {
                        firstToken = se;
                    }
                    switch (se)
                    {
                        case SyntaxElement.GROUP:
                            if (bracketCount == 0)
                            {
                                lastAnchor = i - 1;
                            }
                            bracketCount++;
                            break;
                        case SyntaxElement.UNGROUP:
                            bracketCount--;
                            if (bracketCount < 0)
                            {
                                throw new FormatException("Schließende ohne öffnende Klammer");
                            }
                            break;
                        default:
                            break;
                    }
                    if (bracketCount == 0)
                    {
                        if (i < Children.Count - 1)
                        {
                            zeroBracketLevelInExpr = true;
                        }
                        if (se != SyntaxElement.GROUP && se != SyntaxElement.UNGROUP)
                        {
                            if (se != SyntaxElement.NONE)
                            {
                                if (i - lastAnchor > 2)
                                {
                                    List<SyntaxTree> sub = new List<SyntaxTree>();
                                    // tiefe Kopie erzeugen
                                    foreach (SyntaxTree elem in this.Children.GetRange(lastAnchor + 1, i - lastAnchor - 1))
                                    {
                                        sub.Add(new SyntaxTree(elem.NodeName, elem.NodeType, this, null));
                                    }
                                    this.Children[i - 1].NodeName = "STRUCT";
                                    this.Children[i - 1].NodeType = SyntaxElement.STRUCT;
                                    this.Children[i - 1].Children = sub;
                                    this.Children.RemoveRange(lastAnchor + 1, i - lastAnchor - 2);
                                    i = lastAnchor;
                                }
                                else
                                {
                                    lastAnchor = i - 1;
                                }
                            }
                            else
                            {
                                lastAnchor = i;
                            }
                        } // if (se != SyntaxElement.GROUP && se != SyntaxElement.UNGROUP)
                    }
                }
                if (bracketCount > 0)
                {
                    throw new FormatException("Öffnende ohne schließende Klammer");
                }
                if (!zeroBracketLevelInExpr && firstToken == SyntaxElement.GROUP)
                {
                    Children?.RemoveAt(0);
                    Children?.RemoveAt(Children.Count - 1);
                }
                else
                {
                    if (i - lastAnchor > 2)
                    {
                        List<SyntaxTree> sub = new List<SyntaxTree>();
                        // tiefe Kopie erzeugen
                        if (this.Children != null)
                        {
                            foreach (SyntaxTree elem in this.Children.GetRange(lastAnchor + 1, i - lastAnchor - 1))
                            {
                                sub.Add(new SyntaxTree(elem.NodeName, elem.NodeType, this, null));
                            }
                            this.Children[i - 1].NodeName = "STRUCT";
                            this.Children[i - 1].NodeType = SyntaxElement.STRUCT;
                            this.Children[i - 1].Children = sub;
                            this.Children.RemoveRange(lastAnchor + 1, i - lastAnchor - 2);
                        }
                    }
                }
                // this.ShowFlat();
            } while (!zeroBracketLevelInExpr && this.Children?.Count > 1); // solange durchlaufen, bis überflüssige äußere Klammern weg sind.
                                                                           // 09.07.2016 Nagel+: Bei fehlendem Operator wird der Identitätsoperator "IS" eingefügt.
            if (this.Children?.Count > 0 && this.Children.Where(c => c.NodeType == SyntaxElement.OPERATOR).FirstOrDefault() == null)
            {
                SyntaxTree insertedOperator = new SyntaxTree("IS", SyntaxElement.OPERATOR, null, null);
                this.Children.Insert(0, insertedOperator);
            }
            // 09.07.2016 Nagel-
            if (this.Children != null)
            {
                foreach (SyntaxTree item in Children.Where(c => c.NodeType == SyntaxElement.STRUCT))
                {
                    item.Parse();
                }
            }
        }

        /// <summary>
        /// Stellt den SyntaxTree als Baum-Darstellung in eine StringList.
        /// </summary>
        /// <param name="indent">Zeichenkette für die Einrückung der Ebenen (normalerweise mehrere Leerzeichen).</param>
        /// <returns>StringList mit der mehrzeiligen Baum-Darstellung.</returns>
        public List<string> Show(string indent)
        {
            this.indent = indent;
            this.treeStringList.Clear();
            this.Traverse(this.element2StringList);
            return this.treeStringList;
        }

        /// <summary>
        /// Stellt den SyntaxTree als normalisierten Text-Ausdruck dar.
        /// </summary>
        /// <returns>Normalisierter Text-Ausdruck für den SyntaxTree.</returns>
        public string ShowFlat()
        {
            this.flatFormula = "";
            this.depth = 0;
            this.Traverse(this.buildFlatFormula);
            while (this.depth-- > 1)
            {
                this.flatFormula = String.Concat(this.flatFormula.TrimEnd(' '), " )");
            }
            return this.flatFormula;
        }

        private void element2StringList(int depth, SyntaxTree node)
        {
            string depthIndent = String.Join(this.indent, new string[++depth]);
            this.treeStringList.Add(String.Concat(depthIndent, node.NodeName));
        }

        private void buildFlatFormula(int depth, SyntaxTree node)
        {
            if (depth > this.depth && depth > 1)
            {
                this.flatFormula = String.Concat(this.flatFormula, "( ");
            }
            while (depth < this.depth)
            {
                this.flatFormula = String.Concat(this.flatFormula?.TrimEnd(' '), " ) ");
                this.depth--;
            }
            if (!node.NodeType.Equals(SyntaxElement.STRUCT))
            {
                this.flatFormula = String.Concat(this.flatFormula, node.NodeName, " ");
            }
            this.depth = depth;
        }

    }
}
