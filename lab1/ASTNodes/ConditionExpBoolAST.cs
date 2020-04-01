using System;

namespace lab1.ASTNodes
{
    class ConditionExpBoolAST : ASTNode
    {
        private ASTNode leftNode;
        private ASTNode rightNode;
        private string opCondition;

        public ConditionExpBoolAST(string opCondition, ASTNode leftNode, ASTNode rightNode)
        {
            this.opCondition = opCondition;
            this.leftNode = leftNode;
            this.rightNode = rightNode;
        }

        public ConditionExpBoolAST(ASTNode leftNode) 
        {
            this.leftNode = leftNode;
            rightNode = null;
            opCondition = null;
        }

        public ASTNode GetLeftNode()
        {
            return leftNode;
        }

        public ASTNode GetRightNode()
        {
            return rightNode;
        }

        public string GetOp()
        {
            return opCondition;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[COND_BOOL] " + opCondition);
            leftNode.Print(level + "\t");
            rightNode.Print(level + "\t");
        }
    }
}
