namespace lab1.ASTNodes
{
    class BodyConditionBoolAST : ASTNode
    {
        private ASTNode leftNode;
        private ASTNode rightNode;
        private string opCondition;

        public BodyConditionBoolAST(string opCondition, ASTNode leftNode, ASTNode rightNode) // if(a > b) || if((a > s) > (b && s))
        {
            this.opCondition = opCondition;
            this.leftNode = leftNode;
            this.rightNode = rightNode;
        }

        public BodyConditionBoolAST(ASTNode leftNode) // if(true) || if((a > s))
        {
            this.leftNode = leftNode;
            rightNode = null;
            opCondition = null;
        }
    }
}
