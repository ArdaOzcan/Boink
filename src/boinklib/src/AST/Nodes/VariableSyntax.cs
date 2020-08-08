using System;
using System.Collections.Generic;
using Boink.Analysis.Tokenization;

namespace Boink.AST.Nodes
{
    /// <summary>
    /// SyntaxNode that represents a variable reference.
    /// </summary>
    public class VariableSyntax : SyntaxNode, IParentSyntax
    {
        public SyntaxNode ParentReference { get; set; }
        
        public SyntaxNode ChildReference { get; set; }

        public Token VarToken { get; private set; }

        public string Name { get; private set; }

        /// <summary>
        /// Type of the variable, set later in semantic analysis 
        /// after visitation. Initial value is null.
        /// </summary>
        public Type VarType { get; set; }

        public override Type Type => VarType;

        public override int Pos => VarToken.Pos;

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var dict = new Dictionary<string, object>();
                dict.Add("Name", Name);

                if (ChildReference != null)
                    dict.Add("Child Reference", ChildReference.JsonDict);

                result.Add("VariableSyntax", dict);

                return result;
            }
        }

        /// <summary>
        /// Construct a VariableSyntax object.
        /// </summary>
        /// <param name="token">Token of the variable name.</param>
        public VariableSyntax(Token token)
        {
            VarToken = token;
            Name = (string) token.Val;
            VarType = null;
        }
    }
}
