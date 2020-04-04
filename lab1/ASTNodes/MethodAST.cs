using System;
using System.Collections.Generic;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class MethodAST : ASTNode, IArea
    {
        private string typeMethod;
        private List<ASTNode> argsMethod;
        private BodyMethodAST bodyMethod;
        private string nameMethod;

        public MethodAST(string typeMethod, string nameMethod, List<ASTNode> argsMethod, BodyMethodAST bodyMethod)
        {
            this.typeMethod = typeMethod;
            this.nameMethod = nameMethod;
            this.argsMethod = argsMethod;
            this.bodyMethod = bodyMethod;
        }

        public MethodAST(string typeMethod, string nameMethod, BodyMethodAST bodyMethod)
        {
            this.typeMethod = typeMethod;
            this.nameMethod = nameMethod;
            this.bodyMethod = bodyMethod;
        }

        public MethodAST(string nameMethod, List<ASTNode> argsMethod)
        {
            this.nameMethod = nameMethod;
            this.argsMethod = argsMethod;
        }

        public MethodAST(string nameMethod)
        {
            this.nameMethod = nameMethod;
        }

        public string GetName()
        {
            return nameMethod;
        }

        public string GetTypeMethod()
        {
            return typeMethod;
        }

        public BodyMethodAST GetBodyMethod()
        {
            return bodyMethod;
        }

        public List<ASTNode> GetArgsMethod()
        {
            return argsMethod;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[TYPE] " + typeMethod);
            Console.WriteLine(level + "[METHOD] " + nameMethod);
            if(argsMethod != null)
            {
                Console.WriteLine(level + "[ARGUMENTS] ");
                for (int i = 0; i < argsMethod.Count; i++)
                {
                    argsMethod[i].Print(level + "\t");
                }
            }
            bodyMethod.Print(level + "\t");
        }

        public SymTableUse GetSymTable(string nameParent, Dictionary<string, ASTNode> parentTable)
        {
            Dictionary<string, ASTNode> symTable = new Dictionary<string, ASTNode>(parentTable);
            for(int i = 0; i < argsMethod.Count; i++)
            {
                if (argsMethod[i] is IStorage)
                    (argsMethod[i] as IStorage).SetNewSymbolIn(symTable);
            }
            return (bodyMethod as IArea).GetSymTable(nameMethod, symTable);
        }
    }
}
