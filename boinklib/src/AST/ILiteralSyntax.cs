using Boink.Analysis.Tokenization;

namespace Boink.AST
{
    internal interface ILiteralSyntax
    {
        Token LiteralToken { get; }
    }
}