using System;
using System.Collections.Generic;
using Boink.Analysis.Tokenization;
using Boink.Types;

namespace Boink.AST.Nodes
{

    /// <summary>
    /// SyntaxNode that represents a bool literal.
    /// </summary>
    public class BoolLiteralSyntax : SyntaxNode, IParentSyntax, ILiteralSyntax
    {
        public Token LiteralToken { get; }

        public object Val { get; private set; }

        public override BoinkType ChildOrOwnType 
        {
            get
            {
                if(ChildReference != null)
                    return ChildReference.ChildOrOwnType;
                    
                return new BoinkType(typeof(BoolType));
            }
        }

        public override int Pos => LiteralToken.Pos;

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var dict = new Dictionary<string, object>();
                if (ChildReference != null)
                    dict.Add("Child Reference", ChildReference.JsonDict);

                result.Add("BoolLiteralSyntax", dict);

                return result;
            }
        }

        public SyntaxNode ChildReference { get; set; }

        /// <summary>
        /// Construct an BoolLiteralSyntax object.
        /// </summary>
        /// <param name="token">Token of the bool literal.</param>
        public BoolLiteralSyntax(Token token)
        {
            LiteralToken = token;
            Val = token.Val;
        }
    }
}
