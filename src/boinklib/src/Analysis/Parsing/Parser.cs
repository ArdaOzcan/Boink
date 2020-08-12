using System;
using System.Collections.Generic;

using Boink.Analysis.Tokenization;
using Boink.AST.Nodes;
using Boink.Errors;


namespace Boink.Analysis.Parsing
{
    /// <summary>
    /// Parser for the program.
    /// <para>
    /// Parser takes tokens from the lexer and converts them to SyntaxNodes.
    /// That created the tree of SyntaxNodes which creates the program syntax.
    /// </para>
    /// </summary>
    public sealed class Parser
    {

        /// <summary>
        /// Token types that are for type declarations.
        /// - int
        /// - bool 
        /// - float 
        /// - dyn
        /// </summary>
        static HashSet<TokenType> TypeTokens = new HashSet<TokenType>
        {
            TokenType.DynamicType,
            TokenType.IntType,
            TokenType.BoolType,
            TokenType.FloatType,
            TokenType.StringType
        };

        /// <summary>
        /// Token types that are for binary operations.
        /// - +
        /// - -
        /// - ||
        /// - ==
        /// </summary>
        static HashSet<TokenType> BinaryOperatorTokens = new HashSet<TokenType>
        {
            TokenType.Plus,
            TokenType.Minus,
            TokenType.AmpersandAmpersand,
            TokenType.PipePipe,
            TokenType.Greater,
            TokenType.GreaterEquals,
            TokenType.Less,
            TokenType.LessEquals,
            TokenType.EqualsEquals
        };

        /// <summary>
        /// Token types for parsing terms.
        /// - *
        /// - /
        /// </summary>
        static HashSet<TokenType> TermOperators = new HashSet<TokenType>
        {
            TokenType.Star,
            TokenType.Slash,
            TokenType.Greater,
            TokenType.GreaterEquals,
            TokenType.Less,
            TokenType.LessEquals,
            TokenType.EqualsEquals
        };

        /// <summary>
        /// Lexer of the program.
        /// <para>
        /// Used for getting tokens.
        /// </para>
        /// </summary>
        public Lexer ProgramLexer { get; private set; }

        public string FilePath { get; private set; }

        /// <summary>
        /// Current token the parser will read.
        /// </summary>
        public Token CurrentToken { get; private set; }

        /// <summary>
        /// Construct a Parser object.
        /// </summary>
        /// <param name="lexer">Lexer of the program.</param>
        public Parser(Lexer lexer)
        {
            ProgramLexer = lexer;
            FilePath = lexer.FilePath;
            CurrentToken = ProgramLexer.GetNextToken();
        }

        /// <summary>
        /// Log the parse tree with header and footer.
        /// </summary>
        /// <param name="root">Root node of the AST.</param>
        public string LogTree(SyntaxNode root)
        {
            Console.WriteLine("------- Parsed Program ------\n");
            string tree = root.ToString();
            Console.WriteLine(tree);
            Console.WriteLine("\n------------- End -----------");
            return tree;
        }

        /// <summary>
        /// Consume all tokens until it reaches another token type.
        /// At least one token whose type is the given type is necessary,
        /// otherwise an error will be thrown. 
        /// </summary>
        /// <param name="tokenType">Requested token type.</param>
        public void ConsumeAll(TokenType tokenType)
        {
            Consume(tokenType);
            while (CurrentToken.Type == tokenType)
                Consume(tokenType);
        }

        /// <summary>
        /// Consume a token that can have multiple possible types.
        /// <para>
        /// A HashSet that contains few TokenTypes can be
        /// passed into this method and if the next token is one of 
        /// them, it will be consumed, else an error will be thrown.
        /// </para>
        /// </summary>
        /// <param name="tokenTypes">A container that holds TokenTypes.</param>
        public void ConsumeMultiple(HashSet<TokenType> tokenTypes)
        {
            // Check if the passed set contains the current token's type.
            if (!tokenTypes.Contains(CurrentToken.Type))
            {
                // Throw a Boink error if the current token is not inside the set.
                ErrorHandler.Throw(new UnexpectedTokenError($"Expected one of {{ {string.Join(", ", tokenTypes)} }} instead of {CurrentToken.Type} {CurrentToken.Val}",
                                                            CurrentToken.Pos, FilePath));
            }

            // Continue to get the next token.
            CurrentToken = ProgramLexer.GetNextToken();
        }

