namespace LoxLanguage {
    internal class Interpreter : Expr.Visitor<Object>, Stmt.Visitor<Object> {
        public Environment Globals = new Environment();
        private Environment Env;
        private Dictionary<Expr, int> Locals = new Dictionary<Expr, int> ();

        internal class TimeUtils {
            private static readonly DateTime Jan1st1970 = new DateTime
            (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            public static long CurrentTimeMillis() {
                return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
            }
        }
        internal class ClockDefinition : LoxCallable {
            public int Arity() { return 0; }
            public object Call(Interpreter interpreter, List<object> arguments) {
                return (double)TimeUtils.CurrentTimeMillis();
            }
            public string toString() { return "<native fn>";  }
        }

        public Interpreter() {
            Env = Globals;            

            // native function
            Globals.Define("clock", new ClockDefinition());
        }

        public object VisitBinaryExpr(Expr.Binary expr) {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);

            switch (expr.Oper.Type) {
                case TokenType.GREATER:
                    CheckNumberOperands(expr.Oper, left, right);
                    return Convert.ToDouble(left) > Convert.ToDouble(right);

                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.Oper, left, right);
                    return Convert.ToDouble(left) >= Convert.ToDouble(right);

                case TokenType.LESS:
                    CheckNumberOperands(expr.Oper, left, right);
                    return Convert.ToDouble(left) < Convert.ToDouble(right);

                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.Oper, left, right);
                    return Convert.ToDouble(left) <= Convert.ToDouble(right);

                case TokenType.MINUS:
                    CheckNumberOperands(expr.Oper, left, right);
                    return Convert.ToDouble(left) - Convert.ToDouble(right);                    

                case TokenType.SLASH:
                    CheckNumberOperands(expr.Oper, left, right);                    

                    if (Convert.ToDouble(right) == 0.0) {
                        throw new RuntimeError(expr.Oper, "Division by constant zero.");
                    }
                    return Convert.ToDouble(left) / Convert.ToDouble(right);

                case TokenType.STAR:
                    CheckNumberOperands(expr.Oper, left, right);
                    return Convert.ToDouble(left) * Convert.ToDouble(right);

                case TokenType.PLUS:
                    if (left is double && right is double) return Convert.ToDouble(left) + Convert.ToDouble(right);
                    if ((left is double && right is string) 
                        || (left is string && right is double) 
                        || (left is string && right is string)) return Convert.ToString(left) + Convert.ToString(right);


