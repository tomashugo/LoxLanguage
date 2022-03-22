/*
 *  Keeping track
 *  
 *  06/03/2022 20:27 - stopped at 4.5.2
 *  13/03/2022 21:55 - stopped at 5.2.1
 *  15/03/2022 18:46 - stopped at 5.3
 */

using System.Text;

namespace LoxLanguage
{
    internal class Lox {        
        private static Interpreter interpreter = new Interpreter();
        static bool HadError = false; // I did this on my own
        static bool HadRunTimeError = false;
        public static void Main(string[] args) {
            if (args.Length > 1) {
                Console.WriteLine("Usage: jlox [script]");
                Environment.Exit(64);
            } else if (args.Length == 1) {
                RunFile(args[0]);
            } else {
                RunPrompt();
            }
        }
        private static void RunFile(string path) {
            byte[] bytes = File.ReadAllBytes(path);
            char[] chars = Encoding.Default.GetChars(bytes); // using System.Text;

            Run(new string(chars));

            if (HadError) Environment.Exit(65);
            if (HadRunTimeError) Environment.Exit(70);
        }
        private static void RunPrompt() {            
            TextReader reader = Console.In;

            for (;;) {
                Console.Write("> ");
                #pragma warning disable CS8600 // Conversão de literal nula ou possível valor nulo em tipo não anulável.
                string line = reader.ReadLine();
                #pragma warning restore CS8600 // Conversão de literal nula ou possível valor nulo em tipo não anulável.
                //Console.WriteLine(line);

                if (line == null) break;
                Run(line);
                HadError = false;
            }
        }

        private static void Run (string source) {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            Parser parser = new Parser(tokens);

            try {
                List<Stmt> statements = parser.Parse();
                if (HadError) return;

                interpreter.Interpret(statements);
            }
            catch (ParseError parseError) {
                Console.WriteLine(parseError.Message);
            }


            //Console.WriteLine(new AstPrinter().Print(expression));

            //foreach (var token in tokens) {                
            //    Console.WriteLine(token.ToString());
            //    //Console.WriteLine(token.GetType);
            //}
        }

        public static void Error(int line, string message) {
            Report(line, "", message);
        }

        public static void Error(Token token, string message) {
            if (token.Type == TokenType.EOF) {
                Report(token.Line, " at end", message);
            }
            else {
                Report(token.Line, " at '" + token.Lexeme + "'", message);
            }
        }

        public static void RuntimeError(RuntimeError error) {
            Console.WriteLine("[line " + error.Tk.Line + "] " + error.Message);
            HadRunTimeError = true;
        }        

        static void Report(int line, string where, string message) {
            Console.WriteLine($"[line {line}] Error {where}: {message}");
            HadError = true;
        }
    }
}