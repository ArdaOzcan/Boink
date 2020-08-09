using System.Collections.Generic;

namespace Boink.AST.Nodes
{
    public class PackageSyntax : SyntaxNode
    {
        public override int Pos { get; }
        public List<string> Hierarchy { get; }

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var dict = new Dictionary<string, object>();

                dict.Add("Hierarchy", Hierarchy);
                result.Add("ImportSyntax", dict);

                return result;
            }
        }

        public string HierarchyString 
        { 
            get
            {
                return string.Join(".", Hierarchy);
            }
        }

        public PackageSyntax(List<string> hierarchy)
        {
            Hierarchy = hierarchy;
        }
    }
}