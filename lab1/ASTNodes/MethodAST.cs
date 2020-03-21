using System.Collections.Generic;

namespace lab1.ASTNodes
{
    class MethodAST : ASTNode
    {
        private AccessModifierAST modifier;
        private TypeAST typeMethod;
        private List<ASTNode> argsMethod;
        private BodyMethodAST bodyMethod;
        private string nameMethod;

        public MethodAST(AccessModifierAST modifier, TypeAST typeMethod, string nameMethod, List<ASTNode> argsMethod, BodyMethodAST bodyMethod)
        {
            this.modifier = modifier;
            this.typeMethod = typeMethod;
            this.nameMethod = nameMethod;
            this.argsMethod = argsMethod;
            this.bodyMethod = bodyMethod;
        }

        public MethodAST(TypeAST typeMethod, string nameMethod, List<ASTNode> argsMethod, BodyMethodAST bodyMethod)
        {
            modifier = new AccessModifierAST();
            this.typeMethod = typeMethod;
            this.nameMethod = nameMethod;
            this.argsMethod = argsMethod;
            this.bodyMethod = bodyMethod;
        }

        public MethodAST(TypeAST typeMethod, string nameMethod, BodyMethodAST bodyMethod)
        {
            modifier = new AccessModifierAST();
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
    }
}
