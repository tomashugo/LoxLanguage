namespace LoxLanguage {
    abstract class Stmt {
        public abstract R Accept<R>(Visitor<R> visitor);
    
        public interface Visitor<R> {
            R VisitExpressionStmt(Expression stmt);
            R VisitPrintStmt(Print stmt);
        }
        public class Expression : Stmt {
            public Expr expr { get; }
            public Expression(Expr expression) {
                expr = expression;
            }

            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitExpressionStmt(this);
            }
        }
        public class Print : Stmt {
            public Expr Expression { get; }
            public Print (Expr Expression) {
                this.Expression = Expression;
            }

            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitPrintStmt(this);
            }
        }
    }
}
