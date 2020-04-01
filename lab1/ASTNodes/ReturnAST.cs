using System;

namespace lab1.ASTNodes
{
    class ReturnAST : ASTNode
    {
        private string typeReturn;
        private ASTNode returnNode;
        
        public ReturnAST(string typeReturn, ASTNode returnNode)
        {
            this.typeReturn = typeReturn;
            this.returnNode = returnNode;
        }

        public ASTNode GetReturnNode()
        {
            return returnNode;
        }

        public string GetTypeReturn()
        {
            return typeReturn;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[RETURN] " + typeReturn);
            returnNode.Print(level + "\t");
        }
    }
}
