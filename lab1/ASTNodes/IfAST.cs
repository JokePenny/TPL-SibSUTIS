using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    class IfAST : ASTNode
    {
        private ASTNode branching;
        public IfAST(ASTNode branching)
        {
            this.branching = branching;
        }

        public ASTNode GetBranching()
        {
            return branching;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[IF]");
            if (branching != null) branching.Print(level + "\t");
        }
    }
}
