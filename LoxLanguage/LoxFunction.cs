using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxLanguage {
    class LoxFunction : LoxCallable {
        private Stmt.Function Declaration;
        public LoxFunction(Stmt.Function declaration) {
            Declaration = declaration;
        }

        /*
         * We create a new environment at each call, not at the function declaration.
         * 
         */
        public object Call(Interpreter interpreter, List<Object> arguments) {
            Environment environment = new Environment(interpreter.Globals);

            for (var i = 0; i < Declaration.Params.Count; i++) {
                environment.Define(Declaration.Params[i].Lexeme, arguments[i]);
            }

            interpreter.ExecuteBlock(Declaration.Body, environment);
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
