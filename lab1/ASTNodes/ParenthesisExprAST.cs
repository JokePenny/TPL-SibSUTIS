namespace lab1.ASTNodes
{
    class ParenthesisExprAST : ASTNode
    {
        private ASTNode expr;

        public ParenthesisExprAST(ASTNode expr)
        {
            this.expr = expr;
        }
    }
}
