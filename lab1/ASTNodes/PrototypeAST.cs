using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    // PrototypeAST - Этот класс представляет "прототип" для функции,
    // который хранит её имя и имена аргументов.
    class PrototypeAST : ASTNode
    {
        public string Name;
        List<string> Args;
        public PrototypeAST(string Name, List<string> Args)
        {
            this.Name = Name;
            this.Args = Args;
        }
    }
}
