using System;
using System.Collections.Generic;

using Boink.Analysis.Tokenization;
using Boink.Types;

namespace Boink.AST.Nodes
{
    /// <summary>
    /// SyntaxNode that represents a float literal.
    /// </summary>
    public class FloatLiteralSyntax : SyntaxNode, IParentSyntax
    {
        public SyntaxNode ChildReference { get; set; }
        
        public Token LiteralToken { get; private set; }

        public object Val { get; private set; }

        public override int Pos => LiteralToken.Pos;

        public override Type Type => typeof(FloatType);

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                result.Add("FloatLiteralSyntax", Val);
                return result;
            }
        }

        /// <summary>
        /// Construct an FloatLiteralSyntax object.
        /// </summary>
        /// <param name="token">Token of the float literal.</param>
        public FloatLiteralSyntax(Token token)
        {
            LiteralToken = token;
            Val = token.Val;
        }
    }
}
