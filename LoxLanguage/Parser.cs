/*
 *  That is the grammar being implemented:
 *  
 *   expression     → equality ;
 *   equality       → comparison ( ( "!=" | "==" ) comparison )* ;
 *   comparison     → term ( ( ">" | ">=" | "<" | "<=" ) term )* ;
 *   term           → factor ( ( "-" | "+" ) factor )* ;
 *   factor         → unary ( ( "/" | "*" ) unary )* ;
 *   unary          → ( "!" | "-" ) unary
 *                  | primary ;
 *   primary        → NUMBER | STRING | "true" | "false" | "nil"
 *                  | "(" expression ")" ;
 *
 */

namespace LoxLanguage {
    internal class ParseError : SystemException { }
    internal class Parser {
        private readonly List<Token> Tokens;
        private int Current = 0;    // Current token being consumed

        public Parser(List<Token> tokens) {
            this.Tokens = tokens;
        }

        public Expr? Parse() {
            try {
                return Expression();
            }
            catch (ParseError) {
                return null;
            }
        }

        // expression → equality ;
        private Expr Expression() {
            return Equality();
        }

        // equality → comparison ( ( "!=" | "==" ) comparison )* ;
        private Expr Equality() {
            Expr expr = Comparison();

            while(Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL)) {
                Token oper = Previous();
                Expr right = Comparison();
                expr = new Binary(expr, oper, right);
            }

            return expr;
        }

        // comparison → term ( ( ">" | ">=" | "<" | "<=" ) term )* ;
        private Expr Comparison() {
            Expr expr = Term();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL)) {
                Token oper = Previous();
                Expr right = Term();
                expr = new Binary(expr, oper, right);
            }

            return expr;
        }

        // term → factor ( ( "-" | "+" ) factor )* ;
        private Expr Term () {
            Expr expr = Factor();

            while (Match(TokenType.MINUS, TokenType.PLUS)) {
                Token oper = Previous();
                Expr right = Factor();
                expr = new Binary(expr, oper, right);
            }

            return expr;
        }

        // factor → unary ( ( "/" | "*" ) unary )* ;
        private Expr Factor() {
            Expr expr = Unary();

            while (Match(TokenType.SLASH, TokenType.STAR)) {
                Token oper = Previous();
                Expr right = Unary();
                expr = new Binary(expr, oper, right);
            }

            return expr;
        }

        /*
         * unary          → ( "!" | "-" ) unary
         *                  | primary ;          
         */
        private Expr Unary() {
            if (Match(TokenType.BANG, TokenType.MINUS)) {
                Token oper = Previous();
                Expr right = Unary();
                return new Unary(oper, right);
            }

            return Primary();
        }

        /* *   primary        → NUMBER | STRING | "true" | "false" | "nil"
           *                  | "(" expression ")" ;
           */
        private Expr Primary() {
            if (Match(TokenType.FALSE)) return new Literal(false);
            if (Match(TokenType.TRUE)) return new Literal(true);
            if (Match(TokenType.NIL)) return new Literal(null);

            if (Match(TokenType.NUMBER, TokenType.STRING)) {
                return new Literal(Previous().Literal);
            }

            if (Match(TokenType.LEFT_PAREN)) {
                Expr expr = Expression(); // Calling recursively a new expression
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Grouping(expr);
            }

            throw Error(Peek(), "Expect expression.");
        }

        private Token Consume(TokenType type, string message) {
            if (Check(type)) return Advance();

            throw Error(Peek(), message);
        }

        private ParseError Error(Token token, string message) {
            Lox.Error(token, message);
            return new ParseError();
        }

        private void Synchronize() {
            Advance();

            while (!IsAtEnd()) {
                if (Previous().Type == TokenType.SEMICOLON) return;

                switch (Peek().Type) {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                Advance();
            }
        }

        private bool Match(params TokenType[] types) {
            foreach (var type in types) {
                if (Check(type)) {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private bool Check(TokenType type) {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private Token Advance() {
            if (!IsAtEnd()) Current++;
            return Previous();
        }

        private bool IsAtEnd() {
            return Peek().Type == TokenType.EOF;
        }

        private Token Peek() {
            return Tokens[Current];
        }

        private Token Previous() {
            return Tokens[Current - 1];
        }
    }
}
