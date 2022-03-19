namespace LoxLanguage {
    /* Some token implementations store the location as two numbers: the offset from the beginning of the source file to the beginning
     * of the lexeme, and the length of the lexeme.
     * 
     * The scanner needs to know these anyway, so there's no overhead to calculate them.
     * 
     * An offset can be converted to line and column positions later by looking back at the source file and counting the preceding newlines.
     * That sounds slow, and it is. However, you neet to do it _only when you need to actually display a line and a column to the user_.
     * 
     * Most tokens never appear in an error message. For those, the less time you spend calculating position information ahead of time, the better.
     */
    internal class Token {
        readonly TokenType Type;
        public string Lexeme { get; }
        readonly object Literal;
        readonly int Line;
        public Token (TokenType type, string lexeme, object literal, int line) {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }

        public new string ToString () {
            return new string($"{Type} {Lexeme} {Literal}");
        }
    }
}
