using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxLanguage {
    internal class AstPrinter : Visitor<string> {
        string Print(Expr expr) {
            return expr.Accept(this);
        }
        public string VisitBinaryExpr(Binary expr) {
            return Parenthesize(expr.Oper.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(Grouping expr) {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteralExpr(Literal expr) {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString();
        }

        public string VisitUnaryExpr(Unary expr) {
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
    }
}
