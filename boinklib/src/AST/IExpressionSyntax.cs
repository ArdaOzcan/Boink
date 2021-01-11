using Boink.AST.Nodes;

namespace Boink.AST
{
    /// <summary>
    /// SyntaxNode that represents an variable assignment.
    /// </summary>
    public interface IExpressionSyntax
    {
        SyntaxNode Expr { get; }
    }
}