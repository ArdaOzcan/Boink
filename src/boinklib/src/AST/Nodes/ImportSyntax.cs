using System.Collections.Generic;
using Boink.Analysis.Tokenization;

namespace Boink.AST.Nodes
{
    public sealed class ImportSyntax : SyntaxNode
    {
        public Token ImportToken { get; private set; }
        
        public string LibName { get; private set; }

        public override int Pos => ImportToken.Pos;

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var dict = new Dictionary<string, object>();

                dict.Add("Library Name", LibName);
                result.Add("ImportSyntax", dict);

                return result;
            }
        }

        /// <summary>
        /// Construct an ImportSyntax object.
        /// </summary>
        /// <param name="importToken">Toke</param>
        /// <param name="libName"></param>
        public ImportSyntax(Token importToken, string libName)
        {
            ImportToken = importToken;
            LibName = libName;
        }
    }
}
