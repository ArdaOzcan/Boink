using System;

using Boink.Types;

namespace StandardLibrary
{
    public static class io
    {
        public static void writeLine(StringType text)
        {
            Console.WriteLine(text.Val);
        }

        public static void write(StringType text)
        {
            Console.Write(text.Val);
        }

        public static StringType readLine()
        {
            return new StringType(null, Console.ReadLine());
        }
    }
}