using System.Collections.Generic;

using Boink.Analysis.Tokenization;

namespace Boink.AST.Nodes
{

    /// <summary>
    /// SyntaxNode that represents a type definition.
    /// </summary>
    public class TypeDefinitionSyntax : SyntaxNode
    {
        public Token Name { get; private set; }

        public List<SyntaxNode> Statements { get; private set; }
        
        public override int Pos => Name.Pos;

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var dict = new Dictionary<string, object>();
                dict.Add("Name", Name);
                
                var statementJsonList = new List<object>();
                foreach (var statement in Statements)
                    statementJsonList.Add(statement.JsonDict);
                dict.Add("Statements", statementJsonList);

                result.Add("TypeDefinitionSyntax", dict);

                return result;
            }
        }

        /// <summary>
        /// Construct a FunctionSyntax object.
        /// </summary>
        /// <param name="name">Name token of the function.</param>
        /// <param name="giveTypeSyntax">Given type from the function.</param>
        public TypeDefinitionSyntax(Token name)
        {
            Name = name;
            Statements = new List<SyntaxNode>();
        }
    }
}
