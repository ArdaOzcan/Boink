using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Boink.Analysis.Tokenization;
using Boink.AST;
using Boink.AST.Nodes;
using Boink.Interpretation.Library;
using Boink.Types;

namespace Boink.Interpretation
{

    /// <summary>
    /// Interpreter of the program. Derives from ASTVisitor.
    /// </summary>
    public class Interpreter : ASTVisitor
    {
        public CallStack ProgramCallStack { get; }

        public LibraryManager ProgramLibraryManager { get; }

        public bool Verbose { get; private set; }

        public DirectoryCache DirCache { get; }

        public string ProgramDirectory { get; }

        /// <summary>
        /// Construct an Interpreter object.
        /// </summary>
        public Interpreter(string programDirectory, DirectoryCache dirCache)
        {
            DirCache = dirCache;
            ProgramCallStack = new CallStack();
            ProgramLibraryManager = new LibraryManager();
            ProgramDirectory = programDirectory;
        }

        /// <param name="root">Root node of the program a.k.a. the program itself.</param>
        public ActivationRecord Interpret(ProgramSyntax root, bool verbose = true)
        {
            Verbose = verbose;
            // Start the tree traversal with the root, a ProgramSyntax.
            return (ActivationRecord)Visit(root);
        }

        public void WriteLineIfVerbose(string text)
        {
            if (Verbose) Console.WriteLine(text);
        }

        /// <summary>
        /// Create an ActivationRecord and visit every statement.
        /// </summary>
        /// <param name="node">Program syntax node.</param>
        /// <returns></returns>
        public override object Visit(ProgramSyntax node)
        {
            string programName = node.Name;

            ActivationRecord ar = new ActivationRecord(
                name: programName,
                nestingLevel: 1,
                parentRecord: null
            );

            ProgramCallStack.Push(ar);
            WriteLineIfVerbose($"------- START OF FUNCTION {programName} -------");
            foreach (SyntaxNode s in node.Statements)
                Visit(s);

            WriteLineIfVerbose($"-------- END OF FUNCTION {programName} --------");
            WriteLineIfVerbose(ProgramCallStack.ToString());

            // ProgramCallStack.Pop();

            return ProgramCallStack.Pop();
        }

        /// <summary>
        /// Visit expression if there is any and define a variable
        /// in the activation record.
        /// </summary>
        /// <param name="node">Declaration syntax node.</param>
        /// <returns>Null.</returns>
        public override object Visit(DeclarationSyntax node)
        {
            Type varType = ObjectType.GetTypeByTokenType(node.VarType.TypeToken.Type);
            ConstructorInfo ctorinfo = varType.GetConstructor(new[] { typeof(string), typeof(object) });

            ObjectType var = null;
            object val = null;
            if (node.Expr != null)
            {
                var = (ObjectType)Visit(node.Expr);
                val = var.Val;
            }

            ActivationRecord ar = ProgramCallStack.Peek;
            ar.DefineVar(node.Name, (ObjectType)ctorinfo.Invoke(new object[] { node.Name, val }));
            return null;
        }

        /// <summary>
        /// Create an activation record, set arguments and visit 
        /// every statement.
        /// </summary>
        /// <param name="node">Function call syntax node.</param>
        /// <returns>Give value of the function if there is any.</returns>
        public override object Visit(FunctionCallSyntax node)
        {
            string funcName = node.Var.Name;

            ActivationRecord parentRecord;

            if (node.Var.ParentRecord == null)
                parentRecord = ProgramCallStack.Peek;
            else
                parentRecord = node.Var.ParentRecord;

            ObjectType function = parentRecord.GetVar(funcName);
            if(function.GetType() == typeof(FunctionType))
                return VisitUserDefinedFunction((FunctionType)function);
            else
                return VisitStandardFunction((StandardFunctionType)function);

            object VisitStandardFunction(StandardFunctionType func)
            {
                ParameterInfo[] parameters = ((MethodInfo)func.Val).GetParameters();
                List<SyntaxNode> passedArgs = node.Args;
                object[] arguments = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    SyntaxNode passedArg = passedArgs[i];

                    Type varType = parameters[i].ParameterType;

                    ConstructorInfo ctorinfo = varType.GetConstructor(new[] { typeof(string), typeof(object) });

                    ObjectType var = (ObjectType)Visit(passedArg);

                    object val = null;
                    if (var != null)
                        val = var.Val;

                    arguments[i] = (ObjectType)ctorinfo.Invoke(new object[] { parameters[i].Name, val });
                }

                WriteLineIfVerbose($"------- START OF FUNCTION {funcName} -------");

                var giveVal = func.InvokeFunction(arguments);

                WriteLineIfVerbose($"-------- END OF FUNCTION {funcName} --------");

                return giveVal;
            }