        /// <summary>
        /// Consume a token but don't throw an error if it's not found.
        /// </summary>
        /// <param name="tokenType">Requested token type.</param>
        public void ConsumeOptional(TokenType tokenType)
        {
            if (CurrentToken.Type == tokenType)
                CurrentToken = ProgramLexer.GetNextToken();

        }

        /// <summary>
        /// Consume all tokens until it reaches another token type.
        /// But don't throw an error if it's not found.
        /// </summary>
        /// <param name="tokenType">Requested TokenType.</param>
        public void ConsumeAllOptional(TokenType tokenType)
        {
            while (CurrentToken.Type == tokenType)
                Consume(tokenType);
        }

        /// <summary>
        /// Consume the next token if it is found, else throw an error.
        /// </summary>
        /// <param name="tokenType">Requested TokenType.</param>
        public void Consume(TokenType tokenType, bool ignoreWhitespace = true)
        {
            // Check if the current token type matches the passed in token type.
            if (CurrentToken.Type != tokenType)
            {
                // Throw a Boink error if token types don't match.
                ErrorHandler.Throw(new UnexpectedTokenError($"Expected a {tokenType} instead of <{CurrentToken.Type}: {CurrentToken.Val}>",
                                                            CurrentToken.Pos, FilePath));
            }

            // Continue to get the next token.
            CurrentToken = ProgramLexer.GetNextToken(ignoreWhitespace);
        }

        /// <summary>Parse a GiveSyntax.</summary>
        public GiveSyntax ParseGive()
        {
            // the 'give' token if the syntax is correct.
            Token giveToken = CurrentToken;

            // Consume the give token ahead.
            Consume(TokenType.Give);

            // Parse the give expression
            SyntaxNode expr = ParseExpression();

            return new GiveSyntax(giveToken, expr);
        }

        /// <summary>
        /// Parse a VariableSyntax.
        /// </summary>
        /// <returns>Parsed SyntaxNode.</returns>
        public VariableSyntax ParseVariable()
        {
            // Token that contains the variables name.
            Token varToken = CurrentToken;

            // Consume the word token ahead.
            Consume(TokenType.Word);

            // Create a new VariableSyntax.
            var variable = new VariableSyntax(varToken);

            // Check if there is a dot ahead
            // meaning there is a child reference.
            if (CurrentToken.Type == TokenType.Dot)
            {
                // There is a child reference.

                // Set the child reference of the variable.
                variable.ChildReference = ParseChildReference();

                // Get the type of the child reference syntax.
                var varType = variable.ChildReference.GetType();

            }

            return variable;
        }

        /// <summary>
        /// Parse a child reference with dots.
        /// <example>
        /// io.write
        /// </example>
        /// </summary>
        /// <returns>
        /// A VariableSyntax or a FunctionCallSyntax 
        /// that is the child of another SyntaxNode
        /// </returns>
        private SyntaxNode ParseChildReference()
        {
            // Consume the dot token ahead.
            Consume(TokenType.Dot);

            // Parse a variable (can be a function, parser doesn't care).
            var childVar = ParseVariable();

            // Check if the next token is a left parenthesis.
            if (CurrentToken.Type == TokenType.LeftParenthesis)
            {
                // Is a function call
                // Parse call arguments
                List<SyntaxNode> args = ParseCallArguments();

                // Return a new FunctionCallSyntax.
                return new FunctionCallSyntax(childVar, args);
            }

            // Return the parsed variable.
            return childVar;
        }

        /// <summary>
        /// Parse a StringLiteralSyntax.
        /// </summary>
        /// <returns>Parsed SyntaxNode</returns>
        public StringLiteralSyntax ParseStringLiteral()
        {
            // String literal token.
            Token token = CurrentToken;

            Consume(TokenType.StringLiteral);

            var stringLiteralSyntax = new StringLiteralSyntax(token);

            // Check if the next token is a dot.
            if (CurrentToken.Type == TokenType.Dot)
                // Set the child reference of the string literal.
                stringLiteralSyntax.ChildReference = ParseChildReference();

            return stringLiteralSyntax;
        }

        /// <summary>
        /// Parse a IntLiteralSyntax.
        /// </summary>
        /// <returns>Parsed SyntaxNode</returns>
        public IntLiteralSyntax ParseIntLiteral()
        {
            // Int literal token.
            Token token = CurrentToken;
            Consume(TokenType.IntLiteral);

            var intLiteralSyntax = new IntLiteralSyntax(token);

            // Check if the next token is a dot.
            if (CurrentToken.Type == TokenType.Dot)
                // Set the child reference of the int literal.
                intLiteralSyntax.ChildReference = ParseChildReference();

            return intLiteralSyntax;
        }

