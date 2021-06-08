using System;
using System.Collections.Generic;

using Boink.Analysis.Semantic;
using Boink.Analysis.Tokenization;

using Boink.Interpretation;
using Boink.Types;

namespace Boink.AST.Nodes
{
    /// <summary>
    /// SyntaxNode that represents a variable reference.
    /// </summary>
    public class VariableSyntax : SyntaxNode, IParentSyntax
    {        
        public SymbolTable ParentScope { get; set ;}

        public ActivationRecord ParentRecord { get; set ;}

        public SyntaxNode ChildReference { get; set; }

        public Token VarToken { get; private set; }

        public string Name { get; private set; }

        /// <summary>
        /// Type of the variable, set later in semantic analysis 
        /// after visitation. Initial value is null.
        /// </summary>
        public BoinkType VarType { get; set; }

        public override BoinkType ChildOrOwnType 
        {
            get
            {
                if(ChildReference != null)
                    return ChildReference.ChildOrOwnType;

                return VarType;
            }
        } 

        public override int Pos => VarToken.Pos;

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var dict = new Dictionary<string, object>();
                dict.Add("Name", Name);

                if (ChildReference != null)
                    dict.Add("ChildReference", ChildReference.JsonDict);

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
            Name = (string)token.Val;
            VarType = null;
        }
    }
}
