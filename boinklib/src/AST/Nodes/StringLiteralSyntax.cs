using System;
using System.Collections.Generic;
using Boink.Analysis.Tokenization;
using Boink.Types;

namespace Boink.AST.Nodes
{
    /// <summary>
    /// SyntaxNode that represents a bool literal.
    /// </summary>
    public class StringLiteralSyntax : SyntaxNode, IParentSyntax, ILiteralSyntax
    {
        public SyntaxNode ChildReference { get; set; }

        public Token LiteralToken { get; }
        
        public object Val { get; private set; }

        public override int Pos => LiteralToken.Pos;

        public override BoinkType ChildOrOwnType 
        {
            get
            {
                if(ChildReference != null)
                    return ChildReference.ChildOrOwnType;
                    
                return new BoinkType(typeof(StringType));
            }
        }

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var dict = new Dictionary<string, object>();

                dict.Add("Value", Val.ToString());
                result.Add("StringLiteralSyntax", dict);

                return result;
            }
        }

        /// <summary>
        /// Construct an StringLiteralSyntax object.
        /// </summary>
        /// <param name="token">Token of the bool literal.</param>
        public StringLiteralSyntax(Token token)
        {
            LiteralToken = token;
            Val = token.Val;
        }
    }
}