        /// <summary>
        /// Parse a FloatLiteralSyntax.
        /// </summary>
        /// <returns>Parsed SyntaxNode.</returns>
        public FloatLiteralSyntax ParseFloatLiteral()
        {
            Token token = CurrentToken;
            Consume(TokenType.FloatLiteral);

            var floatLiteralSyntax = new FloatLiteralSyntax(token);

            // Check if the next token is a dot.
            if (CurrentToken.Type == TokenType.Dot)
                // Set the child reference of the float literal.
                floatLiteralSyntax.ChildReference = ParseChildReference();

            return floatLiteralSyntax;
        }

        /// <summary>
        /// Parse a ParenthesizedSyntax.
        /// </summary>
        /// <returns>Parsed SyntaxNode.</returns>
        public ParenthesizedSyntax ParseParenthesizedExpression()
        {
            Consume(TokenType.LeftParenthesis);

            // Parse the expression in the middle.
            SyntaxNode expr = ParseExpression();

            Consume(TokenType.RightParenthesis);

            var parenthesizedSyntax = new ParenthesizedSyntax(expr);

            // Check if the next token is a dot.
            if (CurrentToken.Type == TokenType.Dot)
                // Set the child reference of the parenthesized syntax.
                parenthesizedSyntax.ChildReference = ParseChildReference();

            return parenthesizedSyntax;
        }

        /// <summary>
        /// Parse a BoolLiteralSyntax.
        /// </summary>
        /// <returns>Parsed SyntaxNode.</returns>
        public BoolLiteralSyntax ParseBoolLiteral()
        {
            Token token = CurrentToken;
            Consume(TokenType.BoolLiteral);

            var boolLiteralSyntax = new BoolLiteralSyntax(token);

            // Check if the next token is a dot.
            if (CurrentToken.Type == TokenType.Dot)
                // Set the child reference of the parenthesized syntax.
                boolLiteralSyntax.ChildReference = ParseChildReference();

            return boolLiteralSyntax;
        }

        /// <summary>
        /// Parse the first part of expression.
        /// <para>
        /// **Example:**
        ///     (1 + 2) * 3
        ///     - Left factor is a ParenthesizedSyntax and the right factor is an IntLiteralSyntax. 
        /// </para>
        /// </summary>
        /// <returns>The syntax node of the factor.</returns>
        public SyntaxNode ParseFactor()
        {
            Token token = CurrentToken;
            switch (token.Type)
            {
                case TokenType.Minus:
                    Consume(TokenType.Minus);
                    return new UnaryOperationSyntax(token, ParseFactor());
                case TokenType.Plus:
                    Consume(TokenType.Plus);
                    return new UnaryOperationSyntax(token, ParseFactor());
                case TokenType.IntLiteral:
                    return ParseIntLiteral();
                case TokenType.FloatLiteral:
                    return ParseFloatLiteral();
                case TokenType.BoolLiteral:
                    return ParseBoolLiteral();
                case TokenType.StringLiteral:
                    return ParseStringLiteral();
                case TokenType.LeftParenthesis:
                    return ParseParenthesizedExpression();
                case TokenType.Word:
                    {
                        VariableSyntax var = ParseVariable();

                        if (CurrentToken.Type == TokenType.LeftParenthesis)
                        {
                            // Is a function call
                            List<SyntaxNode> args = ParseCallArguments();
                            return new FunctionCallSyntax(var, args);
                        }

                        return var;
                    }
                default:
                    return null;
            }
        }

        /// <summary>
        /// Parse the second part of an expression.
        /// <para>
        /// **Example:**
        ///     (1 + 2) * 3
        ///     - Left factor is a ParenthesizedSyntax and the right factor is an IntLiteralSyntax,
        ///     - So the term is a BinaryOperationSyntax : (ParenthesizedSyntax * IntLiteralSyntax).
        /// </para>
        /// </summary>
        /// <returns>Binary operation of factors or just the factor.</returns>
        public SyntaxNode ParseTerm()
        {
            // Parse the factor.
            SyntaxNode node = ParseFactor();

            // If the operator is a term operator.
            while (TermOperators.Contains(CurrentToken.Type))
            {
                Token token = CurrentToken;
                Consume(token.Type);
                node = new BinaryOperationSyntax(node, token, ParseFactor());
            }

            return node;
        }

