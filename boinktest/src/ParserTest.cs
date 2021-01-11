using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Boink.Analysis.Parsing;
using Boink.Analysis.Tokenization;
using Boink.Errors;
using Boink.Interpretation;

using Boink.Text;

namespace Boink.UnitTesting
{
    // [TestClass]
    // public class InterpreterTest
    // {
    //     [TestMethod]
    //     public void DeclarationTest()
    //     {
    //         string filePath = "..\\..\\..\\test-scripts\\declTest.boink";
    //         string text = TextOperations.ReadFileNormalized(filePath);

    //         var lexer = new Lexer(text);
    //         var errHandler = new ErrorHandler(lexer);
    //         var parser = new Parser(lexer);

    //         string fileName = Path.GetFileName(filePath);
    //         var root = parser.Parse(fileName);

    //         var interpreter = new Interpreter(root);
    //     }
    // }

    [TestClass]
    public class ParserTest
    {
        public void ParseTreeTest(string filePath)
        {
            Assert.AreEqual(Path.GetExtension(filePath), ".boink");

            var lexer = new Lexer(filePath);
            var errHandler = new ErrorHandler();
            var parser = new Parser(lexer);

            string fileName = Path.GetFileName(filePath);
            var root = parser.Parse(fileName);

            // Check for errors
            Assert.IsFalse(errHandler.HasLogs, "Boink threw an exception");
            
            string path = Path.ChangeExtension(filePath, ".json");
            string parseTree = File.ReadAllText(path);
            parseTree = parseTree.Replace(Environment.NewLine, "\n");
            var generatedTreeDict = root.JsonDict;
            string generatedTree = Json.ToJsonString(generatedTreeDict);

            Assert.AreEqual(parseTree.GetWithoutWhitespace(), generatedTree.GetWithoutWhitespace(), fileName);
        }

        [TestMethod]
        public void DeclarationTest() => ParseTreeTest("..\\..\\..\\test-scripts\\declTest.boink");
        [TestMethod]
        public void FunctionTest() => ParseTreeTest("..\\..\\..\\test-scripts\\fnTest.boink");
        [TestMethod]
        public void IfTest() => ParseTreeTest("..\\..\\..\\test-scripts\\ifTest.boink");
        [TestMethod]
        public void RecursionTest() => ParseTreeTest("..\\..\\..\\test-scripts\\recursionTest.boink");
        [TestMethod]
        public void StringLiteralTest() => ParseTreeTest("..\\..\\..\\test-scripts\\doubleQuotesTest.boink");
    }
}
