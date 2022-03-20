namespace LoxLanguage {
    abstract class Expr {
       public abstract R Accept<R>(Visitor<R> visitor);
    }
    interface Visitor<R> {
       R VisitBinaryExpr(Binary expr);
       R VisitTernaryExpr(Ternary expr);
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
    sealed class Ternary : Expr {
        public Expr Left { get; }
        public Expr Middle { get; }
        public Expr Right { get; }
        public Token Oper { get; }
        public Token Oper2 { get; }

        public Ternary(Expr left, Token oper, Expr middle, Token oper2, Expr right) {
            this.Left = left;
            this.Middle = middle;
            this.Right = right;
            this.Oper = oper;
            this.Oper2 = oper2;
        }

        public override R Accept<R>(Visitor<R> visitor) {
            return visitor.VisitTernaryExpr(this);
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
