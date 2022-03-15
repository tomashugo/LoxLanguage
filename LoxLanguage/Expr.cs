namespace LoxLanguage {
    abstract class Expr {
    }
    sealed class Binary : Expr {
        readonly Expr Left;
        readonly Token Oper;
        readonly Expr Right;
        public Binary (Expr Left, Token Oper, Expr Right) {
            this.Left = Left;
            this.Oper = Oper;
            this.Right = Right;
        }
    }
    sealed class Grouping : Expr {
        readonly Expr Expression;
        public Grouping (Expr Expression) {
            this.Expression = Expression;
        }
    }
    sealed class Literal : Expr {
        readonly Object Value;
        public Literal (Object Value) {
            this.Value = Value;
        }
    }
    sealed class Unary : Expr {
        readonly Token Operator;
        readonly Expr Right;
        public Unary (Token Operator, Expr Right) {
            this.Operator = Operator;
            this.Right = Right;
        }
    }
}