        /// <summary>
        /// Parse an expression.
        /// An expression can be an equation or a logic operation.
        /// <para>
        /// **Example:**
        /// 
        ///     1 + (0 * 2.23) - 2
        ///     - An expression is a binary operation of two terms. ( see <see> Parser.ParseTerm </see>)
        /// </para>
        /// </summary>
        /// <returns>Binary operation of two terms or just the term.</returns>
        public SyntaxNode ParseExpression()
        {

            SyntaxNode node = ParseTerm();

            // BinaryOperatorTokens contain all binary operators as tokens.
            while (BinaryOperatorTokens.Contains(CurrentToken.Type))
            {
                Token token = CurrentToken;
                Consume(token.Type);
                node = new BinaryOperationSyntax(node, token, ParseTerm());
            }

            return node;
        }

        /// <summary>
        /// Parse an arguments list in a function declaration.
        /// <example>
        /// <code>
        /// fn increment(int a)
        ///     int c = a + 1
        /// ;
        /// </code>
        /// - Return value: [DeclarationSyntax: int a] 
        /// </example>
        /// </summary>
        /// <returns>List of DeclarationSyntaxes.</returns>
        public List<DeclarationSyntax> ParseArgumentList()
        {
            Consume(TokenType.LeftParenthesis);

            var args = new List<DeclarationSyntax>();
            while (true)
            {
                // Check if the current token is a type token.
                if (!TypeTokens.Contains(CurrentToken.Type))
                    // Break if not.
                    break;

                // Parse declaration.
                DeclarationSyntax decl = ParseDeclaration();
                args.Add(decl);

                // Check if the list goes on.
                if (CurrentToken.Type != TokenType.Comma)
                    // Break if not.
                    break;

                Consume(TokenType.Comma);
            }

            Consume(TokenType.RightParenthesis);
            return args;
        }

        /// <summary>
        /// Parse a function declaration.
        /// </summary>
        /// <returns>SyntaxNode representation of the parsed function.</returns>
        public FunctionSyntax ParseFunction()
        {
            // Consume 'fn'
            Consume(TokenType.FunctionDefinition);

            Token name = CurrentToken;
            Consume(TokenType.Word);

            List<DeclarationSyntax> args = ParseArgumentList();

            // Determine give type.
            TypeSyntax giveType = null;
            if (CurrentToken.Type == TokenType.RightArrow)
            {
                Consume(TokenType.RightArrow);
                giveType = ParseType();
            }

            ConsumeAll(TokenType.NewLine);

            FunctionSyntax func = new FunctionSyntax(name, giveType);

            List<SyntaxNode> statements = ParseStatements();

            Consume(TokenType.Semicolon);

            // Get statements.
            foreach (SyntaxNode s in statements)
                func.Statements.Add(s);

            // Get args.
            foreach (DeclarationSyntax a in args)
                func.Args.Add(a);

            return func;
        }

        /// <summary>
        /// Parse a type name.
        /// </summary>
        /// <returns>SyntaxNode representation of a type name.</returns>
        public TypeSyntax ParseType()
        {
            Token token = null;

            // Check if the current token is a type token.
            if (TypeTokens.Contains(CurrentToken.Type))
            {
                token = CurrentToken;
                Consume(CurrentToken.Type);
                return new TypeSyntax(token);
            }

            // Throw a Boink error if the token is not a type name.
            ErrorHandler.Throw(new UnexpectedTokenError($"Expected a type token instead of {CurrentToken.Type}: {CurrentToken.Val}",
                                                        CurrentToken.Pos, FilePath));
            return null;
        }

        /// <summary>
        /// Parse a variable declaration.
        /// </summary>
        /// <returns>SyntaxNode representation of a variable declaration.</returns>
        public DeclarationSyntax ParseDeclaration()
        {
            // Parse the type syntax.
            TypeSyntax varType = ParseType();

            string name = (CurrentToken.Val as string);
            Consume(TokenType.Word);

            SyntaxNode expr = null;
            if (CurrentToken.Type == TokenType.Equals)
            {
                Consume(TokenType.Equals);
                expr = ParseExpression();
            }

            return new DeclarationSyntax(varType, name, expr);
        }

        /// <summary>
        /// Parse arguments of a function call.
        /// </summary>
        /// <returns>List of expressions passed as arguments.</returns>
        public List<SyntaxNode> ParseCallArguments()
        {
            Consume(TokenType.LeftParenthesis);

            List<SyntaxNode> args = new List<SyntaxNode>();
            while (true)
            {
                // Break if there is a ')' ahead.
                if (CurrentToken.Type == TokenType.RightParenthesis)
                    break;

                SyntaxNode syntax = ParseExpression();
                args.Add(syntax);

                // Break if there is no comma
                if (CurrentToken.Type != TokenType.Comma)
                    break;

                Consume(TokenType.Comma);
            }

            Consume(TokenType.RightParenthesis);
            return args;
        }

