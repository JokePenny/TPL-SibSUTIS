using System;
using System.Diagnostics;

namespace lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            Command.RunCommand(args);
            //TestMethod();
        }

        private static void TestMethod()
        {
            // find min
            int[] array = new int[3];
            array[0] = 2;
            array[1] = 3;
            array[2] = 4;
            int tmp = array[0];
            for (int i = 0; i < 3; i++)
            {
                if (tmp > array[i])
				{
                    tmp = array[i];
                }
            }
            Console.WriteLine(tmp);

            // find substroke in stroke
            string stroke = "sdf";
            string strokeInFind = "sasdf asd";
            int doneLetter = 3;
            int nowAccesLetter = 0;
            int indexStartWordInStroke = 9999;
            for (int i = 0; i < 9; i++)
            {
                if (stroke[nowAccesLetter] == strokeInFind[i])
                {
                    nowAccesLetter++;
                    if (nowAccesLetter == doneLetter)
                    {
                        indexStartWordInStroke = i - nowAccesLetter;
                        break;
                    }
                }
                else
                {
                   // nowAccesLetter = 0;
                }
            }
            Console.WriteLine(indexStartWordInStroke);

            // find NOD
            int firstNum = 5;
            int secondNum = 10;
            while (firstNum != 0 && secondNum != 0)
            {
                if (firstNum > secondNum)
				{
                    firstNum = firstNum % secondNum;
                }
                else
				{
                    secondNum = secondNum % firstNum;
                }
            }
            Console.WriteLine(firstNum);
        }
    }
}
