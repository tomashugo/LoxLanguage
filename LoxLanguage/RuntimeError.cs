using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxLanguage {
    internal class RuntimeError : SystemException {
        public Token Tk { get; }
        public RuntimeError(Token token, string message): base(message) {
            Tk = token;
        }
    }
}
