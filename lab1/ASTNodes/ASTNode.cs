namespace lab1.ASTNodes
{
    public struct Point
    {
        public int y;
        public int x;
        public Point(int y, int x)
        {
            this.y = y;
            this.x = x;
        }
    }

    public class ASTNode
    {
        public Point point;

        public virtual void Print(string level) { }
        ~ASTNode() { }
    }
}