            object VisitUserDefinedFunction(FunctionType func)
            {
                ActivationRecord functionRecord = new ActivationRecord(
                                name: funcName,
                                nestingLevel: parentRecord.NestingLevel + 1,
                                parentRecord: parentRecord,
                                owner: func
                );

                List<DeclarationSyntax> argDecls = func.Args;
                List<SyntaxNode> passedArgs = node.Args;

                for (int i = 0; i < argDecls.Count; i++)
                {
                    DeclarationSyntax argDecl = argDecls[i];
                    SyntaxNode passedArg = passedArgs[i];

                    TokenType tokenType = argDecl.VarType.TypeToken.Type;
                    Type varType = ObjectType.GetTypeByTokenType(tokenType);

                    ConstructorInfo ctorinfo = varType.GetConstructor(new[] { typeof(string), typeof(object) });

                    ObjectType var = (ObjectType)Visit(passedArg);

                    object val = null;
                    if (var != null)
                        val = var.Val;

                    functionRecord.DefineVar(argDecl.Name, (ObjectType)ctorinfo.Invoke(new object[] { argDecl.Name, val }));
                }

                ProgramCallStack.Push(functionRecord);
                WriteLineIfVerbose($"------- START OF FUNCTION {funcName} -------");

                foreach (SyntaxNode s in (List<SyntaxNode>)func.Val)
                {
                    Visit(s);
                    if (func.Gave)
                    {
                        func.Gave = false;
                        break;
                    }
                }

                WriteLineIfVerbose($"-------- END OF FUNCTION {funcName} --------");
                WriteLineIfVerbose(ProgramCallStack.ToString());

                ProgramCallStack.Pop();
                return func.GiveVal;
            }
        }

        /// <summary>
        /// Define a function in the last activation record.
        /// </summary>
        /// <param name="node">Function declaration syntax node.</param>
        /// <returns>Null.</returns>
        public override object Visit(FunctionSyntax node)
        {
            ActivationRecord ar = ProgramCallStack.Peek;
            string val = (string)node.Name.Val;
            ar.DefineVar((string)node.Name.Val, new FunctionType(val, node.Statements, node.Args));

            return null;
        }

        /// <summary>
        /// Get the variable and return the value.
        /// </summary>
        /// <param name="node">Variable reference syntax node.</param>
        /// <returns>Variable as type obj_</returns>
        public override object Visit(VariableSyntax node)
        {
            string varName = node.Name;

            ObjectType var;

            if(node.ParentRecord == null)
                var = ProgramCallStack.Peek.GetVar(varName);
            else
                var = node.ParentRecord.GetVar(varName);

            if(node.ChildReference != null)
            {
                Type type = var.GetType();
                if (type == typeof(PackageType) || type == typeof(LibraryType))
                {
                    Type childType = node.ChildReference.GetType();
                    ActivationRecord record = (ActivationRecord)var.Val;

                    if (childType == typeof(VariableSyntax))
                        ((VariableSyntax)node.ChildReference).ParentRecord = record;
    
                    else if(childType == typeof(FunctionCallSyntax))
                        ((FunctionCallSyntax)node.ChildReference).Var.ParentRecord = record;
                    
                }

                var = (ObjectType)Visit(node.ChildReference);
            }

            return var;
        }

