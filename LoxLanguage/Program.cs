/*
 *  Keeping track
 *  
 *  06/03/2022 20:27 - stopped at 4.5.2
 * 
 */

using System.Text;

namespace LoxLanguage
{
    internal class Lox {        
        static bool HadError; // I did this on my own
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
        }
        private static void RunPrompt() {            
            TextReader reader = Console.In;

            for (;;) {
                Console.Write("> ");
                string line = reader.ReadLine();
                Console.WriteLine(line);

                if (line == null) break;
                Run(line);
                HadError = false;
            }
        }

        private static void Run (string source) {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            foreach (var token in tokens) {
                Console.WriteLine(token);
            }
        }

        public static void Error(int line, string message) {
            Report(line, "", message);
        }

        static void Report(int line, string where, string message) {
            Console.WriteLine($"[line {line}] Error {where}: {message}");
            HadError = true;
        }
    }
}