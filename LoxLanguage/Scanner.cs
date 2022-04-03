using System.Globalization;

namespace LoxLanguage {
    internal class Scanner {
        private readonly string Source;
        private readonly List<Token> Tokens = new List<Token>();
        private int Start = 0;
        private int Current = 0;
        private int Line = 1;

        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>() {
            { "and", TokenType.AND },
            { "class", TokenType.CLASS },
            { "else", TokenType.ELSE },
            { "false", TokenType.FALSE },
            { "for", TokenType.FOR },
            { "fun", TokenType.FUN },
            { "if", TokenType.IF },
            { "nil", TokenType.NIL },
            { "or", TokenType.OR },
            { "print", TokenType.PRINT },
            { "return", TokenType.RETURN },
            { "super", TokenType.SUPER },
            { "this", TokenType.THIS },
            { "true", TokenType.TRUE },
            { "var", TokenType.VAR },
            { "while", TokenType.WHILE }

        };
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
                case '?': AddToken(TokenType.QUESTION_MARK); break;
                case ':': AddToken(TokenType.COLON); break;
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '/':
                    // If comment it goes until the end of the line.
                    if (Match('/')) {
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    } else if (Match('*')) {
                        MultiLineComment();
                    } else {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace
                    break;
                case '\n':
                    Line++;
                    break;
                case '"':
                    String();
                    break;
                default:
                    if (IsDigit(c)) {
                        Number();
                    } else if (IsAlpha(c)) {
                        Identifier();
                    } else {
                        Lox.Error(Line, "Unexpected character.");
                    }
                    break;
            }
        }
        private void Identifier() {
            while (IsAlphaNumeric(Peek())) Advance();

            string text = Source.Substring(Start, Current - Start);
            
            TokenType type;

            if (!Keywords.ContainsKey(text)) type = TokenType.IDENTIFIER;
            else type = Keywords[text];

            AddToken(type);
        }
        // Start freezes inside this method and Current increment
        private void Number() {
            while (IsDigit(Peek())) Advance();

            // Look for a fractional part.

            // Remember to test if two "." is allowed
            if (Peek() == '.' && IsDigit(PeekNext())) {
                // Consume the "."
                Advance();

                while (IsDigit(Peek())) Advance();                
            }

            IFormatProvider provider;
            provider = CultureInfo.CreateSpecificCulture("en-US");
            
            AddToken(TokenType.NUMBER, double.Parse(Source.Substring(Start, Current - Start), provider));
        }
        // Likewise Number()
        private void String() {
            while (Peek() != '"' && !IsAtEnd()) {
                if (Peek() == '\n') Line++;
                Advance();
            }

            if (IsAtEnd()) {
                Lox.Error(Line, "Unterminated string.");
                return;
            }

            Advance();

            string value = Source.Substring(Start + 1, Current - Start - 2);
            AddToken(TokenType.STRING, value);
        }
        private void MultiLineComment() {
            while (!IsAtEnd() && !(Peek() == '*' && PeekNext() == '/')) {                
                if (Peek() == '\n') {
                    Line++;                    
                }                
                Advance();
            }
         
            Advance();
            Advance();            
        }
        private char Peek() {
            if (IsAtEnd()) return '\0';
            return Source.ElementAt(Current);
        }
        private char PeekNext() {
            if (Current + 1 > Source.Length) return '\0';
            return Source.ElementAt(Current + 1);
        }
        private bool IsAlpha(char c) {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
        }
        private bool IsAlphaNumeric(char c) {
            return IsAlpha(c) || IsDigit(c);
        }
        private bool IsDigit(char c) {
            return c >= '0' && c <= '9';
        }

        // Current variable is incremented only if Match to the argument
        private bool Match(char expected) {
            if (IsAtEnd()) return false;
            if (Source.ElementAt(Current) != expected) return false;

            Current++;

            return true;
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
            string text = Source.Substring(Start, Current - Start);
            Tokens.Add(new Token(type, text, literal, Line));
        }
    }
}