        /// <summary>
        /// Parse a statement.
        /// </summary>
        /// <returns>SyntaxNode representation of the statement.</returns>
        public SyntaxNode ParseStatement()
        {
            if (TypeTokens.Contains(CurrentToken.Type))
                // Is a type name, so a declaration.
                return ParseDeclaration();

            if (CurrentToken.Type == TokenType.Word)
            {
                // Is variable name
                VariableSyntax var = ParseVariable();

                if (CurrentToken.Type == TokenType.Equals)
                {
                    // Is an assignment
                    Consume(TokenType.Equals);
                    SyntaxNode expr = ParseExpression();
                    return new AssignmentSyntax(var, expr);
                }

                if (CurrentToken.Type == TokenType.LeftParenthesis)
                {
                    // Is a call
                    List<SyntaxNode> args = ParseCallArguments();
                    return new FunctionCallSyntax(var, args);
                }

                ConsumeMultiple(new HashSet<TokenType> { TokenType.Equals, TokenType.LeftParenthesis, TokenType.Dot });
            }

            if (CurrentToken.Type == TokenType.FunctionDefinition)
                // Is a function declaration.
                return ParseFunction();

            if (CurrentToken.Type == TokenType.Give)
                // Is a give statement.
                return ParseGive();

            if (CurrentToken.Type == TokenType.If)
                // Is an if statement.
                return ParseIf();

            if (CurrentToken.Type == TokenType.Import)
                // Is an if statement.
                return ParseImport();

            return null;
        }

        /// <summary>
        /// Parse import syntax.
        /// <example>
        /// import io
        /// </example>
        /// </summary>
        /// <returns>SyntaxNode representation of an import statement.</returns>
        private SyntaxNode ParseImport()
        {
            var importToken = CurrentToken;
            Consume(TokenType.Import);

            var package = ParsePackage();

            return new ImportSyntax(importToken, package);
        }

        private PackageSyntax ParsePackage()
        {
            var hierarchy = new List<string>();
            while(CurrentToken.Type == TokenType.Word)
            {
                hierarchy.Add((string)CurrentToken.Val);
                Consume(TokenType.Word);

                if(CurrentToken.Type != TokenType.Dot)
                    break;

                Consume(TokenType.Dot);
            }

            return new PackageSyntax(hierarchy);
        }

        /// <summary>
        /// Parse a list of statements seperated by new lines.
        /// </summary>
        /// <returns>List of statements.</returns>
        public List<SyntaxNode> ParseStatements()
        {
            List<SyntaxNode> statements = new List<SyntaxNode>();
            SyntaxNode statement = ParseStatement();

            if (statement == null) 
                return statements;

            ConsumeAll(TokenType.NewLine);

            while (true)
            {
                statements.Add(statement);
                statement = ParseStatement();

                if (statement == null) 
                    break;

                ConsumeAll(TokenType.NewLine);
            }

            return statements;
        }

        /// <summary>
        /// Entry point of the parser.
        /// </summary>
        /// <param name="fileName">Name of the file to be given to the program.</param>
        /// <returns>Root node of the AST.</returns>
        public ProgramSyntax Parse(string fileName)
        {
            ProgramSyntax program = new ProgramSyntax(fileName);

            ConsumeAllOptional(TokenType.NewLine);

            foreach (SyntaxNode statement in ParseStatements())
                program.Statements.Add(statement);

            Consume(TokenType.EndOfFile);

            return program;
        }

        /// <summary>
        /// Parse an if statement.
        /// </summary>
        /// <returns>SyntaxNode representation of an if statement.</returns>
        public IfSyntax ParseIf()
        {
            Token token = CurrentToken;

            Consume(TokenType.If);
            Consume(TokenType.LeftParenthesis);

            SyntaxNode expr = ParseExpression();

            Consume(TokenType.RightParenthesis);
            Consume(TokenType.NewLine);

            IfSyntax ifStatement = new IfSyntax(token, expr);

            List<SyntaxNode> statements = ParseStatements();

            Consume(TokenType.Semicolon);

            foreach (SyntaxNode s in statements)
                ifStatement.Statements.Add(s);

            return ifStatement;
        }
    }
}
