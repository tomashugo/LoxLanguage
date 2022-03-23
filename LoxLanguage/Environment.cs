using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxLanguage {
    internal class Environment {
        private readonly Dictionary<string, object> Values = new Dictionary<string, object>();
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

            throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
        }
        
        public void Assign(Token name, object value) {
            if (Values.ContainsKey(name.Lexeme)) {
                Values[name.Lexeme] = value;
                return;
            }

            throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
        }

    }
}
