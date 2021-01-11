using System;
using System.Collections.Generic;

using Boink.Analysis.Tokenization;
using Boink.Types;

namespace Boink.AST.Nodes
{
    /// <summary>
    /// SyntaxNode that represents a double literal.
    /// </summary>
    public class DoubleLiteralSyntax : SyntaxNode, IParentSyntax
    {
        public SyntaxNode ChildReference { get; set; }
        
        public Token LiteralToken { get; private set; }

        public object Val { get; private set; }

        public override int Pos => LiteralToken.Pos;

        public override Type Type => typeof(DoubleType);

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                result.Add("DoubleLiteralSyntax", Val);
                return result;
            }
        }

        /// <summary>
        /// Construct an FloatLiteralSyntax object.
        /// </summary>
        /// <param name="token">Token of the float literal.</param>
        public DoubleLiteralSyntax(Token token)
        {
            LiteralToken = token;
            Val = token.Val;
        }
    }
}
