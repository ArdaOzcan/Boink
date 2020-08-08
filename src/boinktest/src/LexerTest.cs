
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Boink.Analysis.Tokenization;

namespace Boink.UnitTesting
{
    /// <summary>
    /// Test Class to compare hardcoded values to the Lexer-generated values
    /// and fail if they're different.
    /// </summary>
    [TestClass]
    public class LexerTest
    {
        /// <summary>
        /// Compare the given token type to the lexed token type.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tokenType"></param>
        public void TokenTest(string text, TokenType tokenType)
        {
            var lexer = new Lexer(text);
            var token = lexer.GetNextToken();
            Assert.IsTrue(token.Type == tokenType);
        }

        [TestMethod]
        public void NewlineTokenTest() => TokenTest("\n", TokenType.NewLine);

        [TestMethod]
        public void IntLiteralTokenTest() => TokenTest("123", TokenType.IntLiteral);

        [TestMethod]
        public void FloatLiteralTokenTest() => TokenTest("1.23", TokenType.FloatLiteral);

        [TestMethod]
        public void FloatLiteralTokenTest2() => TokenTest("1.21134233", TokenType.FloatLiteral);

        [TestMethod]
        public void PlusTokenTest() => TokenTest("+", TokenType.Plus);

        [TestMethod]
        public void MinusTokenTest() => TokenTest("-", TokenType.Minus);

        [TestMethod]
        public void StarTokenTest() => TokenTest("*", TokenType.Star);

        [TestMethod]
        public void SlashTokenTest() => TokenTest("/", TokenType.Slash);

        [TestMethod]
        public void AmpersandAmpersandTokenTest() => TokenTest("&&", TokenType.AmpersandAmpersand);

        [TestMethod]
        public void PipePipeTokenTest() => TokenTest("||", TokenType.PipePipe);

        [TestMethod]
        public void WordTokenTest() => TokenTest("var", TokenType.Word);

        [TestMethod]
        public void FuncDefTokenTest() => TokenTest("fn", TokenType.FunctionDefinition);

        [TestMethod]
        public void EqualsTokenTest() => TokenTest("=", TokenType.Equals);

        [TestMethod]
        public void LParTokenTest() => TokenTest("(", TokenType.LeftParenthesis);

        [TestMethod]
        public void RParTokenTest() => TokenTest(")", TokenType.RightParenthesis);

        [TestMethod]
        public void SemicolonTokenTest() => TokenTest(";", TokenType.Semicolon);

        [TestMethod]
        public void DynTokenTest() => TokenTest("dyn", TokenType.DynamicType);

        [TestMethod]
        public void IntTokenTest() => TokenTest("int", TokenType.IntType);

        [TestMethod]
        public void CommaTokenTest() => TokenTest(",", TokenType.Comma);

        [TestMethod]
        public void BoolLiteralTokenTest() => TokenTest("true", TokenType.BoolLiteral);

        [TestMethod]
        public void BoolLiteralTokenTest2() => TokenTest("false", TokenType.BoolLiteral);

        [TestMethod]
        public void BoolTypeTokenTest() => TokenTest("bool", TokenType.BoolType);

        [TestMethod]
        public void FloatTypeTokenTest() => TokenTest("float", TokenType.FloatType);

        [TestMethod]
        public void RightArrowTokenTest() => TokenTest("->", TokenType.RightArrow);

        [TestMethod]
        public void GiveTokenTest() => TokenTest("give", TokenType.Give);

        [TestMethod]
        public void IfTokenTest() => TokenTest("if", TokenType.If);

        [TestMethod]
        public void GreaterTokenTest() => TokenTest(">", TokenType.Greater);

        [TestMethod]
        public void GreaterEqualsTokenTest() => TokenTest(">=", TokenType.GreaterEquals);

        [TestMethod]
        public void LessTokenTest() => TokenTest("<", TokenType.Less);

        [TestMethod]
        public void LessEqualsTokenTest() => TokenTest("<=", TokenType.LessEquals);

        [TestMethod]
        public void EqualsEqualsTokenTest() => TokenTest("==", TokenType.EqualsEquals);

        [TestMethod]
        public void StringLiteralTokenTest() => TokenTest("\"hello\"", TokenType.StringLiteral);
        
        [TestMethod]
        public void ImportTokenTest() => TokenTest("import", TokenType.Import);
        
        [TestMethod]
        public void DotTokenTest() => TokenTest(".", TokenType.Dot);
    }
}
