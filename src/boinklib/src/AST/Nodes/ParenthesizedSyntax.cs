using System;
using System.Collections.Generic;

namespace Boink.AST.Nodes
{
    /// <summary>
    /// SyntaxNode that represents a parenthesized expression.
    /// </summary>
    public class ParenthesizedSyntax : SyntaxNode, IParentSyntax, IExpressionSyntax
    {
        public SyntaxNode ChildReference { get; set; }

        public SyntaxNode Expr { get; }

        public override int Pos => Expr.Pos - 1;

        public override Type Type => Expr.Type;

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var dict = new Dictionary<string, object>();

                dict.Add("Expression", Expr.JsonDict);
                result.Add("ParenthesizedSyntax", dict);

                return result;
            }
        }

        /// <summary>
        /// Construct a ParenthesizedSyntax object.
        /// </summary>
        /// <param name="expr">Expression as a SyntaxNode.</param>
        public ParenthesizedSyntax(SyntaxNode expr)
        {
            Expr = expr;
        }
    }
}
