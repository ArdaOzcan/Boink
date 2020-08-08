using Boink.AST.Nodes;

namespace Boink.AST
{
    public interface IParentSyntax
    {
        SyntaxNode ChildReference { get; set; }
    }
}