        /// <summary>
        /// Visit both the left and right side and return the
        /// evaluated version.
        /// </summary>
        /// <param name="node">Binary operation syntax node.</param>
        /// <returns>Evaluated result.</returns>
        public override object Visit(BinaryOperationSyntax node)
        {
            ObjectType left = (ObjectType)Visit(node.Left);
            ObjectType right = (ObjectType)Visit(node.Right);

            switch (node.Operator.Type)
            {
                case TokenType.Plus:
                    return left.add(right);
                case TokenType.Minus:
                    return left.subtract(right);
                case TokenType.Star:
                    return left.multiply(right);
                case TokenType.Slash:
                    return left.divide(right);
                case TokenType.AmpersandAmpersand:
                    return left.and(right);
                case TokenType.PipePipe:
                    return left.or(right);
                case TokenType.Greater:
                    return left.greater(right);
                case TokenType.GreaterEquals:
                    return left.greaterEquals(right);
                case TokenType.Less:
                    return left.less(right);
                case TokenType.LessEquals:
                    return left.lessEquals(right);
                case TokenType.EqualsEquals:
                    return left.equalsEquals(right);
                default:
                    return null;
            };
        }

        /// <summary>
        /// Get the variable and set the value.
        /// </summary>
        /// <param name="node">Assignment syntax node.</param>
        /// <returns>Null.</returns>
        public override object Visit(AssignmentSyntax node)
        {
            string varName = node.Var.Name;
            ObjectType varValue = (ObjectType)Visit(node.Expr);

            ActivationRecord ar = ProgramCallStack.Peek;
            ar.SetVar(varName, varValue);

            return null;
        }

        /// <summary>
        /// Return the Boink int_ object.
        /// </summary>
        /// <param name="node">Int literal syntax node.</param>
        /// <returns>Boink int_ object.</returns>
        public override object Visit(IntLiteralSyntax node) => new IntType(null, (int)node.Val);

        /// <summary>
        /// Return the Boink float_ object.
        /// </summary>
        /// <param name="node">Float literal syntax node.</param>
        /// <returns>Boink float_ object.</returns>
        public override object Visit(DoubleLiteralSyntax node) => new DoubleType(null, (double)node.Val);

        /// <summary>
        /// Return the Boink bool_ object.
        /// </summary>
        /// <param name="node">Bool literal syntax node.</param>
        /// <returns>Boink bool_ object.</returns>
        public override object Visit(BoolLiteralSyntax node) => new BoolType(null, (bool)node.Val);

        /// <summary>
        /// Call give on the current function.
        /// </summary>
        /// <param name="node">Give syntax node.</param>
        /// <returns>Null.</returns>
        public override object Visit(GiveSyntax node)
        {
            FunctionType func_symbol = ProgramCallStack.Peek.Owner;
            if (node.Expr == null)
            {
                func_symbol.Give(null);
                return null;
            }

            func_symbol.Give(Visit(node.Expr));
            return null;
        }

        /// <summary>
        /// Evaluate expression and visit all statements is necessary.
        /// </summary>
        /// <param name="node">If syntax node.</param>
        /// <returns>Null.</returns>
        public override object Visit(IfSyntax node)
        {
            BoolType prepositionVal = (BoolType)Visit(node.Expr);
            if (prepositionVal == null)
                return null;

            if ((bool)prepositionVal.Val)
            {
                foreach (SyntaxNode s in node.Statements)
                    Visit(s);
            }
            return null;
        }

        /// <summary>
        /// Visit the expression.
        /// </summary>
        /// <param name="node">Parenthesized syntax node.</param>
        /// <returns>What is returned from the expression.</returns>
        public override object Visit(ParenthesizedSyntax node) => Visit(node.Expr);

        public override object Visit(StringLiteralSyntax node) => new StringType(null, (string)node.Val);

        public override object Visit(ImportSyntax node)
        {
            if(DirCache.HasLibraryOrPackage(node.Package.Hierarchy))
            {
                var importable = DirCache.GetLibraryOrPackageObject(node.Package.Hierarchy);
                ProgramCallStack.Peek.DefineVar(importable.Name, importable);
                
                return null;
            }

            var stdlib = LibraryManager.GetStandardLibraryObject(node.Package.Root);
            ProgramCallStack.Peek.DefineVar(stdlib.Name, stdlib);
                        
            return null;
        }

        public override object Visit(UnaryOperationSyntax node)
        {
            ObjectType obj = (ObjectType)Visit(node.Expr);
            switch (node.Operator.Type)
            {
                case TokenType.Plus:
                    return obj.positive();
                case TokenType.Minus:
                    return obj.negative();
                default:
                    return null;
            }
        }

        public override object Visit(FloatLiteralSyntax node) => new FloatType(null, (float)node.Val);
    }

}
