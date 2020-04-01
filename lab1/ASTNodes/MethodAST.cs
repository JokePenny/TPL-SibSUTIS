using System;
using System.Collections.Generic;

namespace lab1.ASTNodes
{
    class MethodAST : ASTNode
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

        public MethodAST(string nameMethod, List<ASTNode> argsMethod) // call with args
        {
            this.nameMethod = nameMethod;
            this.argsMethod = argsMethod;
            //TODO: check hash table
        }

        public MethodAST(string nameMethod) // call without args
        {
            this.nameMethod = nameMethod;
            //TODO: check hash table
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
    }
}
