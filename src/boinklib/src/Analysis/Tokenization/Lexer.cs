using System;
using System.Collections.Generic;
using System.Globalization;

using Boink.Errors;
using Boink.Text;

namespace Boink.Analysis.Tokenization
{

    /// <summary>
    /// Lexer of the program.
    /// <para>
    /// Lexer loops through every character and converts them to tokens
    /// or raises errors if an unknown character or character sequence occurs.
    /// </para>
    /// </summary>
    /// <param name="text">Text of the program.</param>
    public class Lexer
    {
        /// <summary>
        /// Text of the program.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Current character of the lexer.
        /// </summary>
        public char CurrentChar
        {
            get
            {
                // Return '\0' if Pos exceeds text length.
                // '\0' is a special value equivalent to null for chars.
                if (Pos > Text.Length - 1)
                    return char.MinValue;

                return Text[Pos];
            }
        }

        /// <summary>
        /// Current position of the lexer.
        /// </summary>
        public int Pos { get; set; }
        
        public string FilePath { get; private set; }

        /// <summary>
        /// Dict to hold information about existing keywords and
        /// the corresponding token type to separate regular words
        /// (e.g. variable names) from keywords.
        /// </summary>
        static Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType> 
        {
            { "fn", TokenType.FunctionDefinition },
            { "dyn", TokenType.DynamicType },
            { "int", TokenType.IntType },
            { "false", TokenType.BoolLiteral },
            { "true", TokenType.BoolLiteral },
            { "bool", TokenType.BoolType },
            { "float", TokenType.FloatType },
            { "give", TokenType.Give },
            { "if", TokenType.If },
            { "string", TokenType.StringType },
            { "import", TokenType.Import }
        };

        /// <summary>Construct a Lexer object.</summary>
        /// <param name="text">Text of the program.</param>
        public Lexer(string filePath, string text = null)
        {
            Pos = 0;
            FilePath = filePath;
            if(text == null)
                Text = TextOperations.ReadFileNormalized(filePath);
            else
                Text = text;
        }

        /// <summary>
        /// Return the next number (whole or floating) as a token.
        /// </summary>
        /// <returns>Token of the next number.</returns>
        public Token GetNextNumberToken()
        {
            string result = "";
            int startPos = Pos;

            // While Pos doesn't exceed the text length and is a digit,
            // add the current character to the result
            while (CurrentChar != char.MinValue && char.IsDigit(CurrentChar))
            {
                result += CurrentChar;
                Pos += 1;
            }

            if (CurrentChar == '.') 
            {
                // It is a floating number.
                result += CurrentChar;
                Pos += 1;

                // Add the rest to the result.
                while (CurrentChar != char.MinValue && char.IsDigit(CurrentChar))
                {
                    result += CurrentChar;
                    Pos += 1;
                }

                // Parse string to float.
                float val = float.Parse(result, CultureInfo.InvariantCulture.NumberFormat);
                return new Token(TokenType.FloatLiteral, val, startPos);
            }

            return new Token(TokenType.IntLiteral, int.Parse(result), startPos);
        }

        /// <summary>
        /// Return the character with the offset of amount
        /// from the current position. 
        /// <para>
        /// Return null if the character
        /// is out of range. 
        /// </para>
        /// </summary>
        /// <param name="amount">Offset from the current character.Defaults to 1.</param>
        /// <returns>Peeked character.</returns>
        public char Peek(int amount = 1)
        {
            if (Pos + amount < Text.Length)
                return Text[Pos + amount];

            return char.MinValue;
        }

        /// <summary>
        /// Check if a text is ahead from the current character.
        /// </summary>
        /// <param name="text">Text to be checked if is ahead.</param>
        /// <returns>If the text is ahead.</returns>
        public bool IsAhead(string text)
        {
            if(Pos + text.Length > Text.Length)
                return false;

            return (Text.Substring(Pos, text.Length) == text);
        }

        /// <summary>
        /// Check if character is alphanumeric or an underscore.
        /// </summary>
        /// <param name="c">Character.</param>
        /// <returns>Boolean.</returns>
        public static bool IsAlphanumOrUnderscore(char c) => char.IsLetterOrDigit(c) || c == '_';

        /// <summary>
        /// Check if character is a letter or an underscore.
        /// </summary>
        /// <param name="c">Character.</param>
        /// <returns>Boolean.</returns>
        public static bool IsLetterOrUnderscore(char c) => char.IsLetter(c) || c == '_';

        /// <summary>
        /// Get the next string literal token.
        /// </summary>
        /// <returns>The next string literal token.</returns>
        public Token GetNextStringToken()
        {
            string result = "";

            // Opening quote.
            Pos += 1;
            int startPos = Pos;

            // Add the current character to the result until EOF or a double quote.
            while (CurrentChar != char.MinValue && CurrentChar != '"')
            {
                result += CurrentChar;
                Pos += 1;
            }

            // Closing quote.
            Pos += 1;

            return new Token(TokenType.StringLiteral, result, startPos);            
        }

        /// <summary>
        /// Get the next word token and separate keywords 
        /// with regular word.
        /// </summary>
        /// <returns>The next word token.</returns>
        public Token GetNextWordToken()
        {
            string result = "";
            int startPos = Pos;

            while (CurrentChar != char.MinValue && IsAlphanumOrUnderscore(CurrentChar))
            {
                result += CurrentChar;
                Pos += 1;
            }

            TokenType keyword;
            if (Keywords.TryGetValue(result, out keyword)) 
            {
                // Is a keyword.
                object val = result;
                if (keyword == TokenType.BoolLiteral)
                    val = (result == "true");

                return new Token(keyword, val, startPos);
            }

            return new Token(TokenType.Word, result, startPos);
        }

