namespace LoxLanguage {
    internal class Scanner {
        private readonly string Source;
        private readonly List<Token> Tokens = new List<Token>();
        private int Start = 0;
        private int Current = 0;
        private int Line = 1;
        public Scanner(string source) {
            Source = source;
        }

        public List<Token> ScanTokens() {
            while (!IsAtEnd()) {
                Start = Current;
                ScanToken();
            }

            Tokens.Add(new Token(TokenType.EOF, "", null, Line));
            return Tokens;
        }

        private void ScanToken() {
            char c = Advance();

            switch (c) {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                default:
                    Lox.Error(Line, "Unexpected character.");
                    break;
            }
        }

        private bool IsAtEnd() {
            return Current >= Source.Length;
        }        

        private char Advance() {
            return Source.ElementAt(Current++);
        }
        private void AddToken(TokenType type) {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal) {
            string text = Source.Substring(Start, Current);
            Tokens.Add(new Token(type, text, literal, Line));
        }
    }
}
