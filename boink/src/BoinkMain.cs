using System;
using System.Collections.Generic;
using System.IO;

using ArdaOzcan.SimpleArgParse;

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

            // ErrorHandler Handler for exceptions during lexing, parsing and semantic analysis.
            var lexer = new Lexer(filePath);
            var errorHandler = new ErrorHandler();

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
        public static void Run(string filePath, bool verbose)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File '{filePath}' doesn't exist");
                return;
            }

            var dirCache = new DirectoryCache(Path.GetDirectoryName(filePath));

            // ErrorHandler Handler for exceptions during lexing, parsing and semantic analysis.
            var lexer = new Lexer(filePath);
            var errorHandler = new ErrorHandler();

            // ErrorHandler Assign lexer to convert positions to line and offset.
            var parser = new Parser(lexer);

            // Root node of the program a.k.a. the program itself.
            // Argument is the program name which is equivalent to file's name.
            var root = parser.Parse(Path.GetFileName(filePath));

            var symbolTreeBuilder = new SemanticAnalyzer(filePath, dirCache);
            symbolTreeBuilder.Visit(root);

            // symbolTreeBuilder.WriteAll();

            if (errorHandler.HasLogs)
                errorHandler.WriteAll();
            else
            {
                string pathDirectory = Path.GetDirectoryName(filePath);
                var interpreter = new Interpreter(pathDirectory, dirCache);
                interpreter.Interpret(root, verbose);
            }
        }

        /// <summary>Main entry point for Boink language. </summary> 
        /// <param name="args">Arguments passed in by the user</param>
        public static void Main(string[] args)
        {
            var parser = new ArgumentParser();
            var subparsers = parser.AddSubparsers(title:"commands", help:"Boink command to be run.", dest:"cmd");
            
            var runParser = subparsers.AddParser("run");
            runParser.AddArgument("filename", help:"Name of the file to be run.");
            runParser.AddArgument("-v", longName:"--verbose", action: ArgumentAction.StoreTrue);

            var parseParser = subparsers.AddParser("parse");
            parseParser.AddArgument("filename", help:"Name of the file to be parsed.");
            parseParser.AddArgument("outdir", help: "Output directory of the parsed JSON.");

            var ns = parser.ParseArgs(args);

            switch(ns["cmd"])
            {
                case "run":
                    Run((string)ns["filename"], (bool)ns["verbose"]);
                    break;
                case "parse":
                    Parse((string)ns["filename"], (string)ns["outdir"]);
                    break;
            }
        }
    }
}