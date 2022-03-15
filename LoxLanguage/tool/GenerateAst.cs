using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LoxLanguage.tool {
    class GenerateAst {
        public static void Main(string[] args) {          
            DefineAst(Path.Combine("..","..",".."), "Expr", new List<string> {
                "Binary   : Expr Left, Token Oper, Expr Right",
                "Grouping : Expr Expression",
                "Literal  : Object Value",
                "Unary    : Token Operator, Expr Right"
            });
        }
        public static void DefineAst(string outputDir, string baseName, List<string> types) {         
            string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName,  baseName + ".cs"); 
            Console.WriteLine(path);

            using (StreamWriter sw = File.CreateText(path)) {
                sw.WriteLine("namespace LoxLanguage {");
                sw.WriteLine("    abstract class " + baseName+ " {");
                sw.WriteLine("    }");                

                foreach (var type in types) {
                    var className = type.Split(":")[0].Trim();
                    var fields = type.Split(":")[1].Trim();

                    DefineType(sw, baseName, className, fields);
                }

                sw.WriteLine("}");
            }
        }

        public static void DefineType(StreamWriter sw, string baseName, string className, string fieldList) {
            sw.WriteLine("    sealed class " + className + " : " + baseName + " {");

            var fields = fieldList.Split(",");

            foreach (var field in fields) {
                sw.WriteLine("        readonly " + field.Trim() + ";");
            }

            sw.WriteLine("        public " + className + " (" + fieldList + ") {");

            foreach (var field in fields) {
                string name = field.Trim().Split(" ")[1];
                sw.WriteLine("            this." + name + " = " + name + ";");
            }
            
            sw.WriteLine("        }");

            sw.WriteLine("    }");
        }
    }
}


