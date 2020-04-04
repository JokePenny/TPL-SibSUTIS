using System;
using System.Collections.Generic;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class PostconditionForAST : ASTNode, IStorage
    {
        private List<ASTNode> memberPostcondition;

        public PostconditionForAST(List<ASTNode> memberPostcondition)
        {
            this.memberPostcondition = memberPostcondition;
        }

        public List<ASTNode> GetMemberPostcondition()
        {
            return memberPostcondition;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[FOR_POSTCONDITION]");
            for (int i = 0; i < memberPostcondition.Count; i++)
            {
                memberPostcondition[i].Print(level + "\t");
            }
        }

        public void SetNewSymbolIn(Dictionary<string, ASTNode> symTable)
        {
            throw new NotImplementedException();
        }
    }
}