                    throw new RuntimeError(expr.Oper, "Operands (" + left + ", " + right + ") must be numbers or strings.");                    
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);

                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);
            }

            return null;
        }

        public object VisitCallExpr(Expr.Call expr) {
            object callee = Evaluate(expr.Callee);

            List<object> args = new List<object>();

            /*
             * This is another one of those subtle semantic
             * choices. Since argument expressions may
             * have side effects, the order they are evaluated
             * could be user visible. Even so, some
             * languages like Scheme and C don't specify an
             * order. This gives compilers freedom to
             * reorder them for efficiency, but means users
             * may be unpleasantly surprised if arguments
             * aren't evaluated in order they expect.
             * 
             */

            foreach (var argument in expr.Arguments) {
                args.Add(Evaluate(argument));
            }

            if (!(callee is LoxCallable)) {
                throw new RuntimeError(expr.Paren, "Can only call functions and classes");            
            }
            LoxCallable function = (LoxCallable)callee;

            if (args.Count != function.Arity()) {
                throw new RuntimeError(expr.Paren, "Expected " + function.Arity() + " arguments but got " + args.Count + ".");
            }

            
            return function.Call(this, args);
        }
        public object VisitGetExpr(Expr.Get expr) {
            object obj = Evaluate(expr.Object);
            if (obj is LoxInstance) {
                return ((LoxInstance)obj).Get(expr.Name);
            }

            throw new RuntimeError(expr.Name, "Only instances have properties.");
        }
        public void Interpret(List<Stmt> statements) {
            try {
                foreach (var stmt in statements) {
                    Execute(stmt);
                }
            }
            catch (RuntimeError error) {
                Lox.RuntimeError(error);
            }            
        }

        public object VisitGroupingExpr(Expr.Grouping expr) {
            return Evaluate(expr.Expression);
        }

        private object Evaluate(Expr expr) {
            return expr.Accept(this);
        }

        private void Execute(Stmt stmt) {
            stmt.Accept(this);
        }

        public void Resolve(Expr expr, int depth) {
            Locals.Add(expr, depth);
        }        
        public void ExecuteBlock(List<Stmt> statements, Environment environment) {
            Environment previous = Env;

            try {
                Env = environment;

                foreach (var statement in statements) {
                    Execute(statement);
                }
            }
            finally {
                Env = previous; 
            }
        }
        public object VisitClassStmt(Stmt.Class stmt) {
            Env.Define(stmt.Name.Lexeme, null);

            Dictionary<string, LoxFunction> methods = new Dictionary<string, LoxFunction>();
            
            foreach (var method in stmt.Methods) {
                LoxFunction function = new LoxFunction(method, Env, method.Name.Lexeme.Equals("init"));
                methods.Add(method.Name.Lexeme, function);
            }

            LoxClass klass = new LoxClass(stmt.Name.Lexeme, methods);
            Env.Assign(stmt.Name, klass);
            return null;
        }
        public object VisitBlockStmt(Stmt.Block stmt) {
            ExecuteBlock(stmt.Statements, new Environment(Env));
            return null;
        }

        public object VisitLiteralExpr(Expr.Literal expr) {
            return expr.Value;
        }

        public object VisitLogicalExpr(Expr.Logical expr) {
            object left = Evaluate(expr.Left);

            if (expr.Oper.Type == TokenType.OR) {
                if (IsTruthy(left)) return left;
            }
            else {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expr.Right);
        }
        public object VisitSetExpr(Expr.Set expr) {
            object obj = Evaluate(expr.Object);

            if (!(obj is LoxInstance)) {
                throw new RuntimeError(expr.Name, "Only instances have fields.");
            }

            object value = Evaluate(expr.Value);
            ((LoxInstance)obj).Set(expr.Name, value);
            return value;
        }
        public object VisitThisExpr(Expr.This expr) {
            return LookUpVariable(expr.Keyword, expr);
        }
        public object VisitTernaryExpr(Expr.Ternary expr) {            
            object left = Evaluate(expr.Left);
            object middle = Evaluate(expr.Middle);
            object right = Evaluate(expr.Right);

            Token oper = expr.Oper;
            Token oper2 = expr.Oper2;

            if (oper.Type == TokenType.QUESTION_MARK && oper2.Type == TokenType.COLON) {
                if (Convert.ToBoolean(left)) {
                    return middle;
                }
                else {
                    return right;
                }
            }

            return null;
        }

        public object VisitUnaryExpr(Expr.Unary expr) {
            object right = Evaluate(expr.Right);

            switch (expr.Operator.Type) {
                case TokenType.BANG:
                    return !IsTruthy(right);
                case TokenType.MINUS:
                    CheckNumberOperand(expr.Operator, right);
                    return -(double)right;
            }

            // Unreachable
            return null;
        }

        public object VisitVariableExpr(Expr.Variable expr) {
            return LookUpVariable(expr.Name, expr);
        }

        private object LookUpVariable(Token Name, Expr expr) {                         
            if (Locals.ContainsKey(expr)) {
                Locals.TryGetValue(expr, out int distance);
                return Env.GetAt(distance, Name.Lexeme);
            } else {
                return Globals.Get(Name);
            }
        }

        private void CheckNumberOperand(Token oper, object operand) {
            if (operand is double) return;
            throw new RuntimeError(oper, "Operand '" + operand + "' must be a number");
        }

        private void CheckNumberOperands(Token oper, object left, object right) {
            if (left is double && right is double) return;
            throw new RuntimeError(oper, "Operands (" + left + ", " + right + ") must be numbers.");
        }

        private bool IsTruthy(object defaultValue) {
            if (defaultValue == null) return false;
            if (defaultValue is bool) return Convert.ToBoolean(defaultValue);            
            return true;
        }

        private bool IsEqual (object a, object b) {
            if (a == null && b == null) return true;
            if (a == null) return false;

            return a.Equals(b);
        }

        private string Stringify(Object obj) {
            if (obj == null) return "nil";

            if (obj is double) {
                string text = Convert.ToString(obj);

                if (text.EndsWith(".0")) {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return Convert.ToString(obj);
        }        

        public object VisitExpressionStmt(Stmt.Expression stmt) {
            Evaluate(stmt.expr);
            return null;
        }

        public object VisitFunctionStmt(Stmt.Function stmt) {
            LoxFunction function = new LoxFunction(stmt, Env, false);
            Env.Define(stmt.Name.Lexeme, function);
            return null;
        }

        public object VisitIfStmt(Stmt.If stmt) {
            if (IsTruthy(Evaluate(stmt.Condition))) {
                Execute(stmt.ThenBranch);
            }
            else if (stmt.ElseBranch != null) {
                Execute(stmt.ElseBranch);
            }

            return null;
        }

        public object VisitPrintStmt(Stmt.Print stmt) {
            object value = Evaluate(stmt.Expression);
            Console.WriteLine(Stringify(value));
            return null;
        }

        public object VisitReturnStmt(Stmt.Return stmt) {
            object value = null;
            if (stmt.Value != null) value = Evaluate(stmt.Value);
            throw new Return(value);
        }

        public object VisitVarStmt(Stmt.Var stmt) {
            object value = null;

            if (stmt.Initializer != null) {
                value = Evaluate(stmt.Initializer);
            }

            Env.Define(stmt.Name.Lexeme, value);
            return null;
        }

        public object VisitWhileStmt(Stmt.While stmt) {
            while (IsTruthy(Evaluate(stmt.Condition))) {
                Execute(stmt.Body);
            }
            return null;
        }

        public object VisitAssignExpr(Expr.Assign expr) {
            Object value = Evaluate(expr.Value);

            Locals.TryGetValue(expr, out int distance);
            if (distance != null) {                
                Env.AssignAt(distance, expr.Name, value);
            }
            else {
                Globals.Assign(expr.Name, value);
            }
            return value;
        }
        
    }
}
