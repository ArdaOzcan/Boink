using System.Collections.Generic;

namespace Boink.AST.Nodes
{
    /// <summary>
    /// SyntaxNode that represents the program.
    /// </summary>
    public sealed class ProgramSyntax : SyntaxNode
    {
        public string Name { get; private set; }
        
        public List<SyntaxNode> Statements { get; private set; }

        public override int Pos => Statements[0].Pos;

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var dict = new Dictionary<string, object>();

                var statementJsonList = new List<object>();
                foreach (var statement in Statements)
                    statementJsonList.Add(statement.JsonDict);
                
                dict.Add("Statements", statementJsonList);
                result.Add("ProgramSyntax", dict);

                return result;
            }
        }

        /// <summary>
        /// Construct a ProgramSyntax object.
        /// </summary>
        /// <param name="name">Name of the program.</param>
        public ProgramSyntax(string name)
        {
            Name = name;
            Statements = new List<SyntaxNode>();
        }
    }
}
