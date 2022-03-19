namespace LoxLanguage {
    abstract class Expr {
       public abstract R Accept<R>(Visitor<R> visitor);
    }
    interface Visitor<R> {
       R VisitBinaryExpr(Binary expr);
       R VisitGroupingExpr(Grouping expr);
       R VisitLiteralExpr(Literal expr);
       R VisitUnaryExpr(Unary expr);
    }
    sealed class Binary : Expr {
        public Expr Left { get; }
        public Token Oper { get; }
        public Expr Right { get; }
        public Binary (Expr Left, Token Oper, Expr Right) {
            this.Left = Left;
            this.Oper = Oper;
            this.Right = Right;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitBinaryExpr(this);
        }
    }
    sealed class Grouping : Expr {
        public Expr Expression { get; }
        public Grouping (Expr Expression) {
            this.Expression = Expression;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitGroupingExpr(this);
        }
    }
    sealed class Literal : Expr {
        public Object? Value { get; } 
        public Literal (Object Value) {
            this.Value = Value;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitLiteralExpr(this);
        }
    }
    sealed class Unary : Expr {
        public Token Operator { get; }
        public Expr Right { get; }
        public Unary (Token Operator, Expr Right) {
            this.Operator = Operator;
            this.Right = Right;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitUnaryExpr(this);
        }
    }
}
