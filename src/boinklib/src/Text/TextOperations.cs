using System;
using System.IO;

namespace Boink.Text
{
    public static class TextOperations
    {
        public const string BoinkNewLine = "\n";

        public static string ReadFileNormalized(string filePath)
        {
            string result = File.ReadAllText(filePath);
            result = result.Replace(Environment.NewLine, BoinkNewLine);
            result += BoinkNewLine;
            return result;
        }
        
        public static string GetWithoutWhitespace(this string str)
        {
            str = str.Replace("\n", "");
            str = str.Replace("\r", "");
            str = str.Replace(" ", "");
            return str;
        }

        /// <summary>
        /// Convert a 1D position to a 2D position that is
        /// the line number and the offset from the previous newline.
        /// </summary>
        /// <param name="pos">1D position of a character.</param>
        /// <returns>New 2D position with the line number and offset.</returns>
        public static Tuple<int, int> ConvertPosToLine(string filePath, int pos)
        {
            string text = ReadFileNormalized(filePath);
            if (pos >= text.Length)
                return null;

            int i = 0, lineCount = 0, offsetToNewLine = 0;
            char chr = text[i];
            while (i < pos)
            {
                offsetToNewLine += 1;
                if (i + BoinkNewLine.Length - 1 < pos && text.Substring(i, BoinkNewLine.Length) == BoinkNewLine)
                {
                    lineCount += 1;
                    offsetToNewLine = 0;
                    i += BoinkNewLine.Length - 1; // If longer than 1, increase i.
                }

                i += 1;
                chr = text[i];
            }

            return new Tuple<int, int>(lineCount + 1, offsetToNewLine + 1);
        }
    }
}