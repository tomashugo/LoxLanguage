namespace LoxLanguage {
    class LoxFunction : LoxCallable {
        private Stmt.Function Declaration;
        private readonly Environment Closure;
        private bool IsInitializer;
        public LoxFunction(Stmt.Function declaration, Environment closure, bool isInitializer) {
            Declaration = declaration;
            Closure = closure;
            IsInitializer = isInitializer;
        }
        public LoxFunction Bind(LoxInstance instance) {
            Environment environment = new Environment(Closure);
            environment.Define("this", instance);
            return new LoxFunction(Declaration, environment, IsInitializer);
        }

        /*
         * We create a new environment at each call, not at the function declaration.
         * 
         */
        public object Call(Interpreter interpreter, List<Object> arguments) {
            Environment environment = new Environment(Closure);

            for (var i = 0; i < Declaration.Params.Count; i++) {
                environment.Define(Declaration.Params[i].Lexeme, arguments[i]);
            }

            try {
                interpreter.ExecuteBlock(Declaration.Body, environment);
            } catch (Return returnValue) {
                if (IsInitializer) return Closure.GetAt(0, "this");
                return returnValue.Value;
            }

            if (IsInitializer) return Closure.GetAt(0, "this");
            return null;
        }

        public int Arity() {
            return Declaration.Params.Count;
        }

        public string ToString() {
            return "<fn " + Declaration.Name.Lexeme + ">";
        }
    }
}
