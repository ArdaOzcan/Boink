using System;
using System.Collections.Generic;

using Boink.Analysis.Tokenization;
using Boink.Logging;

using Boink.Text;

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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERRORS: ");

            foreach (string err in Logs)
                Console.WriteLine(err);

            Console.ResetColor();
        }

        public void AddLog(string s) => Logs.Add(s);

        public bool HasLogs => Logs.Count != 0;


        /// <summary>Add an error to the list.</summary>
        /// <param name="error">Error to be added.</param>
        public static void Throw(Error error)
        {
            instance.AddLog($"{error.GetType().Name}: {error.Msg}. Error Position: {TextOperations.ConvertPosToLine(error.FilePath, error.Pos)}. File: '{error.FilePath}'");
            /// Console.WriteLine($"{error.GetType().Name}: {error.Msg}. Error Position: {ProgramLexer.ConvertPosToLine(error.Pos)}");
            /// throw new Exception();
        }
        
        public ErrorHandler()
        {
            instance = this;
            Logs = new List<string>();
        }
    }
}
