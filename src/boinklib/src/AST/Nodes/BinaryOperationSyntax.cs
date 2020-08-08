using System;
using System.Collections.Generic;
using Boink.Analysis.Tokenization;
using Boink.Types;

namespace Boink.AST.Nodes
{
    /// <summary>
    /// SyntaxNode that represents a binary operation.
    /// <para>
    /// A binary operation means an operation with two operands.
    /// </para>
    /// </summary>
    public sealed class BinaryOperationSyntax : SyntaxNode, IParentSyntax, IOperationSyntax
    {
        public SyntaxNode Left { get; private set; }

        public Token Operator { get; }

        public SyntaxNode Right { get; private set; }

        public SyntaxNode ChildReference { get; set; }

        public override Type Type
        {
            get
            {
                Type leftType = Left.Type;
                Type rightType = Right.Type;

                int pos = Pos;

                return OperationTypes.GetBinaryResultType(leftType, Operator.Type, rightType, pos);
            }
        }

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var members = new Dictionary<string, object>();

                members.Add("Left", Left.JsonDict);

                members.Add("Operator", Operator.ToString());

                members.Add("Right", Right.JsonDict);

                result.Add("BinaryOperationSyntax", members);

                return result;
            }
        }

        public override int Pos => Left.Pos;
        
        /// <summary>
        /// Construct a BinaryOperationSyntax object.
        /// </summary>
        /// <param name="left">Left side of the binary operation.</param>
        /// <param name="op">Token of the binary operator.</param>
        /// <param name="right">Right side of the binary operation.</param>
        public BinaryOperationSyntax(SyntaxNode left, Token op, SyntaxNode right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }
    }
}
