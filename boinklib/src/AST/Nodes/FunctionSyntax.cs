using System.Collections.Generic;

using Boink.Analysis.Tokenization;

namespace Boink.AST.Nodes
{

    /// <summary>
    /// SyntaxNode that represents a function declaration.
    /// </summary>
    public class FunctionSyntax : SyntaxNode
    {
        public Token Name { get; private set; }

        public TypeSyntax GiveTypeSyntax { get; private set; }

        public List<SyntaxNode> Statements { get; private set; }
        
        public List<DeclarationSyntax> Args { get; private set; }

        public override int Pos => Name.Pos;

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var dict = new Dictionary<string, object>();
                dict.Add("Name", Name);

                var argumentJsonList = new List<object>();
                foreach (var arg in Args)
                    argumentJsonList.Add(arg.JsonDict);
                dict.Add("Arguments", argumentJsonList);
                
                var statementJsonList = new List<object>();
                foreach (var statement in Statements)
                    statementJsonList.Add(statement.JsonDict);
                dict.Add("Statements", statementJsonList);

                result.Add("FunctionSyntax", dict);

                return result;
            }
        }

        /// <summary>
        /// Construct a FunctionSyntax object.
        /// </summary>
        /// <param name="name">Name token of the function.</param>
        /// <param name="giveTypeSyntax">Given type from the function.</param>
        public FunctionSyntax(Token name, TypeSyntax giveTypeSyntax)
        {
            Name = name;
            GiveTypeSyntax = giveTypeSyntax;
            Statements = new List<SyntaxNode>();
            Args = new List<DeclarationSyntax>();
        }
    }
}
