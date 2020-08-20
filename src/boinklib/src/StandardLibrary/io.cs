using System;
using System.Collections.Generic;

using Boink.Analysis.Semantic;
using Boink.Analysis.Semantic.Symbols;
using Boink.Interpretation.Library;

using Boink.Types;

namespace StandardLibrary
{
    public static class io
    {
        public static void writeLine(string_ text)
        {
            Console.WriteLine(text.Val);
        }
    }
}