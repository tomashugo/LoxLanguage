namespace LoxLanguage {
    abstract class Stmt {
        public abstract R Accept<R>(Visitor<R> visitor);

        public interface Visitor<R> {
            R VisitExpressionStmt(Expression stmt);
            R VisitPrintStmt(Print stmt);
            R VisitVarStmt(Var stmt);
            R VisitBlockStmt(Block stmt);
            R VisitIfStmt(If stmt);
            R VisitWhileStmt(While stmt);
        }
        public class Block : Stmt {
            public List<Stmt> Statements;

            public Block(List<Stmt> statements) {
                Statements = statements;
            }
            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitBlockStmt(this);
            }
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
        public class If : Stmt {
            public Expr Condition { get; }
            public Stmt ThenBranch { get;  }
            public Stmt? ElseBranch { get; }

            public If (Expr condition, Stmt thenBranch, Stmt elseBranch) {
                Condition = condition;
                ThenBranch = thenBranch;
                ElseBranch = elseBranch;
            }
            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitIfStmt(this);
            }
        }
        public class Print : Stmt {
            public Expr Expression { get; }
            public Print(Expr Expression) {
                this.Expression = Expression;
            }

            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitPrintStmt(this);
            }
        }

        public class Var : Stmt {
            public Token Name { get; }
            public Expr Initializer { get; }

            public Var(Token name, Expr initializer) {
                Name = name;
                Initializer = initializer;
            }

            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitVarStmt(this);
            }
        }

        public class While : Stmt {
            public Expr Condition { get; }
            public Stmt Body { get; }

            public While(Expr condition, Stmt body ) {
                Condition = condition;
                Body = body;
            }

            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitWhileStmt(this);
            }
        }
    }
}
