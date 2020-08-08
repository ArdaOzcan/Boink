using System;
using System.Collections.Generic;
using System.IO;

using Boink.Analysis.Parsing;
using Boink.Analysis.Tokenization;
using Boink.AST;
using Boink.AST.Nodes;
using Boink.Errors;
using Boink.Interpretation.Library;
using Boink.Logging;
using Boink.Text;
using Boink.Types;

namespace Boink.Analysis.Semantic
{
    /// <summary>
    /// ASTVisitor to do type and definition checking. 
    /// <para>
    /// This allows the interpreter to work without thinking about
    /// type or definition checking.
    /// </para>
    /// </summary>
    public class SemanticAnalyzer : ASTVisitor, ILogger
    {
        public List<string> Logs { get; set; }

        public void AddLog(string log) => Logs.Add(log);

        /// <summary>
        /// Global scope of the analyzer.
        /// </summary>
        public SymbolTable GlobalScope { get; private set; }

        /// <summary>
        /// Current scope of the analyzer.
        /// </summary>
        public SymbolTable CurrentScope { get; private set; }

        /// <summary>
        /// Directory of the Boink program that is being analyzed.
        /// </summary>
        public string ProgramDirectory { get; }

        /// <summary>
        /// Log all information stored in the list of logs with header and footer.
        /// </summary>
        public void WriteAll()
        {
            Console.WriteLine($"------ Semantic Analysis ----\n");

            foreach (string l in Logs)
                Console.WriteLine(l);

            Console.WriteLine("\n------------- End -----------");
        }

        /// <summary>
        /// Construct a SemanticAnalyzer object.
        /// </summary>
        public SemanticAnalyzer(string programDirectory)
        {
            Logs = new List<string>();
            ProgramDirectory = programDirectory;
        }

        /// <summary>
        /// Create a global scope and visit every statement in ProgramSyntax.
        /// </summary>
        /// <param name="node">Program syntax node.</param>
        /// <returns>Null.</returns>
        public override object Visit(ProgramSyntax node)
        {
            // Create the global scope.
            var scope = new SymbolTable("global");
            GlobalScope = scope;

            foreach (var s in node.Statements)
            {
                // Current scope will be global for every statement
                // directly belongs to the ProgramSyntax.
                CurrentScope = scope;

                Visit(s);
            }

            // Set the current scope to global on analysis exit.
            CurrentScope = scope;
            return null;
        }

        /// <summary>
        /// Check for previous definitions of the function, define the function,
        /// determine argument types and visit statements.
        /// </summary>
        /// <param name="node">Function syntax node.</param>
        /// <returns>Null.</returns>
        public override object Visit(FunctionSyntax node)
        {
            // Check if the function exists
            if (CurrentScope.Lookup((string)node.Name.Val) != null)
            {
                // If the function already exists, throw a Boink error.
                // TODO: Check for argument type too for function overloading.
                ErrorHandler.Throw(new MultipleDefinitionError($"Function {node.Name.Val} is already defined",
                                                               node.Pos));
                return null;
            }

            // Determine argument types.
            var argTypes = new List<Type>();
            foreach (var a in node.Args)
                argTypes.Add(a.Type);

            // Determine the give type as an obj_ type.
            Type giveType = null;
            if (node.GiveTypeSyntax != null)
            {
                var tokenType = node.GiveTypeSyntax.TypeToken.Type;
                giveType = obj_.GetTypeByTokenType(tokenType);
            }

            // Create a symbol for the function.
            var symbol = new FunctionSymbol(argTypes, (string)node.Name.Val, giveType);

            // Define the symbol to the current scope.
            CurrentScope.Define(symbol);

            // Log the definition.
            AddLog($"DEFINITION: {symbol} defined in scope {CurrentScope.ScopeName}.");

            // Generate a scope for this function.
            // (used for variables defined inside this function).
            var scope = new SymbolTable((string)node.Name.Val, symbol, CurrentScope);

            // Set the current scope to function's scope.
            CurrentScope = scope;

            // Visit arguments for declaration.
            foreach (var a in node.Args)
                Visit(a);

            bool hasGive = false;
            foreach (var s in node.Statements)
            {
                // Set hasGive is a statement is a GiveSyntax
                if (s.GetType() == typeof(GiveSyntax))
                    hasGive = true;

                Visit(s);

                // Set the current scope for the next statement.
                CurrentScope = scope;
            }

            // Check if the function either doesn't give or has a give type.
            if (giveType == null || hasGive)
                return null;
            

            // If the function has a give type but it doesn't give, throw a Boink error.
            ErrorHandler.Throw(new NoGiveError($"Function {node.Name.Val} doesn't give any value even though it has a give type of {giveType}",
                                               node.Pos));

            return null;
        }

