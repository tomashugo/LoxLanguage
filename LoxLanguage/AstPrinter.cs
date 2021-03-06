using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxLanguage {
    internal class AstPrinter : Expr.Visitor<string> {
        public string Print(Expr expr) {
            return expr.Accept(this);
        }
        public string VisitBinaryExpr(Expr.Binary expr) {
            return Parenthesize(expr.Oper.Lexeme, expr.Left, expr.Right);
        }

        public string VisitTernaryExpr(Expr.Ternary expr) {
            return Parenthesize($"{expr.Oper.Lexeme}{expr.Oper2.Lexeme}", expr.Left, expr.Middle, expr.Right);
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

            sb.Append("(").Append(name);
            foreach (var expr in exprs) {
                sb.Append(" ");
                sb.Append(expr.Accept(this));
            }
            sb.Append(")");

            return sb.ToString();
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

        public string VisitThisExpr(Expr.This thisobj) {
            throw new NotImplementedException();
        }

        public string VisitSuperExpr(Expr.Super expr) {
            throw new NotImplementedException();
        }

        //public static void Main(string[] args) {
        //    Expr expression = new Binary(
        //        new Unary (
        //            new Token(TokenType.MINUS, "-", null, 1),
        //            new Literal(123)),
        //        new Token(TokenType.STAR, "*", null, 1),
        //        new Grouping(new Literal(45.67))
        //    );

        //    Console.WriteLine(new AstPrinter().Print(expression));
        //}
    }
}
