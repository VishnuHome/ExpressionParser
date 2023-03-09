using System;
using System.Collections.Generic;
using System.Linq;
using NetEti.ExpressionParser;


namespace NetEti.ExpressionParserDemo
{
    class Program
    {
        static BooleanParser booleanParser = new BooleanParser();
        static ArithmeticParser arithmeticalParser = new ArithmeticParser();
        static LogicalParser logicalParser = new LogicalParser();
        static TresholdParser tresholdParser = new TresholdParser();

        static void Main(string[] args)
        {

            
            
            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine("--- BOOLEAN                                                               ---");
            Console.WriteLine("-----------------------------------------------------------------------------");

            string expression1 = @"[[[( [ { a | BORD } & !{ c OR d } ] => [f >< go_h4ome ])]]]";
            List<string> operands = booleanParser.GetOperands(expression1);
            Console.WriteLine(expression1);
            Console.WriteLine("Operanden: " + String.Join(", ", operands));
            Console.ReadLine();

            expression1 = @"a < b | d <= f != x => b";
            operands = booleanParser.GetOperands(expression1);
            Console.WriteLine(expression1);
            Console.WriteLine("Operanden: " + String.Join(", ", operands));

            ParseAndShowBOOLEAN(@"a !>< b");
            ParseAndShowBOOLEAN(@"a !>< b !>< c");
            ParseAndShowLOGICAL(@"a");
            ParseAndShowLOGICAL(@"!a");
            ParseAndShowLOGICAL(@":a");

            ParseAndShowBOOLEAN(@"a !>< b !>< c !>< d");
            ParseAndShowBOOLEAN(@"[[[( [ { a | BORD } & !{ c OR d } ] => [f >< go_h4ome ])]]]");

            ParseAndShowBOOLEAN(@"![[[( [ { a | BORD } & !{ c OR d } ] => [f >< go_home ])]]]");
            ParseAndShowBOOLEAN(@"!{ a | BORD } & !{ c OR d } => [ f >< go_home ]");
            ParseAndShowBOOLEAN(@"!{ a | BORD } & !{ c OR { d & e} }");
            ParseAndShowBOOLEAN(@"! a | BORD");
            ParseAndShowBOOLEAN(@"a & b | c");
            ParseAndShowBOOLEAN(@"a | b & c");
            ParseAndShowBOOLEAN(@"a & ( b | c )");
            ParseAndShowBOOLEAN(@"(a | b) & c");

            ParseAndShowBOOLEAN(@"a | b & c | d");

            ParseAndShowBOOLEAN(@"a | !b & x imp ! c | d");
            ParseAndShowBOOLEAN(@"a | b & x imp c | d");
            ParseAndShowBOOLEAN(@"a | b imp c | d");
            ParseAndShowBOOLEAN(@"a | b => c | d");

            ParseAndShowBOOLEAN(@"a | b | f => c | d");
            ParseAndShowBOOLEAN(@"a | b => c & d & e | !z");

            ParseAndShowBOOLEAN(@"a | b");
            ParseAndShowBOOLEAN(@"a !& b !>< c");
            ParseAndShowBOOLEAN(@"a NAND b XNOR c");
            ParseAndShowBOOLEAN(@"a !| c");

            ParseAndShowBOOLEAN(@"Check_C AND Check_D");
            ParseAndShowBOOLEAN(@"Google AND Heise AND (Local OR Local_Backup)");
            ParseAndShowBOOLEAN(@"(Google AND Heise) AND (Local OR Local_Backup)");      

            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine("--- LOGICAL                                                               ---");
            Console.WriteLine("-----------------------------------------------------------------------------");

            string expression2 = @"[[[( [ { a | BORD } & !{ c OR d } ] => [f >< go_h4ome ])]]]";
            List<string> operands2 = logicalParser.GetOperands(expression2);
            Console.WriteLine(expression2);
            Console.WriteLine("Operanden: " + String.Join(", ", operands2));
            expression2 = @"a < b | d >= f != x => b";
            operands2 = logicalParser.GetOperands(expression2);
            Console.WriteLine(expression2);
            Console.WriteLine("Operanden: " + String.Join(", ", operands2));
            ParseAndShowLOGICAL(expression2);
            Console.WriteLine();

            expression2 = @"a & b & c & d & e";
            operands2 = logicalParser.GetOperands(expression2);
            Console.WriteLine(expression2);
            Console.WriteLine("Operanden: " + String.Join(", ", operands2));
            ParseAndShowLOGICAL(expression2);
            Console.WriteLine();
            ParseAndShowLOGICAL(@"a => b");

            ParseAndShowLOGICAL(@"a | b");

            ParseAndShowLOGICAL(@"a < b | c > d");
            ParseAndShowLOGICAL(@"a < b | c > d = e <= f");
            ParseAndShowLOGICAL(@"a < b | c != d");
            ParseAndShowLOGICAL(@"a < b | d <= f != x => b");
            ParseAndShowLOGICAL(@"a");
            ParseAndShowLOGICAL(@"A");
            ParseAndShowLOGICAL(@"IS (DialogChecker)");
            ParseAndShowLOGICAL(@"IS (DialogChecker & Dummy)");
            ParseAndShowLOGICAL(@"IS DialogChecker_");
            ParseAndShowLOGICAL(@"IS DialogChecker");
            
            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine("--- ARITHMETICAL                                                          ---");
            Console.WriteLine("-----------------------------------------------------------------------------");
            ParseAndShowARITHMETICAL(@"a + b * c - d");
            Console.ReadLine();

            ParseAndShowARITHMETICAL(@"a * b + c / d");
            ParseAndShowARITHMETICAL(@"a / b / c / d");
            

            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine("--- TRESHOLD                                                              ---");
            Console.WriteLine("-----------------------------------------------------------------------------");

            string expression3 = @"a TR50 b TR50 c tr50 d tr50 e | x";
            List<string>? operands3 = tresholdParser.GetOperands(expression3);
            Console.WriteLine(expression3);
            Console.WriteLine("Operanden: " + String.Join(", ", operands3));

            ParseAndShowTRESHOLD(expression3);

            Console.ReadLine();

        }

        static void ParseAndShow(string expr, NetEti.ExpressionParser.ExpressionParser parser)
        {
            SyntaxTree tree = parser.Parse(expr);
            Console.WriteLine(expr);
            List<string> list = tree.Show("   ");
            list.TakeWhile(c => { Console.WriteLine(c); return true; }).ToList();
            Console.WriteLine(tree.ShowFlat());
            Console.WriteLine();
        }

        static void ParseAndShowBOOLEAN(string expr)
        {
            ParseAndShow(expr, booleanParser);
            Console.WriteLine();
        }

        static void ParseAndShowARITHMETICAL(string expr)
        {
            ParseAndShow(expr, arithmeticalParser);
            Console.WriteLine();
        }

        static void ParseAndShowLOGICAL(string expr)
        {
            ParseAndShow(expr, logicalParser);
            Console.WriteLine();
        }

        static void ParseAndShowTRESHOLD(string expr)
        {
            ParseAndShow(expr, tresholdParser);
            Console.WriteLine();
        }
    }
}
