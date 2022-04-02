using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxLanguage {
    internal class ReversePolishNotation : Expr.Visitor<string> {
        string Print(Expr expr) {
            return expr.Accept(this);
        }
        public string VisitBinaryExpr(Expr.Binary expr) {
            return Parenthesize(expr.Oper.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(Expr.Grouping expr) {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteralExpr(Expr.Literal expr) {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString();
        }

        public string VisitUnaryExpr(Expr.Unary expr) {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);
        }

        private string Parenthesize(string name, params Expr[] exprs) {
            StringBuilder sb = new StringBuilder();

            foreach (var expr in exprs) {                
                sb.Append(expr.Accept(this));
                sb.Append(" ");
            }
            sb.Append(name);

            return sb.ToString();
        }

        public string VisitTernaryExpr(Expr.Ternary expr) {
            throw new NotImplementedException();
        }

        public string VisitExpressionStmt(Stmt stmt) {
            throw new NotImplementedException();
        }

        public string VisitPrintStmt(Stmt.Print prnt) {
            throw new NotImplementedException();
        }

        public string VisitVariableExpr(Expr.Variable expr) {
            throw new NotImplementedException();
        }

        public string VisitAssignExpr(Expr.Assign expr) {
            throw new NotImplementedException();
        }

        public string VisitLogicalExpr(Expr.Logical logical) {
            throw new NotImplementedException();
        }

        public string VisitCallExpr(Expr.Call call) {
            throw new NotImplementedException();
        }

        public string VisitGetExpr(Expr.Get getobj) {
            throw new NotImplementedException();
        }

        public string VisitSetExpr(Expr.Set setobj) {
            throw new NotImplementedException();
        }

        //public static void Main(string[] args) {
        //    Expr expression = new Binary(
        //        new Binary(new Literal(1), new Token(TokenType.PLUS, "+", null, 1), new Literal(2)), 
        //        new Token(TokenType.STAR, "*", null, 1),
        //        new Binary(new Literal(4), new Token(TokenType.MINUS, "-", null, 1), new Literal(3))
        //    );

        //    Console.WriteLine(new ReversePolishNotation().Print(expression));
        //}
    }
}
