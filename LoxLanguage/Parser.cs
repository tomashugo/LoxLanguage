﻿/*
 *  That is the grammar being implemented:
 *  
 *  
 *  
 *   program        → declaration* EOF ;
 *   declaration    → varDecl
 *                  | statement ;
 *   varDecl        → "var" IDENTIFIER ( "=" expression )? ";" ;
 *   statement      → exprStmt
 *                    | ifStmt 
 *                    | printStmt 
 *                    | block ;
 *   ifStmt         → "if" "(" expression ")" statement
 *                    ( "else" statement )? ;
 *   block          → "{" declaration* "}" ;
 *   exprStmt       → expression ";" ;
 *   printStmt      → "print" expression ";" ;
 *   expression     → assignment ;
 *   assignment     → IDENTIFIER "=" assignment
 *                  | logic_or ;
 *   logic_or       → logic_and ( "or" logic_and )* ;
 *   logic_and      → equality ( "and" equality )* ;
 *   equality       → comparison ( ( "!=" | "==" | "," ) comparison )* ;
 *   comparison     → term ( ( ">" | ">=" | "<" | "<=" ) term )* ;
 *   term           → factor ( ( "-" | "+" ) factor )* ;
 *   factor         → unary ( ( "/" | "*" ) unary )* ;
 *   unary          → ( "!" | "-" ) unary
 *                  | primary ;
 *   primary        → NUMBER | STRING | "true" | "false" | "nil"
 *                  | "(" expression ")" 
 *                  | IDENTIFIER ;
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

        public List<Stmt>? Parse() {
            List<Stmt> statements = new List<Stmt>();

            while(!IsAtEnd()) {
                statements.Add(Declaration());
            }

            return statements;            
        }

        // expression → equality ;
        private Expr Expression() {
            return Assignment();
        }

        private Stmt Declaration() {
            try {
                if (Match(TokenType.VAR)) return VarDeclaration();

                return Statement();
            }
            catch (ParseError error) {
                Synchronize();
                return null;
            }
        }

        private Stmt Statement() {
            if (Match(TokenType.IF)) return IfStatement();
            if (Match(TokenType.PRINT)) return PrintStatement();
            if (Match(TokenType.LEFT_BRACE)) return new Stmt.Block(Block());

            return ExpressionStatement();
        }

        private Stmt IfStatement() {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'");
            Expr condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

            Stmt thenBranch = Statement();
            Stmt elseBranch = null;

            if (Match(TokenType.ELSE)) {
                elseBranch = Statement();
            }

            return new Stmt.If(condition, thenBranch, elseBranch);  
        }

        private Stmt PrintStatement() {
            Expr value = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value");
            return new Stmt.Print(value);
        }

        private Stmt VarDeclaration() {
            Token name = Consume(TokenType.IDENTIFIER, "Exepct variable name");

            Expr initializer = null;
            if (Match(TokenType.EQUAL)) {
                initializer = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new Stmt.Var(name, initializer);
        }            

        private Stmt ExpressionStatement() {
            Expr expr = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after expression");
            return new Stmt.Expression(expr);
        }

        private List<Stmt> Block() {
            List<Stmt> statements = new List<Stmt> ();

            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd()) {
                statements.Add(Declaration());
            }

            Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }
        private Expr Assignment() {
            Expr expr = Or();

            if (Match(TokenType.EQUAL)) {
                Token equals = Previous();
                Expr value = Assignment();

                if (expr is Expr.Variable) {
                    Token name = ((Expr.Variable)expr).Name;
                    return new Expr.Assign(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private Expr Or() {
            Expr expr = And();

            while (Match(TokenType.OR)) {
                Token oper = Previous();
                Expr right = And();
                expr = new Expr.Logical(expr, oper, right);
            }

            return expr;
        }

        private Expr And() {
            Expr expr = Equality();

            while (Match(TokenType.AND)) {
                Token oper = Previous();
                Expr right = Equality();
                expr = new Expr.Logical(expr, oper, right);
            }

            return expr;
        }

        // equality → comparison ( ( "!=" | "==" | "," ) comparison )* ;
        private Expr Equality() {
            Expr expr = Comparison();

            if (Peek().Type == TokenType.COLON) {
                return expr;
            }

            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL, TokenType.COMMA)) {
                Token oper = Previous();
                Expr right = Comparison();
                expr = new Expr.Binary(expr, oper, right);
            }

            while (Match(TokenType.QUESTION_MARK)) {
                Token oper = Previous();
                Expr middle = Comparison();
                Token oper2 = Consume(TokenType.COLON, "Expect ':' after expression.");
                Expr right = Comparison();

                expr = new Expr.Ternary(expr, oper, middle, oper2, right);
            }

            return expr;
        }

        // comparison → term ( ( ">" | ">=" | "<" | "<=" ) term )* ;
        private Expr Comparison() {
            Expr expr = Term();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL)) {
                Token oper = Previous();
                Expr right = Term();
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        // term → factor ( ( "-" | "+" ) factor )* ;
        private Expr Term () {
            Expr expr = Factor();

            while (Match(TokenType.MINUS, TokenType.PLUS)) {
                Token oper = Previous();
                Expr right = Factor();
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        // factor → unary ( ( "/" | "*" ) unary )* ;
        private Expr Factor() {
            Expr expr = Unary();

            while (Match(TokenType.SLASH, TokenType.STAR)) {
                Token oper = Previous();
                Expr right = Unary();
                expr = new Expr.Binary(expr, oper, right);
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
                return new Expr.Unary(oper, right);
            }

            return Primary();
        }

        /* *   primary        → NUMBER | STRING | "true" | "false" | "nil"
           *                  | "(" expression ")" ;
           */
        private Expr Primary() {
            if (Match(TokenType.FALSE)) {
                return new Expr.Literal(false);
            }

            if (Match(TokenType.TRUE)) { 
                return new Expr.Literal(true);                
            }


            if (Match(TokenType.NIL)) {
                return new Expr.Literal(null);
            }

            if (Match(TokenType.NUMBER, TokenType.STRING)) {
                return new Expr.Literal(Previous().Literal);              
            }

            if (Match(TokenType.IDENTIFIER)) {
                return new Expr.Variable(Previous());
            }

            if (Match(TokenType.LEFT_PAREN)) {
                Expr expr = Expression(); // Calling recursively a new expression
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
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
