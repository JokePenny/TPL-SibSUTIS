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
    }
}
