namespace LoxLanguage {
    class Resolver : Expr.Visitor<Object>, Stmt.Visitor<Object> {
        Interpreter interpreter;
        Stack<Dictionary<string, bool>> Scopes = new Stack<Dictionary<string, bool>>();
        FunctionType currentFunction = FunctionType.NONE;

        public Resolver(Interpreter interpreter) {
            this.interpreter = interpreter;
        }
        private enum FunctionType {
            NONE,
            FUNCTION,
            INITIALIZER,
            METHOD
        }
        private enum ClassType {
            NONE,
            CLASS,
            SUBCLASS
        }
        private ClassType CurrentClass = ClassType.NONE;
        public object VisitBlockStmt(Stmt.Block stmt) {
            BeginScope();
            Resolve(stmt.Statements);
            EndScope();
            return null;
        }
        public object VisitClassStmt(Stmt.Class stmt) {
            ClassType enclosingClass = CurrentClass;
            CurrentClass = ClassType.CLASS;

            Declare(stmt.Name);
            Define(stmt.Name);

            if (stmt.Superclass != null) {
                CurrentClass = ClassType.SUBCLASS;
                Resolve(stmt.Superclass);
            }

            if (stmt.Superclass != null && stmt.Name.Lexeme.Equals(stmt.Superclass.Name.Lexeme)) {
                Lox.Error(stmt.Superclass.Name, "A class can't inherit from itself.");
            }

            if (stmt.Superclass != null) {
                BeginScope();
                Scopes.Peek().Add("super", true);
            }


            BeginScope();
            Scopes.Peek().Add("this", true);

            foreach (Stmt.Function method in stmt.Methods) {
                FunctionType declaration = FunctionType.METHOD;
                if (method.Name.Lexeme.Equals("init")) {
                    declaration = FunctionType.INITIALIZER;
                }
                ResolveFunction(method, declaration);
            }

            EndScope();

            if (stmt.Superclass != null) EndScope();

            CurrentClass = enclosingClass;
            return null;
        }
        public object VisitExpressionStmt(Stmt.Expression stmt) {
            Resolve(stmt.expr);
            return null;
        }
        public object VisitFunctionStmt(Stmt.Function stmt) {
            Declare(stmt.Name);
            Define(stmt.Name);

            
            ResolveFunction(stmt, FunctionType.FUNCTION);
            return null;
        }
        public object VisitIfStmt(Stmt.If stmt) {
            Resolve(stmt.Condition);
            Resolve(stmt.ThenBranch);
            if (stmt.ElseBranch != null) Resolve(stmt.ElseBranch);
            return null;
        }
        public object VisitPrintStmt(Stmt.Print stmt) {
            Resolve(stmt.Expression);
            return null;
        }
        public object VisitReturnStmt(Stmt.Return stmt) {            
            if (currentFunction == FunctionType.NONE) {
                Lox.Error(stmt.Keyword, "Can't return from top-level code.");
            }

            if (stmt.Value != null) {
                if (currentFunction == FunctionType.INITIALIZER) {
                    Lox.Error(stmt.Keyword, "Can't return a value from a initializer");
                }

                Resolve(stmt.Value);
            }

            return null;
        }
        public void Resolve(List<Stmt> statements) {
            foreach (var statement in statements) {
                Resolve(statement);
            }
        }
        private void ResolveFunction(Stmt.Function function, FunctionType functionType) {
            
            FunctionType enclosingFunction = currentFunction;
            currentFunction = functionType;

            BeginScope();
            foreach (Token param in function.Params) {
                Declare(param);
                Define(param);
            }
            Resolve(function.Body);
            EndScope();

            currentFunction = enclosingFunction;
        }
        private void Resolve(Stmt stmt) {
            stmt.Accept(this);
        }
        private void Resolve(Expr expr) {
            expr.Accept(this);
        }
        private void BeginScope() {
            Scopes.Push(new Dictionary<string, bool>());
        }
        private void EndScope() {
            Scopes.Pop();
        }
        private void Declare(Token name) {
            if (Scopes.Count == 0) return;

            Dictionary<string, bool> scope = Scopes.Peek();


            if (scope.ContainsKey(name.Lexeme)) {
                Lox.Error(name, "Already a variable with this name in this scope");
            }

            scope.Add(name.Lexeme, false);
        }
        private void Define(Token name) {
            if (Scopes.Count == 0) return;

            // NOT
            if (Scopes.Peek().ContainsKey(name.Lexeme)) Scopes.Peek()[name.Lexeme] = true;
            else Scopes.Peek().Add(name.Lexeme, true);
        }
        private void ResolveLocal(Expr expr, Token name) {
            int index = 0;

            foreach (var scope in Scopes) {
                if (scope.ContainsKey(name.Lexeme)) {
                    interpreter.Resolve(expr, index);
                }
                index++;
            }

        }
        public object VisitVarStmt(Stmt.Var stmt) {
            Declare(stmt.Name);
            if (stmt.Initializer != null) {
                Resolve(stmt.Initializer);
            }
            Define(stmt.Name);
            return null;
        }
        public object VisitWhileStmt(Stmt.While stmt) {
            Resolve(stmt.Condition);
            Resolve(stmt.Body);
            return null;
        }
        public object VisitAssignExpr(Expr.Assign expr) {
            Resolve(expr.Value);
            ResolveLocal(expr, expr.Name);
            return null;
        }
        public object VisitBinaryExpr(Expr.Binary expr) {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }
        public object VisitTernaryExpr(Expr.Ternary expr) {
            Resolve(expr.Left);
            Resolve(expr.Middle);
            Resolve(expr.Right);
            return null;
        }
        public object VisitCallExpr(Expr.Call expr) {
            Resolve(expr.Callee);

            foreach (var argument in expr.Arguments) {
                Resolve(argument);
            }

            return null;
        }
        public object VisitGetExpr (Expr.Get expr) {
            Resolve(expr.Object);
            return null;
        }
        public object VisitGroupingExpr(Expr.Grouping expr) {
            Resolve(expr.Expression);
            return null;
        }
        public object VisitLiteralExpr(Expr.Literal expr) {
            return null;
        }
        public object VisitLogicalExpr(Expr.Logical expr) {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }
        public object VisitSetExpr(Expr.Set expr) {
            Resolve(expr.Value);
            Resolve(expr.Object);
            return null;
        }

        public object VisitSuperExpr(Expr.Super expr) {
            if (CurrentClass == ClassType.NONE) {
                Lox.Error(expr.Keyword, "Can't use 'super' outside of a class");
            } else if (CurrentClass != ClassType.SUBCLASS) {
                Lox.Error(expr.Keyword, "Can't use 'super' in a class with no superclass.");
            }

            ResolveLocal(expr, expr.Keyword);
            return null;
        }
        public object VisitThisExpr(Expr.This expr) {
            if (CurrentClass == ClassType.NONE) {
                Lox.Error(expr.Keyword, "Can't use 'this' outside a class.");
            }

            ResolveLocal(expr, expr.Keyword);
            return null;
        }

        public object VisitUnaryExpr(Expr.Unary expr) {
            Resolve(expr.Right);
            return null;
        }
        public object VisitVariableExpr(Expr.Variable expr) {
            /*  
             *  Avoiding such constructions:
             *  
             *  var a;
             *  a = a;
             *  
             */
            if (Scopes.Count != 0 && Scopes.Peek().ContainsKey(expr.Name.Lexeme) && Scopes.Peek()[expr.Name.Lexeme] == false) {
                Lox.Error(expr.Name, "Can't read local variable in its own initializer.");
            }
            ResolveLocal(expr, expr.Name);
            return null;
        }        
    }
}
