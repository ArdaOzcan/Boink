using System;
using System.Collections.Generic;
using Boink.Analysis.Tokenization;
using Boink.Errors;
using Boink.Types;

namespace Boink.AST.Nodes
{
    /// <summary>
    /// SyntaxNode that represents a unary operation.
    /// </summary>
    public class UnaryOperationSyntax : SyntaxNode, IParentSyntax, IOperationSyntax, IExpressionSyntax
    {
        public SyntaxNode ChildReference { get; set; }

        public Token Operator { get; }
        
        public SyntaxNode Expr { get; }

        public override int Pos => Operator.Pos;

        public override BoinkType ChildOrOwnType 
        {
            get 
            {
                if(ChildReference != null)
                    return ChildReference.ChildOrOwnType;

                if(OperationTypes.TypeSupportsUnaryOperation(Expr.ChildOrOwnType.CSType, Operator.Type, Pos))
                    return Expr.ChildOrOwnType;
                
                return null;
            }
        }

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var dict = new Dictionary<string, object>();

                dict.Add("Operator", Operator.ToString());
                dict.Add("Expression", Expr.JsonDict);
                result.Add("UnaryOperationSyntax", dict);

                return result;
            }
        }
        
        /// <summary>
        /// Construct a UnaryOperationSyntax object.
        /// </summary>
        /// <param name="op">Operator token.</param>
        /// <param name="expr">Expression as a syntax node.</param>
        public UnaryOperationSyntax(Token op, SyntaxNode expr)
        {
            Operator = op;
            Expr = expr;
        }
    }
}
