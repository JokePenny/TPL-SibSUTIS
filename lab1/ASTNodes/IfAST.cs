using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    class IfAST : ASTNode
    {
        private List<ConditionNodeAST> branching;
        public IfAST(List<ConditionNodeAST> branching)
        {
            this.branching = branching;
        }

        public List<ConditionNodeAST> GetBranching()
        {
            return branching;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[IF]");
            for (int i = 0; i < branching.Count; i++)
            {
                branching[i].Print(level + "\t");
            }
        }
    }
}
