using System;

namespace lab1.ASTNodes
{
    class CrementAST : ASTNode
    {
        private string crement;
        private ASTNode id;

        public CrementAST(string crement, ASTNode id)
        {
            this.crement = crement;
            this.id = id;
        }

        public string GetCrement()
        {
            return crement;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[CREMENT] " + crement);
            id.Print(level + "\t");
        }
    }
}
