namespace LoxLanguage {
    interface LoxCallable {
        int Arity();
        object Call(Interpreter interpreter, List<object> arguments);

        string ToString();
    }
}
