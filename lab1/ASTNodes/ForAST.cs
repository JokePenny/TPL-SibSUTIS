using System;
using System.Collections.Generic;

namespace lab1.ASTNodes
{
    class ForAST : ASTNode
    {
        private List<ASTNode> declaredVar; // int a = 5, b = 7
        private ConditionNodeAST conditionWithBody; // a < b {body}
        private ASTNode postcondition; // a++

        public ForAST(List<ASTNode> declaredVar, ConditionNodeAST conditionWithBody, ASTNode postcondition) // if(a > b) || if((a > s) > (b && s))
        {
            this.declaredVar = declaredVar;
            this.conditionWithBody = conditionWithBody;
            this.postcondition = postcondition;
        }

        public List<ASTNode> GetDeclaredVar()
        {
            return declaredVar;
        }

        public ConditionNodeAST GetCondWithBody()
        {
            return conditionWithBody;
        }

        public ASTNode GetPostcondition()
        {
            return postcondition;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[FOR]");
            string levelUp = level + "\t";
            Console.WriteLine(levelUp + "[DECLARED]");
            for (int i = 0; i < declaredVar.Count; i++)
            {
                declaredVar[i].Print(levelUp + "\t");
            }
            conditionWithBody.Print(level + "\t");
            postcondition.Print(level + "\t");
        }
    }
}
