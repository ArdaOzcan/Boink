using System;
using System.Collections.Generic;

using Boink.Analysis.Tokenization;
using Boink.Logging;

namespace Boink.Errors
{
    /// <summary>Data structure to hold a list of errors.</summary>
    public class ErrorHandler : ILogger
    {
        private static ErrorHandler instance;

        public List<string> Logs { get; set; }

        /// <summary>Log all errors in the list with a header.</summary>
        public void WriteAll()
        {
            Console.WriteLine("ERRORS: ");

            foreach (string err in Logs)
                Console.WriteLine(err);
        }

        public void AddLog(string s) => Logs.Add(s);

        public bool HasLogs => Logs.Count != 0;

        public Lexer ProgramLexer { get; set; }

        /// <summary>Add an error to the list.</summary>
        /// <param name="error">Error to be added.</param>
        public static void Throw(Error error)
        {
            instance.AddLog($"{error.GetType().Name}: {error.Msg}. Error Position: {instance.ProgramLexer.ConvertPosToLine(error.Pos)}. File: '{error.FilePath}'");
            /// Console.WriteLine($"{error.GetType().Name}: {error.Msg}. Error Position: {ProgramLexer.ConvertPosToLine(error.Pos)}");
            /// throw new Exception();
        }
        
        public ErrorHandler(Lexer programLexer)
        {
            instance = this;
            ProgramLexer = programLexer;
            Logs = new List<string>();
        }
    }
}
