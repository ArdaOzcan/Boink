using System;
using System.Collections.Generic;
using Boink.Analysis.Tokenization;
using Boink.Types;

namespace Boink.AST.Nodes
{
    /// <summary>
    /// SyntaxNode that represents an integer literal.
    /// </summary>
    public class IntLiteralSyntax : SyntaxNode, IParentSyntax, ILiteralSyntax
    {
        public SyntaxNode ChildReference { get; set; }

        public override int Pos => LiteralToken.Pos;

        public override BoinkType ChildOrOwnType 
        {
            get
            {
                if(ChildReference != null)
                    return ChildReference.ChildOrOwnType;
                    
                return new BoinkType(typeof(IntType));
            }
        }

        public Token LiteralToken { get; }

        public object Val { get; private set; }
        
        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var dict = new Dictionary<string, object>();

                dict.Add("Value", Val.ToString());
                result.Add("IntLiteralSyntax", dict);

                return result;
            }
        }
        
        /// <summary>
        /// Construct an IntLiteralSyntax object.
        /// </summary>
        /// <param name="token">Token of the int literal.</param>
        public IntLiteralSyntax(Token token)
        {
            LiteralToken = token;
            Val = token.Val;
        }
    }
}