        /// <summary>
        /// Visit left and right sides of a binary operation.
        /// </summary>
        /// <param name="node">Binary operation syntax node.</param>
        /// <returns></returns>
        public override object Visit(BinaryOperationSyntax node)
        {
            // Visit both sides.
            Visit(node.Left);
            Visit(node.Right);

            return null;
        }

        /// <summary>
        /// Lookup for a  variable reference and set the variable 
        /// type of the reference.
        /// </summary>
        /// <param name="node">Variable syntax node</param>
        /// <returns>Null.</returns>
        public override object Visit(VariableSyntax node)
        {
            // Parent node of this variable if there is any.
            var parentNode = node.ParentReference;

            // Scope of the parent (e.g. a library) if there is any.
            SymbolTable parentScope = null;

            // Is the parent node is a VariableSyntax?
            bool parentNodeIsVar = (parentNode != null &&
                                    parentNode.GetType() == typeof(VariableSyntax));

            // Check if the parent node is a VariableSyntax.
            if (parentNodeIsVar)
            {
                // Parent node as a VariableSyntax.
                VariableSyntax parentNodeAsVar = (VariableSyntax)parentNode;

                // Symbol of the parent node.
                Symbol parentSymbol = CurrentScope.Lookup(parentNodeAsVar.Name);

                // Check if the parent node is a LibrarySymbol.
                // Set the parentScope to the global scope of the library if true.
                if (parentSymbol.GetType() == typeof(LibrarySymbol))
                    parentScope = ((LibrarySymbol)parentSymbol).GlobalScope;
            }

            // Symbol of the referenced variable.
            Symbol symbol;

            // If there isn't any parent scope, lookup from the current scope.
            // Otherwise lookup from the parent scope.
            if (parentScope == null) symbol = CurrentScope.Lookup(node.Name);
            else symbol = parentScope.Lookup(node.Name);

            // Check if the symbol is defined.
            if (symbol != null)
            {
                // Set this node's type to the symbol's type.
                node.VarType = symbol.VarType;

                if (parentNodeIsVar)
                    // Set the parent node's type to this variable's type.
                    ((VariableSyntax)parentNode).VarType = node.VarType;


                if (node.ChildReference != null)
                    // Visit child reference.
                    Visit(node.ChildReference);

                return null;
            }

            // If a variable was not found, throw a Boink error.
            ErrorHandler.Throw(new UndefinedSymbolError($"Variable '{node.Name}' is not defined",
                                                        node.Pos));
            return null;
        }

        /// <summary>
        /// Visit the expression of the syntax.
        /// </summary>
        /// <param name="node">Parenthesized syntax node.</param>
        /// <returns>Null.</returns>
        public override object Visit(ParenthesizedSyntax node)
        {
            // Visit the expression
            Visit(node.Expr);
            return null;
        }

        /// <summary>
        /// Check for previous definitions, define a variable in the
        /// current scope, check types if assigned. 
        /// </summary>
        /// <param name="node">Declaration syntax node.</param>
        /// <returns>Null.</returns>
        public override object Visit(DeclarationSyntax node)
        {
            // Get the type of the type token.
            var varType = obj_.GetTypeByTokenType(node.VarType.TypeToken.Type);

            // Lookup for the variable in the current scope.
            var varSymbol = CurrentScope.LookupOnlyCurrentScope(node.Name);

            // Check if the variable is defined.
            if (varSymbol == null)
            {
                // Variable has not been defined before.

                // Create a a symbol for the variable.
                var symbol = new VarSymbol(varType, node.Name);

                // Define the symbol.
                CurrentScope.Define(symbol);

                // Add log.
                AddLog($"DEFINITION: {symbol} defined in scope {CurrentScope.ScopeName}.");

                // Check if there is an expression.
                if (node.Expr != null)
                {
                    // Variable is assigned in declaration.

                    // Visit the expression
                    Visit(node.Expr);

                    // Check if types match.
                    if (node.Expr.Type != varType)
                    {
                        // There is a type mismatch, throw a Boink error.
                        ErrorHandler.Throw(new IncompatibleTypesError($"Type {node.Expr.Type} and {varType} are not compatible for assignment",
                                                                      node.Pos));
                        return null;
                    }

                    // Add log.
                    AddLog($"ASSIGNMENT: Assigned {node.Expr.Type} to {varType}.");
                }

                return null;
            }

            // Throw a Boink error if it is already defined.
            ErrorHandler.Throw(new MultipleDefinitionError($"Variable {node.Name} is already defined",
                                                           node.Pos));
            return null;
        }

