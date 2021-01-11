using System.Collections.Generic;
using Boink.Analysis.Tokenization;

namespace Boink.AST.Nodes
{
    public sealed class ImportSyntax : SyntaxNode
    {
        public Token ImportToken { get; private set; }
        
        public PackageSyntax Package { get; private set; }

        public override int Pos => ImportToken.Pos;

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var dict = new Dictionary<string, object>();

                dict.Add("Package", Package.JsonDict);
                result.Add("ImportSyntax", dict);

                return result;
            }
        }

        /// <summary>
        /// Construct an ImportSyntax object.
        /// </summary>
        /// <param name="importToken">Toke</param>
        /// <param name="package"></param>
        public ImportSyntax(Token importToken, PackageSyntax package)
        {
            ImportToken = importToken;
            Package = package;
        }
    }
}
