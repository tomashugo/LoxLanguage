namespace LoxLanguage {
    internal class Environment {
        public Environment? EnclosingEnvironment { get; }

        private readonly Dictionary<string, object> Values = new Dictionary<string, object>();
        public Environment() {
            EnclosingEnvironment = null;
        }
        public Environment(Environment enclosing) { 
            EnclosingEnvironment = enclosing;
        }

        public void Define(string key, object value) {
            if (!Values.ContainsKey(key)) {
                Values.Add(key, value);
                return;
            }

            Values[key] = value;
        }

        private Environment Ancestor(int distance) {
            Environment environment = this;

            for (int i=0;i<distance;i++) {
                environment = environment.EnclosingEnvironment;
            }

            return environment;
        }
        public object Get(Token name) {
            if (Values.ContainsKey(name.Lexeme)) {
                return Values[name.Lexeme];
            }

            if (EnclosingEnvironment != null) return EnclosingEnvironment?.Get(name);

            throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
        }        
        public object GetAt(int distance, string name) {
            return Ancestor(distance).Values[name];
        }
        public void AssignAt(int distance, Token name, object value) {
            Ancestor(distance).Values[name.Lexeme]  = value;
        }
        public void Assign(Token name, object value) {
            if (Values.ContainsKey(name.Lexeme)) {
                Values[name.Lexeme] = value;
                return;
            }

            if (EnclosingEnvironment != null) {
                EnclosingEnvironment.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
        }

    }
}