        /// <summary>
        /// Check types and previous definitions.
        /// 
        /// TODO: Add assignment to library members.
        /// </summary>
        /// <param name="node">Assignment syntax node.</param>
        /// <returns>Null.</returns>
        public override object Visit(AssignmentSyntax node)
        {
            // Lookup the variable.
            var symbol = CurrentScope.Lookup(node.Var.Name);

            // Visit the assigned expression.
            Visit(node.Expr);

            // Check if the variable is defined.
            if (symbol != null)
            {
                // Variable is defined.

                // Check if the types match.
                if (node.Expr.Type == symbol.VarType)
                {
                    // Types match.

                    // Add log.
                    AddLog($"ASSIGNMENT: Assigned {node.Expr.Type} to {symbol.VarType}.");
                    return null;
                }

                // Throw a Boink error if types don't match.
                ErrorHandler.Throw(new IncompatibleTypesError($"Type {node.Expr.Type} and {symbol.VarType} are not compatible for assignment",
                                                              node.Pos));
                return null;
            }

            // Throw a Boink error if the variable is not defined.
            ErrorHandler.Throw(new UndefinedSymbolError($"Variable '{node.Var.Name}' is not defined",
                                                        node.Var.Pos));
            return null;
        }

        /// <summary>
        /// Check types and amounts of arguments.
        /// </summary>
        /// <param name="node">Function call syntax node.</param>
        /// <returns>Null.</returns>
        public override object Visit(FunctionCallSyntax node)
        {
            // Parent node of this variable if there is any.
            SyntaxNode parentNode = node.Var.ParentReference;

            // Scope of the parent (e.g. a library) if there is any.
            SymbolTable parentScope = null;

            // Is the parent node is a VariableSyntax?
            bool parentNodeIsVar = (parentNode != null &&
                                    parentNode.GetType() == typeof(VariableSyntax));
            if (parentNodeIsVar)
            {
                // Parent node as a VariableSyntax.
                VariableSyntax parentNodeVar = (VariableSyntax)parentNode;

                // Symbol of the parent node.
                Symbol parentSymbol = CurrentScope.Lookup(parentNodeVar.Name);

                // Check if the parent node is a LibrarySymbol.
                // Set the parentScope to the global scope of the library if true.
                if (parentSymbol.GetType() == typeof(LibrarySymbol))
                    parentScope = ((LibrarySymbol)parentSymbol).GlobalScope;
            }

            // Symbol of the referenced variable.
            Symbol symbol;

            // Symbol as a FunctionSymbol.
            FunctionSymbol functionSymbol;

            // If there isn't any parent scope, lookup from the current scope.
            // Otherwise lookup from the parent scope.
            if (parentScope == null) symbol = CurrentScope.Lookup(node.Var.Name);
            else symbol = parentScope.Lookup(node.Var.Name);

            // Set functionSymbol to "as" casted symbol.
            functionSymbol = (symbol as FunctionSymbol);

            // Function has been defined before.
            if (functionSymbol != null)
            {
                // Set the parent's variable type to this function's give type is it is a variable.
                if (parentNodeIsVar)
                    ((VariableSyntax)parentNode).VarType = functionSymbol.GiveType;

                // Set this node's variable type to this function's give type.
                node.VarType = functionSymbol.GiveType;

                // Visit every argument passed in and
                // determine argument types of them.
                var argTypes = new List<Type>();
                foreach (var callArg in node.Args)
                {
                    Visit(callArg);
                    argTypes.Add(callArg.Type);
                }

                // Check for amount of arguments.
                if (argTypes.Count > functionSymbol.ArgTypes.Count)
                {
                    // Throw a Boink error if there are too many arguments passed in.
                    ErrorHandler.Throw(new ArgumentMismatchError($"Too many arguments for function call",
                                                                 node.Pos));
                    return null;
                }
                else if (argTypes.Count < functionSymbol.ArgTypes.Count)
                {
                    // Throw a Boink error if there are too few arguments passed in.
                    ErrorHandler.Throw(new ArgumentMismatchError($"Too few arguments for function call",
                                                                 node.Pos));
                    return null;
                }
                else
                {
                    // Check if types of each argument matches.
                    for (int i = 0; i < argTypes.Count; i++)
                    {
                        var callArgType = argTypes[i];
                        var actualArgType = functionSymbol.ArgTypes[i];
                        
                        if (callArgType != actualArgType)
                        {
                            // Throw a Boink error if types don't match.
                            ErrorHandler.Throw(new IncompatibleTypesError($"Type {callArgType} and {actualArgType} are not compatible for assignment",
                                                                          node.Pos));
                            return null;
                        }

                        AddLog($"CALL: Function {functionSymbol.Name} called with arguments {functionSymbol.ArgTypes}");
                    }
                }

                return null;
            }
            
            // Throw a Boink error if the function has not been defined before.
            ErrorHandler.Throw(new UndefinedSymbolError($"Function '{node.Var.Name}' is not defined",
                                                        node.Pos));
            return null;
        }

