using Boink.Analysis.Tokenization;

namespace Boink.AST
{
    internal interface IOperationSyntax
    {
        Token Operator { get; }
    }
}