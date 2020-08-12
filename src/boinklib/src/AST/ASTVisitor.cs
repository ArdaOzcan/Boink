using System;
using System.Reflection;
using Boink.Analysis.Semantic;
using Boink.AST.Nodes;

namespace Boink.AST
{
    /// <summary>
    /// Abstract Syntax Tree Visitor to traverse in the syntax tree.
    /// <para>
    /// Any class that derives from this can call visit with a SyntaxNode
    /// that calls the corresponding method. There are methods for every SyntaxNode.
    /// </para>
    /// </summary>
    public abstract class ASTVisitor
    {

        [System.Diagnostics.DebuggerStepThrough]
        public object Visit(SyntaxNode node)
        {
            Type nodeType = node.GetType();
            MethodInfo overloadedMethod = GetType().GetMethod("Visit", new Type[] { nodeType });
            return overloadedMethod.Invoke(this, new object[] { node });
        }

        public abstract object Visit(BoolLiteralSyntax node);

        public abstract object Visit(ProgramSyntax node);

        public abstract object Visit(FunctionSyntax node);

        public abstract object Visit(BinaryOperationSyntax node);

        public abstract object Visit(UnaryOperationSyntax node);

        public abstract object Visit(VariableSyntax node);

        public abstract object Visit(ParenthesizedSyntax node);

        public abstract object Visit(DeclarationSyntax node);

        public abstract object Visit(AssignmentSyntax node);

        public abstract object Visit(IntLiteralSyntax node);

        public abstract object Visit(DoubleLiteralSyntax node);

        public abstract object Visit(FunctionCallSyntax node);

        public abstract object Visit(GiveSyntax node);

        public abstract object Visit(IfSyntax node);

        public abstract object Visit(StringLiteralSyntax node);

        public abstract object Visit(ImportSyntax node);
    }
}
