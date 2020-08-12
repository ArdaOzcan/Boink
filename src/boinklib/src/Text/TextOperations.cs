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
    }
}