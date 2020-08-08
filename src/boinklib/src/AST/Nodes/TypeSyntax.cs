using System.Collections.Generic;
using Boink.Analysis.Tokenization;

namespace Boink.AST.Nodes
{
    /// <summary>
    /// /// SyntaxNode that represents a type name.
    /// </summary>
    public sealed class TypeSyntax : SyntaxNode
    {
        public override int Pos => TypeToken.Pos;

        public Token TypeToken { get; private set; }

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var dict = new Dictionary<string, object>();

                dict.Add("Token", TypeToken.ToString());
                result.Add("TypeSyntax", dict);

                return result;
            }
        }

        /// <summary>
        /// Construct a TypeSyntax object.
        /// </summary>
        /// <param name="token">Token of the type name.</param>
        public TypeSyntax(Token token)
        {
            TypeToken = token;
        }
    }
}
