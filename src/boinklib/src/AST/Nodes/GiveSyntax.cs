using System.Collections.Generic;
using Boink.Analysis.Tokenization;

namespace Boink.AST.Nodes
{

    public sealed class GiveSyntax : SyntaxNode, IExpressionSyntax
    {
        public Token GiveToken { get; private set; }
        
        public SyntaxNode Expr { get; }

        public override int Pos => GiveToken.Pos;

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var dict = new Dictionary<string, object>();
                if (Expr != null)
                    dict.Add("Expression", Expr.JsonDict);

                result.Add("GiveSyntax", dict);

                return result;
            }
        }

        public GiveSyntax(Token token, SyntaxNode expr)
        {
            GiveToken = token;
            Expr = expr;
        }
    }
}
