using System;
using System.Collections.Generic;
using System.IO;

using Boink.Analysis.Parsing;
using Boink.Analysis.Semantic;
using Boink.Analysis.Tokenization;
using Boink.Errors;
using Boink.Interpretation;
using Boink.Interpretation.Library;
using Boink.Text;

namespace Boink
{
    /// <summary>
    /// Class where the Main method belongs to.
    /// Contains methods to operate Boink language components.
    /// </summary>
    public static class BoinkMain
    {
        /// <summary>
        /// Handle the Boink command 'parse' with the given arguments.
        /// </summary>
        /// <para>
        /// Generate the parse tree for a boink file and export it to the given directory.
        /// </para>
        /// <param name="filePath">Path of the Boink file.></param>
        /// <param name="outDir">Output directory of the parse tree.</param>
        public static void Parse(string filePath, string outDir)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File '{filePath}' doesn't exist");
                return;
            }

            string text = TextOperations.ReadFileNormalized(filePath);

            // ErrorHandler Handler for exceptions during lexing, parsing and semantic analysis.
            var lexer = new Lexer(text);
            var errorHandler = new ErrorHandler(lexer);

            var parser = new Parser(lexer);

            // Root node of the program a.k.a. the program itself.
            // Argument is the program name which is equivalent to file's name.
            var root = parser.Parse(Path.GetFileName(filePath));

            var parseTreeDict = root.JsonDict;
            // var parseTree = JsonSerializer.Serialize(parseTreeDict, options);
            string fullPath = Path.Combine(outDir, Path.GetFileNameWithoutExtension(filePath));
            string parseTree = Json.ToJsonString(parseTreeDict);

            if (!fullPath.EndsWith(".json"))
                fullPath += ".json";

            File.WriteAllText(fullPath, parseTree);

            Console.WriteLine($"Written the parse tree for {filePath} to {fullPath}:");
            // Console.WriteLine(parseTree);
        }

        /// <summary>
        /// Handle the Boink command 'run' with the given argument.
        /// </summary>
        /// <param name="filePath">Path of the Boink file.</param>
        public static void Run(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File '{filePath}' doesn't exist");
                return;
            }

            var dirCache = new DirectoryCache(Path.GetDirectoryName(filePath));
            string text = TextOperations.ReadFileNormalized(filePath);

            // ErrorHandler Handler for exceptions during lexing, parsing and semantic analysis.
            var lexer = new Lexer(text);
            var errorHandler = new ErrorHandler(lexer);

            // ErrorHandler Assign lexer to convert positions to line and offset.
            var parser = new Parser(lexer);

            // Root node of the program a.k.a. the program itself.
            // Argument is the program name which is equivalent to file's name.
            var root = parser.Parse(Path.GetFileName(filePath));

            string pathDirectory = Path.GetDirectoryName(filePath);
            var symbolTreeBuilder = new SemanticAnalyzer(pathDirectory, dirCache);
            symbolTreeBuilder.Visit(root);

            symbolTreeBuilder.WriteAll();

            if (errorHandler.HasLogs)
                errorHandler.WriteAll();
            else
            {
                var interpreter = new Interpreter(pathDirectory, dirCache);
                interpreter.Interpret(root, true);
            }
        }

        /// <summary>Main entry point for Boink language. </summary> 
        /// <param name="args">Arguments passed in by the user</param>
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("No command was supplied.");
                return;
            }
            if (args.Length == 0)
            {
                Console.WriteLine("No arguments were supplied.");
                return;
            }

            switch (args[0])
            {
                case "run":
                    {
                        if (args.Length < 2)
                        {
                            Console.WriteLine("No filename was supplied.");
                            break;
                        }
                        Run(args[1]);
                        break;
                    }

                case "parse":
                    if (args.Length < 3)
                    {
                        Console.WriteLine("No output directory was supplied.");
                        break;
                    }
                    if (args.Length < 2)
                    {
                        Console.WriteLine("No filename was supplied.");
                        break;
                    }
                    Parse(args[1], args[2]);
                    break;
                default:
                    Console.WriteLine($"Command '{args[1]}' doesn't exist");
                    return;
            }
        }
    }
}
