using System.Collections.Generic;

namespace Boink.AST.Nodes
{
    /// <summary>
    /// SyntaxNode that represents an variable assignment.
    /// </summary>
    public class AssignmentSyntax : SyntaxNode, IExpressionSyntax
    {
        public VariableSyntax Var { get; private set; }
        
        public SyntaxNode Expr { get; }

        public override int Pos => Var.Pos;

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();

                var dict = new Dictionary<string, object>();
                dict.Add(Var.ToString(), Expr.JsonDict);

                result.Add("AssignmentSyntax", dict);

                return result;
            }
        }

        /// <summary>
        /// Construct an AssignmentSyntax object.
        /// </summary>
        /// <param name="var">Variable reference as a SyntaxNode.</param>
        /// <param name="expr">Expression as a SyntaxNode.</param>
        public AssignmentSyntax(VariableSyntax var, SyntaxNode expr)
        {
            Var = var;
            Expr = expr;
        }
    }
}