        /// <summary>
        /// Check for give type and if give is allowed.
        /// </summary>
        /// <param name="node">Give syntax node.</param>
        /// <returns>Null.</returns>
        public override object Visit(GiveSyntax node)
        {
            // Determine the expression type.
            Type exprType = null;
            if (node.Expr != null)
            {
                // Visit the expression syntax.
                Visit(node.Expr);
                exprType = node.Expr.Type;
            }

            // Set the currentFunctionSymbol to the function that this syntax gives from.
            var currentFunctionSymbol = CurrentScope.Owner;

            // Check if there is such a function.
            if (currentFunctionSymbol != null)
            {
                // Give type of the function.
                var giveType = currentFunctionSymbol.GiveType;

                // Check if there is give type or there is no expression.
                if (giveType != null || node.Expr == null)
                {
                    // Return if give type equals the expression type,
                    // syntax is correct.
                    if (giveType == exprType)
                        return null;
                    
                    // Throw a Boink error if the types don't match.
                    ErrorHandler.Throw(new IncompatibleTypesError($"Type {exprType} and {giveType} are not compatible for giving",
                                                                  node.Pos));
                    return null;
                }

                // Throw a Boink error if there is no give type.
                ErrorHandler.Throw(new IncompatibleTypesError($"'give' is not allowed because function '{currentFunctionSymbol.Name}' has no return type",
                                                              node.Pos));
                return null;
            }

            // Throw a Boink error if the give is not inside of a function
            // meaning the function is not defined.
            ErrorHandler.Throw(new GiveNotAllowedError($"'give' is not allowed here because it is not inside of a function",
                                                       node.Pos));
            return null;
        }

        /// <summary>
        /// Check for preposition type and visit every statement.
        /// </summary>
        /// <param name="node">If syntax node.</param>
        /// <returns>Null.</returns>
        public override object Visit(IfSyntax node)
        {
            // Visit the expression.
            Visit(node.Expr);

            // Check if the expression type is a Boink boolean.
            if (node.Expr.Type != typeof(bool_))
            {
                // Throw a Boink error if the type is not a bool_.
                ErrorHandler.Throw(new IncompatibleTypesError($"Type {node.Expr.Type} is not compatible for if preposition",
                                                              node.Expr.Pos));
            }

            // Visit each statement of the if syntax.
            foreach (var s in node.Statements)
                Visit(s);

            return null;
        }

        /// <summary>
        /// Import a library if any was found, throw an error otherwise.
        /// </summary>
        /// <param name="node">Import syntax node.</param>
        /// <returns>Null.</returns>
        public override object Visit(ImportSyntax node)
        {
            // TODO: Create some kind of cache for the files in a dir
            //       like Python.
            foreach (string file in Directory.GetFiles(ProgramDirectory))
            {
                // Continue if the extension is not .boink or
                // the filename doesn't equal the requested library name.
                if (Path.GetExtension(file) != ".boink" ||
                    Path.GetFileNameWithoutExtension(file) != node.LibName)
                    continue;
                

                // Read the library.
                string text = TextOperations.ReadFileNormalized(file);

                // Lex the library.
                var lexer = new Lexer(text);

                var parser = new Parser(lexer);

                
                // Parse the library.
                var root = parser.Parse(Path.GetFileName(file));

                var symbolTreeBuilder = new SemanticAnalyzer(Path.GetDirectoryName(file));

                // Semantically analyze the library.
                symbolTreeBuilder.Visit(root);

                // Write all logs.
                symbolTreeBuilder.WriteAll();

                // Create a library symbol for the library.
                LibrarySymbol symbol = new LibrarySymbol(node.LibName, root);

                // Set the global scope.
                symbol.GlobalScope = symbolTreeBuilder.GlobalScope;

                // Define the library.
                CurrentScope.Define(symbol);

                // Add log.
                AddLog($"IMPORT: Library '{node.LibName}' imported.");
                return null;
            }

            // Return if there is a standard library as the requested library name.
            if (LibraryManager.StandardLibraries.Contains(node.LibName))
                return null;
            
            // Throw a Boink error if no library could be found.
            ErrorHandler.Throw(new UnknownLibraryError($"Library {node.LibName} not found",
                                                       node.ImportToken.Pos));

            return null;
        }

        /// <summary>
        /// Visit the expression of the syntax.
        /// </summary>
        /// <param name="node">Unary operation syntax node.</param>
        /// <returns>Null.</returns>
        public override object Visit(UnaryOperationSyntax node)
        {
            // Visit the expression.
            Visit(node.Expr);
            return null;
        }

        public override object Visit(BoolLiteralSyntax node) => null;

        public override object Visit(IntLiteralSyntax node) => null;

        public override object Visit(FloatLiteralSyntax node) => null;

        public override object Visit(StringLiteralSyntax node) => null;
    }
}
