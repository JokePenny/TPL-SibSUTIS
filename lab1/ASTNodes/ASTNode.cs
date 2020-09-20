using System;

namespace lab1.ASTNodes
{
    /// <summary>
    /// Координаты узла
    /// </summary>
    public struct Point
    {
        public readonly int y; //номер строки (сверху вниз)
        public readonly int x; //номер символа в строке (слева на право)
        public Point(int y, int x)
        {
            this.y = y;
            this.x = x;
        }
    }

    public class ASTNode
    { 
        public Point point;
        public ASTNode parent;

        /// <summary>
        /// Выввод в консоль построенное AST дерево
        /// </summary>
        public virtual void Print(string level) { }

        /// <summary>
        /// Запсь в файл сгенерерованный АСМ код
        /// </summary>
		public virtual void PrintASM(string leveltabulation, bool isNewLine = false) { }
    }
}
