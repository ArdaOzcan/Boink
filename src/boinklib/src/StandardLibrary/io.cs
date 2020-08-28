using System;

using Boink.Types;

namespace StandardLibrary
{
    public static class io
    {
        public static void writeLine(string_ text)
        {
            Console.WriteLine(text.Val);
        }

        public static void write(string_ text)
        {
            Console.WriteLine(text.Val);
        }

        public static string_ readLine()
        {
            return new string_(null, Console.ReadLine());
        }
    }
}