        /// <summary>
        /// Pass whitespace until it isn't whitespace.
        /// </summary>
        public void IgnoreWhitespace()
        {
            while (CurrentChar != char.MinValue && CurrentChar == ' ')
                Pos += 1;
        }

        /// <summary>
        /// Pass characters until it's newline.
        /// </summary>
        public void IgnoreComment()
        {
            if(IsAhead("#>"))
            {
                while (CurrentChar != char.MinValue && 
                       !IsAhead("<#"))
                    Pos += 1;
                Pos += 2;
            }
            else
            {
                while (CurrentChar != char.MinValue && 
                       !IsAhead(TextOperations.BoinkNewLine))
                    Pos += 1;
            }
        }

        /// <summary>
        /// Return the next token or call a method that would.
        /// this method is the entry point for getting the next.
        /// </summary>
        /// <returns>The next token.</returns>
        public Token GetNextToken(bool ignoreWhitespace=true)
        {
            Token token;

            if(ignoreWhitespace) 
                IgnoreWhitespace();
            else
            {
                if(CurrentChar == ' ')
                {
                    ErrorHandler.Throw(
                        new UnexpectedTokenError("Whitespace not expected", Pos, FilePath)
                    );

                    IgnoreWhitespace();
                    return GetNextToken();
                }
            }

            switch (CurrentChar)
            {
                case '#':
                    IgnoreComment();
                    return GetNextToken();
                case char.MinValue:
                    return new Token(TokenType.EndOfFile, null, Pos);
            }

            if (char.IsDigit(CurrentChar))
                return GetNextNumberToken();

            if (IsLetterOrUnderscore(CurrentChar))
                return GetNextWordToken();

            switch (CurrentChar)
            {
                case '+':
                    token = new Token(TokenType.Plus, CurrentChar, Pos);
                    Pos += 1;
                    return token;
                case ',':
                    token = new Token(TokenType.Comma, CurrentChar, Pos);
                    Pos += 1;
                    return token;
                case '-':
                    {
                        if (IsAhead("->"))
                        {
                            int startPos = Pos;
                            Pos += "->".Length;
                            return new Token(TokenType.RightArrow, "->", startPos);
                        }

                        token = new Token(TokenType.Minus, CurrentChar, Pos);
                        Pos += 1;
                        return token;
                    }

                case '*':
                    token = new Token(TokenType.Star, CurrentChar, Pos);
                    Pos += 1;
                    return token;
                case '/':
                    token = new Token(TokenType.Slash, CurrentChar, Pos);
                    Pos += 1;
                    return token;
                case '(':
                    token = new Token(TokenType.LeftParenthesis, CurrentChar, Pos);
                    Pos += 1;
                    return token;
                case ')':
                    token = new Token(TokenType.RightParenthesis, CurrentChar, Pos);
                    Pos += 1;
                    return token;
                case '=':
                    {
                        if (IsAhead("=="))
                        {
                            int startPos = Pos;
                            Pos += "==".Length;
                            return new Token(TokenType.EqualsEquals, "==", startPos);
                        }

                        token = new Token(TokenType.Equals, CurrentChar, Pos);
                        Pos += 1;
                        return token;
                    }
                case ';':
                    token = new Token(TokenType.Semicolon, CurrentChar, Pos);
                    Pos += 1;
                    return token;
                case '.':
                    token = new Token(TokenType.Dot, CurrentChar, Pos);
                    Pos += 1;
                    return token;
                case '"':
                    return GetNextStringToken();
                case '>':
                    {
                        if (IsAhead(">="))
                        {
                            int startPos = Pos;
                            Pos += ">=".Length;
                            return new Token(TokenType.GreaterEquals, ">=", startPos);
                        }

                        token = new Token(TokenType.Greater, CurrentChar, Pos);
                        Pos += 1;
                        return token;
                    }

                case '<':
                    {
                        if (IsAhead("<="))
                        {
                            int startPos = Pos;
                            Pos += "<=".Length;
                            return new Token(TokenType.LessEquals, "<=", startPos);
                        }

                        token = new Token(TokenType.Less, CurrentChar, Pos);
                        Pos += 1;
                        return token;
                    }
            }
            
            if (IsAhead(TextOperations.BoinkNewLine))
            {
                token = new Token(TokenType.NewLine, TextOperations.BoinkNewLine, Pos);
                Pos += TextOperations.BoinkNewLine.Length;
                return token;
            }
            if (IsAhead("&&"))
            {
                int startPos = Pos;
                Pos += "&&".Length;
                return new Token(TokenType.AmpersandAmpersand, "&&", startPos);
            }

            if (IsAhead("||"))
            {
                int startPos = Pos;
                Pos += "||".Length;
                return new Token(TokenType.PipePipe, "||", startPos);
            }

            ErrorHandler.Throw(
                new UnknownTokenError($"Character '{CurrentChar}' is not known",
                                      Pos, FilePath)
            );

            // Skip to next token if an error is thrown.
            Pos += 1;
            return GetNextToken();
        }

    }

}
