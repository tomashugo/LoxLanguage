using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxLanguage {
    internal class LoxInstance {
        private LoxClass Klass;
        private Dictionary<string, object> Fields = new Dictionary<string, object>();
        public LoxInstance (LoxClass loxClass) {
            Klass = loxClass;
        }
        public override string ToString() {
            return Klass.Name + " instance";
        }
        public object Get(Token name) {
            if (Fields.ContainsKey(name.Lexeme)) {
                return Fields[name.Lexeme];
            }

            LoxFunction method = Klass.FindMethod(name.Lexeme);
            if (method != null) return method;

            throw new RuntimeError(name, "Undefined property '" + name.Lexeme + "'.");
        }
        public void Set(Token name, object value) {
            if (Fields.ContainsKey(name.Lexeme)) {
                Fields[name.Lexeme] = value;
            }
            else {
                Fields.Add(name.Lexeme, value);
            }            
        }
    }
}
