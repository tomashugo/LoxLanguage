using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxLanguage {
    class LoxFunction : LoxCallable {
        private Stmt.Function Declaration;
        private readonly Environment Closure;
        public LoxFunction(Stmt.Function declaration, Environment closure) {
            Declaration = declaration;
            Closure = closure;
        }
        public LoxFunction Bind(LoxInstance instance) {
            Environment environment = new Environment(Closure);
            environment.Define("this", instance);
            return new LoxFunction(Declaration, environment);
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
                return returnValue.Value;
            }
           
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
