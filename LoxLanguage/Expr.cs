namespace LoxLanguage {
    abstract class Expr {
        public abstract R Accept<R>(Visitor<R> visitor);
        public interface Visitor<R> {
            R VisitBinaryExpr(Binary expr);
            R VisitTernaryExpr(Ternary expr);
            R VisitGroupingExpr(Grouping expr);
            R VisitLiteralExpr(Literal expr);
            R VisitUnaryExpr(Unary expr);
            R VisitVariableExpr(Variable expr);
            R VisitAssignExpr(Assign expr);
            R VisitLogicalExpr(Logical expr);
            R VisitCallExpr(Call expr);
            R VisitGetExpr(Get expr);
            R VisitSetExpr(Set expr);
            R VisitThisExpr(This expr);
            R VisitSuperExpr(Super expr);
        }
        public class Assign : Expr {
            public Token Name { get; }
            public Expr Value { get; }
            public Assign(Token name, Expr value) {
                Name = name;
                Value = value;
            }
            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitAssignExpr(this);
            }
        }

        public class Binary : Expr {
            public Expr Left { get; }
            public Token Oper { get; }
            public Expr Right { get; }
            public Binary(Expr Left, Token Oper, Expr Right) {
                this.Left = Left;
                this.Oper = Oper;
                this.Right = Right;
            }
            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitBinaryExpr(this);
            }
        }
        public class Call : Expr {
            public Expr Callee { get; }
            public Token Paren { get; }
            public List<Expr> Arguments { get; }

            public Call(Expr callee, Token paren, List<Expr> arguments) {
                Callee = callee;
                Paren = paren;
                Arguments = arguments;
            }
            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitCallExpr(this);
            }
        }
        public class Get : Expr {
            public Expr Object { get; }
            public Token Name { get; }

            public Get (Expr obj, Token name) {
                Object = obj;
                Name = name;
            }
            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitGetExpr(this);
            }
        }
        public class Ternary : Expr {
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
        public class Grouping : Expr {
            public Expr Expression { get; }
            public Grouping(Expr Expression) {
                this.Expression = Expression;
            }

            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitGroupingExpr(this);
            }
        }
        public class Literal : Expr {
            public Object? Value { get; }
            public Literal(Object Value) {
                this.Value = Value;
            }

            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitLiteralExpr(this);
            }
        }
        public class Logical : Expr {
            public Expr Left { get; }
            public Token Oper { get; }
            public Expr Right { get; }
            public Logical(Expr Left, Token Oper, Expr Right) {
                this.Left = Left;
                this.Oper = Oper;
                this.Right = Right;
            }
            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitLogicalExpr(this);
            }
        }
        public class Set : Expr {
            public Expr Object { get; }
            public Token Name { get; }
            public Expr Value { get; }

            public Set (Expr obj, Token name, Expr value) {
                Object = obj;
                Name = name;
                Value = value;
            }
            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitSetExpr(this);
            }
        }
        public class Super : Expr {
            public Token Keyword { get; }
            public Token Method { get; }
            public Super (Token keyword, Token method) {
                Keyword = keyword;
                Method = method;
            }
            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitSuperExpr(this);
            }
        }
        public class This : Expr {
            public Token Keyword { get; }
            public This (Token keyword) {
                Keyword = keyword;
            }
            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitThisExpr(this);
            }
        }

        public class Unary : Expr {
            public Token Operator { get; }
            public Expr Right { get; }
            public Unary(Token Operator, Expr Right) {
                this.Operator = Operator;
                this.Right = Right;
            }

            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitUnaryExpr(this);
            }
        }

        public class Variable : Expr {
            public Token Name { get; }
            public Variable(Token name) {
                Name = name;
            }
            public override R Accept<R>(Visitor<R> visitor) {
                return visitor.VisitVariableExpr(this);
            }
        }

    }
}
