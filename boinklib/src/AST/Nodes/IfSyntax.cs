using System.Collections.Generic;

using Boink.Analysis.Tokenization;

namespace Boink.AST.Nodes
{

    /// <summary>
    /// SyntaxNode that represents an if statement.
    /// </summary>
    public sealed class IfSyntax : SyntaxNode, IExpressionSyntax
    {
        public Token IfToken { get; private set; }

        public SyntaxNode Expr { get; }
        
        public List<SyntaxNode> Statements { get; private set; }

        public override int Pos => IfToken.Pos;

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var dict = new Dictionary<string, object>();
                if (Expr != null)
                    dict.Add("Expression", Expr.JsonDict);

                dict.Add("Statements", Statements);
                result.Add("IfSyntax", dict);

                return result;
            }
        }

        /// <summary>
        /// Construct an IfSyntax object.
        /// </summary>
        /// <param name="token">Token of the word 'if'.</param>
        /// <param name="expr">Boolean preposition of the if statement.</param>
        public IfSyntax(Token token, SyntaxNode expr)
        {
            IfToken = token;
            Expr = expr;
            Statements = new List<SyntaxNode>();
        }
    }
}
