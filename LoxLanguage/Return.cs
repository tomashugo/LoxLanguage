using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxLanguage {
    internal class Return : Exception {
        public object Value { get; }

        public Return(object value) : base() {            
            Value = value;
        }
    }
}
