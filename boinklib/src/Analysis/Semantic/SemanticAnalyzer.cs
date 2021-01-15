using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Boink.Analysis.Semantic.Symbols;
using Boink.AST;
using Boink.AST.Nodes;
using Boink.Errors;
using Boink.Interpretation.Library;
using Boink.Logging;

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

        public DirectoryCache DirCache { get; }

        public LibraryManager ProgramLibraryManager { get; }

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

        public string FilePath { get; }

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
        public SemanticAnalyzer(string filePath, DirectoryCache dirCache)
        {
            Logs = new List<string>();
            ProgramDirectory = Path.GetDirectoryName(filePath);
            FilePath = filePath;
            DirCache = dirCache;
            ProgramLibraryManager = new LibraryManager();
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

        public override object Visit(TypeDefinitionSyntax node)
        {
            var oldScope = CurrentScope;
            var scope = new SymbolTable((string)node.Name.Val, null, CurrentScope);

            string typeName = (string)node.Name.Val;
            var symbol = new ClassSymbol(typeName);

            CurrentScope.Define(symbol);

            CurrentScope = scope;

            foreach (var statement in node.Statements)
            {
                if (statement.GetType() == typeof(FunctionSyntax))
                {
                    var funcNode = (FunctionSyntax)statement;
                    string functionName = (string)funcNode.Name.Val;
                    if (CurrentScope.Lookup(functionName) != null)
                    {
                        // If the function already exists, throw a Boink error.
                        ErrorHandler.Throw(new MultipleDefinitionError($"Function {funcNode.Name.Val} is already defined",
                                                                       funcNode.Pos, FilePath));
                        continue;
                    }

                    var argTypes = new List<BoinkType>();
                    foreach (var a in funcNode.Args)
                        argTypes.Add(a.ChildOrOwnType);

                    // Determine the give type as an obj_ type.
                    BoinkType giveType = new BoinkType();
                    if (funcNode.GiveTypeSyntax != null)
                    {
                        var tokenType = funcNode.GiveTypeSyntax.TypeToken.Type;
                        giveType = new BoinkType(ObjectType.GetTypeByTokenType(tokenType));
                    }

                    var ctor = false;

                    if (functionName == "construct")
                    {
                        giveType = new BoinkType(BoinkType.userTypes[typeName]);
                        ctor = true;
                    }

                    // Create a symbol for the function.
                    var funcSymbol = new FunctionSymbol(argTypes, functionName, giveType);

                    CurrentScope.Define(funcSymbol);
                    BoinkType.userTypes[typeName].Methods.Add(funcSymbol);

                    if (ctor)
                        symbol.Constructor = funcSymbol;
                }
                else if (statement.GetType() == typeof(DeclarationSyntax))
                {
                    var declNode = (DeclarationSyntax)statement;

                    string name = (string)declNode.VarType.TypeToken.Val;
                    var userDefinedType = CurrentScope.Lookup(name);
                    var builtinType = ObjectType.GetTypeByTokenType(declNode.VarType.TypeToken.Type);

                    BoinkType varType = null;
                    if (builtinType != null)
                        varType = new BoinkType(builtinType);


                    if (varType == null && userDefinedType.GetType() == typeof(ClassSymbol))
                    {
                        varType = new BoinkType(BoinkType.userTypes[userDefinedType.Name]);
                        declNode.UserDefinedType = varType;
                    }


                    // Lookup for the variable in the current scope.
                    var varSymbol = CurrentScope.LookupOnlyCurrentScope(declNode.Name);

                    // Check if the variable is defined.
                    if (varSymbol == null)
                    {
                        // Variable has not been defined before.
                        // Create a a symbol for the variable.

                        var sym = new VarSymbol(varType, declNode.Name);
                        CurrentScope.Define(sym);
                        BoinkType.userTypes[typeName].Fields.Add(sym);
                    }
                    else
                    {
                        // ERROR
                        Console.WriteLine(varSymbol + " is already defined.");
                    }

                }
                else
                {
                    Console.WriteLine("Illegal");
                }

                symbol.SymbolTable = CurrentScope;
            }

            foreach (var statement in node.Statements)
            {
                if (statement.GetType() == typeof(FunctionSyntax))
                {
                    var funcNode = (FunctionSyntax)statement;
                    string functionName = (string)funcNode.Name.Val;

                    FunctionSymbol owner = (FunctionSymbol)CurrentScope.Lookup(functionName);
                    var funcScope = new SymbolTable(functionName, owner, CurrentScope);

                    // Set the current scope to function's scope.
                    CurrentScope = scope;

                    // Visit arguments for declaration.
                    foreach (var a in funcNode.Args)
                        Visit(a);

                    BoinkType giveType = new BoinkType();
                    if (funcNode.GiveTypeSyntax != null)
                    {
                        var tokenType = funcNode.GiveTypeSyntax.TypeToken.Type;
                        giveType = new BoinkType(ObjectType.GetTypeByTokenType(tokenType));
                    }

                    CurrentScope = funcScope;


                    bool hasGiven = false;
                    foreach (var s in funcNode.Statements)
                    {
                        // Set hasGive is a statement is a GiveSyntax
                        if (s.GetType() == typeof(GiveSyntax))
                            hasGiven = true;

                        Visit(s);

                        // Set the current scope for the next statement.
                        CurrentScope = funcScope;
                    }

                    // Check if the function either doesn't give or has a give type.
                    if ((giveType.CSType != null || giveType.UType != null) && !hasGiven)
                    {
                        ErrorHandler.Throw(new NoGiveError($"Function {funcNode.Name.Val} doesn't give any value even though it has a give type of {giveType}",
                                               funcNode.Pos, FilePath));
                    }
                }
                else if (statement.GetType() != typeof(DeclarationSyntax))
                {
                    Console.WriteLine("Illegal");
                }

            }


            scope.Owner = symbol;

            CurrentScope = oldScope;

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
                                                               node.Pos, FilePath));
                return null;
            }

            // Determine argument types.
            var argTypes = new List<BoinkType>();
            foreach (var a in node.Args)
                argTypes.Add(a.ChildOrOwnType);

            // Determine the give type as an obj_ type.
            BoinkType giveType = new BoinkType();
            if (node.GiveTypeSyntax != null)
            {
                var tokenType = node.GiveTypeSyntax.TypeToken.Type;
                giveType = new BoinkType(ObjectType.GetTypeByTokenType(tokenType));
            }

            if (CurrentScope.Owner != null && CurrentScope.Owner.GetType() == typeof(ClassSymbol)
            && (string)node.Name.Val == "constructor")
                giveType = CurrentScope.Owner.VarType;

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
            if ((giveType.CSType == null && giveType.UType == null) || hasGive)
                return null;


            // If the function has a give type but it doesn't give, throw a Boink error.
            ErrorHandler.Throw(new NoGiveError($"Function {node.Name.Val} doesn't give any value even though it has a give type of {giveType}",
                                               node.Pos, FilePath));

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

            if (node.ChildOrOwnType == null)
            {
                ErrorHandler.Throw(new UnsupportedOperationError($"Binary operation not supported",
                                                                 node.Pos, FilePath));
            }

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
            // Symbol of the referenced variable.
            Symbol symbol;

            // If there isn't any parent scope, lookup from the current scope.
            // Otherwise lookup from the parent scope.
            if (node.ParentScope != null)
                symbol = node.ParentScope.Lookup(node.Name);
            else
                symbol = CurrentScope.Lookup(node.Name);

            // Check if the symbol is defined.
            if (symbol != null)
            {
                // Set this node's type to the symbol's type.
                node.VarType = symbol.VarType;

                if (node.ChildReference != null)
                {
                    SymbolTable parentScope = null;

                    Type symbolType = symbol.GetType();
                    if (symbolType == typeof(PackageSymbol))
                        parentScope = ((PackageSymbol)symbol).GlobalScope;

                    else if (symbolType == typeof(LibrarySymbol))
                        parentScope = ((LibrarySymbol)symbol).Importables;

                    else if (symbolType == typeof(VarSymbol) || symbolType == typeof(ClassSymbol))
                    {
                        var scope = new SymbolTable(node.Name, null, null);

                        // FieldInfo fieldInfo = node.VarType.GetField("Methods");
                        if (node.VarType.IsBuiltin)
                        {
                            FieldInfo fieldInfo = node.VarType.CSType.GetField("Methods");
                            if (fieldInfo != null)
                            {
                                object methodsObj = fieldInfo.GetValue(null);

                                Dictionary<string, MethodInfo> methodsDictionary = (Dictionary<string, MethodInfo>)methodsObj;

                                foreach (var kv in methodsDictionary)
                                {
                                    var methodName = kv.Key;
                                    var methodInfo = kv.Value;
                                    if (methodInfo == null)
                                        break;

                                    var args = methodInfo.GetParameters();
                                    List<BoinkType> argTypes = new List<BoinkType>();

                                    // i is 1 because the first real parameter
                                    // is an object reference since the actual
                                    // method is static.
                                    // Boink should ignore the first parameter
                                    // while creating a function symbol.
                                    for (int i = 1; i < args.Length; i++)
                                    {
                                        argTypes.Add(new BoinkType(args[i].ParameterType));
                                    }

                                    scope.Add(methodName, new FunctionSymbol(argTypes, methodName, new BoinkType(methodInfo.ReturnType)));
                                }
                            }
                            parentScope = scope;

                        }
                        else if (node.VarType.IsUserDefined)
                        {
                            var classInfo = BoinkType.userTypes[node.VarType.Name];

                            foreach (var method in classInfo.Methods)
                            {
                                scope.Add(method.Name, method);
                            }

                            foreach (var field in classInfo.Fields)
                            {
                                scope.Add(field.Name, field);
                            }
                        }
                        parentScope = scope;
                    }

                    SetChildsParentScope(node, parentScope);

                    Visit(node.ChildReference);
                }

                return null;
            }

            if (ProgramLibraryManager.HasStandardLibrary(node.Name))
            {
                var parentScope = ProgramLibraryManager.GetLibrarySymbolTable(node.Name);
                SetChildsParentScope(node, parentScope);

                Visit(node.ChildReference);

                return null;
            }

            // If a variable was not found, throw a Boink error.
            ErrorHandler.Throw(new UndefinedSymbolError($"Variable '{node.Name}' is not defined",
                                                        node.Pos, FilePath));
            return null;

            void SetChildsParentScope(VariableSyntax thisNode, SymbolTable parentScope)
            {
                Type childType = thisNode.ChildReference.GetType();
                if (childType == typeof(VariableSyntax))
                    ((VariableSyntax)thisNode.ChildReference).ParentScope = parentScope;

                else if (childType == typeof(FunctionCallSyntax))
                    ((FunctionCallSyntax)thisNode.ChildReference).Var.ParentScope = parentScope;
            }
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
            string name = (string)node.VarType.TypeToken.Val;
            var userDefinedType = CurrentScope.Lookup(name);
            var builtinType = ObjectType.GetTypeByTokenType(node.VarType.TypeToken.Type);

            BoinkType varType = null;
            if (builtinType != null)
                varType = new BoinkType(builtinType);


            if (varType == null && userDefinedType.GetType() == typeof(ClassSymbol))
            {
                varType = new BoinkType(BoinkType.userTypes[userDefinedType.Name]);
                node.UserDefinedType = varType;
            }


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
                    if (node.Expr.ChildOrOwnType != null && !node.Expr.ChildOrOwnType.IsEqual(varType))
                    {
                        // There is a type mismatch, throw a Boink error.
                        ErrorHandler.Throw(new IncompatibleTypesError($"Type {node.Expr.ChildOrOwnType} and {varType} are not compatible for assignment",
                                                                      node.Pos, FilePath));
                        return null;
                    }

                    // Add log.
                    AddLog($"ASSIGNMENT: Assigned {node.Expr.ChildOrOwnType} to {varType}.");
                }

                return null;
            }

            // Throw a Boink error if it is already defined.
            ErrorHandler.Throw(new MultipleDefinitionError($"Variable {node.Name} is already defined",
                                                           node.Pos, FilePath));
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
                if (node.Expr.ChildOrOwnType.IsEqual(symbol.VarType))
                {
                    // Types match.

                    // Add log.
                    AddLog($"ASSIGNMENT: Assigned {node.Expr.ChildOrOwnType} to {symbol.VarType}.");
                    return null;
                }

                // Throw a Boink error if types don't match.
                ErrorHandler.Throw(new IncompatibleTypesError($"Type {node.Expr.ChildOrOwnType} and {symbol.VarType} are not compatible for assignment",
                                                              node.Pos, FilePath));
                return null;
            }

            // Throw a Boink error if the variable is not defined.
            ErrorHandler.Throw(new UndefinedSymbolError($"Variable '{node.Var.Name}' is not defined",
                                                        node.Var.Pos, FilePath));
            return null;
        }

        /// <summary>
        /// Check types and amounts of arguments.
        /// </summary>
        /// <param name="node">Function call syntax node.</param>
        /// <returns>Null.</returns>
        public override object Visit(FunctionCallSyntax node)
        {
            // Symbol of the referenced variable.
            Symbol symbol;

            // Symbol as a FunctionSymbol.
            FunctionSymbol functionSymbol;

            // If there isn't any parent scope, lookup from the current scope.
            // Otherwise lookup from the parent scope.
            if (node.Var.ParentScope != null)
                symbol = node.Var.ParentScope.Lookup(node.Var.Name);
            else
                symbol = CurrentScope.Lookup(node.Var.Name);

            // Set functionSymbol to "as" casted symbol.
            functionSymbol = (symbol as FunctionSymbol);

            // Function has been defined before.
            if (functionSymbol != null)
            {
                // Set this node's variable type to this function's give type.
                node.VarType = functionSymbol.GiveType;

                // Visit every argument passed in and
                // determine argument types of them.
                var argTypes = new List<BoinkType>();
                foreach (var callArg in node.Args)
                {
                    Visit(callArg);
                    argTypes.Add(callArg.ChildOrOwnType);
                }

                // Check for amount of arguments.
                if (argTypes.Count > functionSymbol.ArgTypes.Count)
                {
                    // Throw a Boink error if there are too many arguments passed in.
                    ErrorHandler.Throw(new ArgumentMismatchError($"Too many arguments for function call",
                                                                 node.Pos, FilePath));
                    return null;
                }
                else if (argTypes.Count < functionSymbol.ArgTypes.Count)
                {
                    // Throw a Boink error if there are too few arguments passed in.
                    ErrorHandler.Throw(new ArgumentMismatchError($"Too few arguments for function call",
                                                                 node.Pos, FilePath));
                    return null;
                }
                else
                {
                    // Check if types of each argument matches.
                    for (int i = 0; i < argTypes.Count; i++)
                    {
                        var callArgType = argTypes[i];
                        var actualArgType = functionSymbol.ArgTypes[i];

                        if (callArgType != null && !callArgType.IsEqual(actualArgType))
                        {
                            // Throw a Boink error if types don't match.
                            ErrorHandler.Throw(new IncompatibleTypesError($"Type {callArgType} and {actualArgType} are not compatible for assignment",
                                                                          node.Pos, FilePath));
                            return null;
                        }

                        AddLog($"CALL: Function {functionSymbol.Name} called with arguments {{{string.Join(", ", functionSymbol.ArgTypes)}}}");
                    }
                }

                return null;
            }

            // Throw a Boink error if the function has not been defined before.
            ErrorHandler.Throw(new UndefinedSymbolError($"Function '{node.Var.Name}' is not defined",
                                                        node.Pos, FilePath));
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
            BoinkType exprType = null;
            if (node.Expr != null)
            {
                // Visit the expression syntax.
                Visit(node.Expr);
                exprType = node.Expr.ChildOrOwnType;
            }

            // Set the currentFunctionSymbol to the function that this syntax gives from.
            var currentFunctionSymbol = CurrentScope.Owner;

            // Check if there is such a function.
            if (currentFunctionSymbol != null)
            {
                // Give type of the function.
                var giveType = ((FunctionSymbol)currentFunctionSymbol).GiveType;

                // Check if there is give type or there is no expression.
                if (giveType != null && (giveType.CSType != null || giveType.UType != null) || node.Expr == null)
                {
                    // Return if give type equals the expression type,
                    // syntax is correct.
                    if (giveType.IsEqual(exprType))
                        return null;

                    // Throw a Boink error if the types don't match.
                    ErrorHandler.Throw(new IncompatibleTypesError($"Type {exprType} and {giveType} are not compatible for giving",
                                                                  node.Pos, FilePath));
                    return null;
                }

                // Throw a Boink error if there is no give type.
                ErrorHandler.Throw(new IncompatibleTypesError($"'give' is not allowed because function '{currentFunctionSymbol.Name}' has no return type",
                                                              node.Pos, FilePath));
                return null;
            }

            // Throw a Boink error if the give is not inside of a function
            // meaning the function is not defined.
            ErrorHandler.Throw(new GiveNotAllowedError($"'give' is not allowed here because it is not inside of a function",
                                                       node.Pos, FilePath));
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
            if (node.Expr.ChildOrOwnType != new BoinkType(typeof(BoolType)))
            {
                // Throw a Boink error if the type is not a bool_.
                ErrorHandler.Throw(new IncompatibleTypesError($"Type {node.Expr.ChildOrOwnType} is not compatible for if preposition",
                                                              node.Expr.Pos, FilePath));
            }

            // Visit each statement of the if syntax.
            foreach (var s in node.Statements)
                Visit(s);

            return null;
        }

        /// <summary>
        /// Import a library or a package if any was found, throw an error otherwise.
        /// </summary>
        /// <param name="node">Import syntax node.</param>
        /// <returns>Null.</returns>
        public override object Visit(ImportSyntax node)
        {
            // Check for the importables in the directory.
            // (user-created files)
            if (DirCache.HasLibraryOrPackage(node.Package.Hierarchy))
            {
                var symbol = DirCache.GetLibraryOrPackageSymbol(node.Package.Hierarchy);
                AddLog($"IMPORT: '{node.Package.HierarchyString}' imported.");
                if (symbol.IsLibrary)
                {
                    CurrentScope.Define((LibrarySymbol)symbol);
                    return null;
                }
                else if (symbol.IsPackage)
                {
                    CurrentScope.Define((PackageSymbol)symbol);
                    return null;
                }
            }

            // Return if there is a standard library as the requested library name.
            if (LibraryManager.StandardLibraries.ContainsKey(node.Package.Root))
            {
                ProgramLibraryManager.LoadStandardLibrary(node.Package.Root);
                return null;
            }

            // Throw a Boink error if no library could be found.
            ErrorHandler.Throw(new UnknownLibraryError($"Library '{node.Package.HierarchyString}' not found",
                                                       node.ImportToken.Pos, FilePath));

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

            if (node.ChildOrOwnType == null)
            {
                ErrorHandler.Throw(new UnsupportedOperationError($"Type {node.Expr.ChildOrOwnType.Name} doesn't support {OperationTypes.GetUnaryOperationByTokenType(node.Operator.Type)}",
                                                                 node.Pos, FilePath));
            }
            return null;
        }

        public override object Visit(BoolLiteralSyntax node) => null;

        public override object Visit(IntLiteralSyntax node) => null;

        public override object Visit(DoubleLiteralSyntax node) => null;

        public override object Visit(StringLiteralSyntax node) => null;

        public override object Visit(FloatLiteralSyntax node) => null;
    }
}
