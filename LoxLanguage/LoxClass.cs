﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxLanguage {
    internal class LoxClass : LoxCallable {
        public string Name { get; }
        private Dictionary<string, LoxFunction> Methods;

        public LoxClass(string name, Dictionary<string, LoxFunction> methods) {
            Name = name;
            Methods = methods;
        }
        public LoxFunction FindMethod(string name) {
            if (Methods.ContainsKey(name)) {
                return Methods[name];
            }

            return null;
        }

        public override string ToString() {
            return Name;
        }

        public int Arity() {
            return 0;
        }

        public object Call(Interpreter interpreter, List<object> arguments) {
            LoxInstance instance = new LoxInstance(this);
            return instance;
        }
    }
}