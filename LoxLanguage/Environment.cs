using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxLanguage {
    internal class Environment {
        readonly Environment? EnclosingEnvironment;

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
        public object Get(Token name) {
            if (Values.ContainsKey(name.Lexeme)) {
                return Values[name.Lexeme];
            }

            if (EnclosingEnvironment != null) return EnclosingEnvironment?.Get(name);

            throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
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
