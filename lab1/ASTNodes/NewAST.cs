using System;

namespace lab1.ASTNodes
{
    class NewAST : ASTNode
    {
        private ASTNode storageType;

        public NewAST(ASTNode storageType)
        {
            this.storageType = storageType;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[NEW]");
            storageType.Print(level + "\t");
        }
    }
}
