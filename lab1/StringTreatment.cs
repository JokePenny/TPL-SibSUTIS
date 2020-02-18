using System;
using System.Collections.Generic;
using System.Text;

namespace lab1
{
    public sealed class StringTreatment
    {
        public readonly static string dictSpaceBetween = "[@,.]+-/{}|*^$!~&()%|?=<>!;";

        public static string[] DeleteCommentsAndTab(string stroke)
        {
            string deleteComments = FindComments(stroke);
            string deleteTabulation = deleteComments.Replace("\t", "");
            string deleteSlashR = deleteTabulation.Replace("\r", "");
            string[] deleteNewline = deleteSlashR.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            return deleteNewline;
        }

        public static string FindComments(string str)
        {
            StringBuilder strWithoutComments = new StringBuilder();
            StringBuilder multyLineComment = new StringBuilder();
            bool isFindLine = false;
            bool isFindMultyLine = false;
            for (int i = 0; i < str.Length - 1; i++)
            {
                if (str[i] == '/' && str[i + 1] == '*')
                {
                    isFindMultyLine = true;
                }
                if (str[i] == '/' && str[i + 1] == '/')
                {
                    isFindLine = true;
                }
                if (str[i] == '\n' || i == str.Length - 1)
                {
                    isFindLine = false;
                }

                if (!isFindLine && !isFindMultyLine) strWithoutComments.Append(str[i]);
                else if (isFindMultyLine) multyLineComment.Append(str[i]);

                if (str[i] == '*' && str[i + 1] == '/')
                {
                    isFindMultyLine = false;
                }
            }
            if (isFindMultyLine) strWithoutComments.Append(multyLineComment);

            int strLen = str.Length;

            // добавляем последний элемет если он не относится ни к одному из видов комментариев
            if (!(str[strLen - 2] == '/' && str[strLen - 1] == '*'
                || str[strLen - 2] == '/' && str[strLen - 1] == '/'
                || str[strLen - 2] == '*' && str[strLen - 1] == '/'
                || str[strLen - 1] == '\n') && !isFindLine)
            {
                strWithoutComments.Append(str[strLen - 1]);
            }

            return strWithoutComments.ToString();
        }

        public static string[] FormatStroke(string stroke)
        {
            string insertSpaceBetweenKey = InsertSpaceBetween(stroke);
            return SplitString(insertSpaceBetweenKey);
        }

        public static string[] SplitString(string insertSpaceBetweenKey)
        {
            List<string> listWords = new List<string>();
            StringBuilder insertingSpace = new StringBuilder();
            char[] strokeWithSpace = insertSpaceBetweenKey.ToCharArray();

            bool isFindString = false;

            for (int i = 0; i < strokeWithSpace.Length; i++)
            {
                if(insertSpaceBetweenKey[i] == '"')
                {
                    isFindString = !isFindString;
                    insertingSpace.Append(insertSpaceBetweenKey[i]);
                    continue;
                }
                else if(insertSpaceBetweenKey[i] == ' ' && !isFindString && insertingSpace.Length != 0)
                {
                    listWords.Add(insertingSpace.ToString());
                    insertingSpace = new StringBuilder();
                    continue;
                }
                if (insertSpaceBetweenKey[i] != ' ' || isFindString) insertingSpace.Append(insertSpaceBetweenKey[i]);
            }
            if (insertingSpace.Length != 0) listWords.Add(insertingSpace.ToString());
            return listWords.ToArray();
        }

        public static string InsertSpaceBetween(string deleteNewline)
        {
            StringBuilder insertingSpace = new StringBuilder(deleteNewline);
            char[] strokeWithSpace = deleteNewline.ToCharArray();

            bool isFindString = false;

            for (int i = 0, indexInsert = 0; i < strokeWithSpace.Length; i++, indexInsert++)
            {
                for (int j = 0; j < dictSpaceBetween.Length; j++)
                {
                    if (strokeWithSpace[i] == '"')
                    {
                        isFindString = !isFindString;
                        continue;
                    }

                    if (strokeWithSpace[i] == dictSpaceBetween[j] && !isFindString)
                    {
                        if (indexInsert - 1 >= 0)
                        {
                            if (insertingSpace[indexInsert - 1] != ' ')
                            {
                                insertingSpace.Insert(indexInsert, " ");
                                indexInsert++;
                            }
                        }
                        if (indexInsert + 1 < insertingSpace.Length)
                        {
                            if (insertingSpace[indexInsert + 1] != ' ')
                            {
                                insertingSpace.Insert(indexInsert + 1, " ");
                                indexInsert++;
                            }
                        }
                        break;
                    }
                }
            }
            return insertingSpace.ToString();
        }
    }
}
