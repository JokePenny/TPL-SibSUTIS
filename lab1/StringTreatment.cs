using System;
using System.Collections.Generic;
using System.Text;

namespace lab1
{
    class StringTreatment
    {
        public static char[] dictSpaceBetween;

        public static string[] DeleteCommentsAndTab(string stroke)
        {
            string deleteComments = FindComments(stroke);
            string deleteTabulation = deleteComments.Replace("\t", " ");
            //string deleteSlash = deleteTabulation.Replace("\\", " ");
            string deleteSlashR = deleteTabulation.Replace("\r", " ");
            string[] deleteNewline = deleteSlashR.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            return deleteNewline;
        }

        private static string FindComments(string stroke)
        {
            StringBuilder insertingSpace = new StringBuilder();
            StringBuilder multyLineComment = new StringBuilder();
            bool isFindNewLine = false;
            bool isFindMultyLine = false;
            for (int i = 0; i < stroke.Length - 1; i++)
            {
                if (stroke[i] == '/' && stroke[i + 1] == '*')
                {
                    isFindMultyLine = true;
                }
                if (stroke[i] == '/' && stroke[i + 1] == '/')
                {
                    isFindNewLine = true;
                }
                if (stroke[i] == '\n')
                {
                    isFindNewLine = false;
                }

                if (!isFindNewLine && !isFindMultyLine) insertingSpace.Append(stroke[i]);
                else if (isFindMultyLine) multyLineComment.Append(stroke[i]);

                if (stroke[i] == '*' && stroke[i + 1] == '\\')
                {
                    isFindMultyLine = false;
                }
            }
            if (isFindMultyLine) insertingSpace.Append(multyLineComment);
            return insertingSpace.ToString();
        }

        internal static string FindString()
        {
            throw new NotImplementedException();
        }

        public static string FormatStroke(string stroke)
        {
            string insertSpaceBetweenKey = InsertSpaceBetween(stroke);
            return insertSpaceBetweenKey;
        }

        // удаление пробелов с одновременным поиском конца строки
        public static string[] HeavyDeleteSpace(string stroke, ref StringBuilder stringFinder, ref bool isFindStroke)
        {
            string[] deleteSpace = HeavySplit(stroke, ref stringFinder, ref isFindStroke);
            return deleteSpace;
        }

        private static string[] HeavySplit(string stroke, ref StringBuilder stringFinder, ref bool isFindStroke)
        {
            List<string> stringWithoutSpace = new List<string>();
            StringBuilder newSubString = new StringBuilder();
            for (int i = 0; i < stroke.Length; i++)
            {

                if (stroke[i] == ' ' && !isFindStroke)
                {
                    stringWithoutSpace.Add(newSubString.ToString());
                    newSubString.Clear();
                }
                else if (stroke[i] == '"' || isFindStroke)
                {
                    isFindStroke = !isFindStroke;
                    newSubString.Append(stroke[i]);

                    if (isFindStroke)
                    {
                        stringWithoutSpace.Add(newSubString.ToString());
                        newSubString.Clear();
                    }
                }
                else if(isFindStroke)
                    newSubString.Append(stroke[i]);
            }
            return stringWithoutSpace.ToArray();
        }

        private static string InsertSpaceBetween(string deleteNewline)
        {
            StringBuilder insertingSpace = new StringBuilder(deleteNewline);
            char[] strokeWithSpace = deleteNewline.ToCharArray();

            for (int i = 0, indexInsert = 0; i < strokeWithSpace.Length; i++, indexInsert++)
            {
                for (int j = 0; j < dictSpaceBetween.Length; j++)
                {
                    if (strokeWithSpace[i] == dictSpaceBetween[j])
                    {
                        insertingSpace.Insert(indexInsert, " ");
                        indexInsert++;
                        insertingSpace.Insert(indexInsert + 1, " ");
                        indexInsert++;
                        break;
                    }
                }
            }
            Console.WriteLine(insertingSpace);
            return insertingSpace.ToString();
        }
    }
}
