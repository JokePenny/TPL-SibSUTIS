using System;

namespace lab1.ASTNodes
{
    class ConditionNodeAST : ASTNode
    {
        private ASTNode bodyCondition; // условие
        private ASTNode body; // тело
        public ConditionNodeAST(ASTNode bodyCondition, ASTNode body)
        {
            this.bodyCondition = bodyCondition;
            this.body = body;
        }

        public ASTNode GetCondition()
        {
            return bodyCondition;
        }

        public ASTNode GetBody()
        {
            return body;
        }

        public override void Print(string level)
        {
            if (bodyCondition != null)
            {
                Console.WriteLine(level + "[CONDITION]");
                bodyCondition.Print(level + "\t");
            }
            Console.WriteLine(level + "[BODY]");
            body.Print(level + "\t");
        }
    }
}
