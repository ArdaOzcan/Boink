using System;
using System.Collections.Generic;

namespace Boink.AST.Nodes
{
    /// <summary>
    /// SyntaxNode that represents a function call.
    /// </summary>
    public class FunctionCallSyntax : SyntaxNode, IParentSyntax
    {
        public SyntaxNode ChildReference { get; set; }

        public VariableSyntax Var { get; private set; }

        public List<SyntaxNode> Args { get; private set; }
        
        public Type VarType { get; set; }

        public override int Pos => Var.VarToken.Pos;

        public override Type Type
        {
            get
            {
                if(ChildReference != null)
                    return ChildReference.Type;

                return VarType;
            }
        }

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var dict = new Dictionary<string, object>();
                dict.Add("Name", Var.Name);

                var argumentJsonList = new List<object>();
                foreach (var arg in Args)
                    argumentJsonList.Add(arg.JsonDict);

                dict.Add("Arguments", argumentJsonList);

                result.Add("FunctionCallSyntax", dict);

                return result;
            }
        }
        
        /// <summary>
        /// Construct a FunctionCallSyntax object.
        /// </summary>
        /// <param name="var">Function reference as a VariableSyntax.</param>
        /// <param name="args">List of arguments as syntax nodes.</param>
        public FunctionCallSyntax(VariableSyntax var, List<SyntaxNode> args)
        {
            Var = var;
            Args = args;
            VarType = null;
        }
    }
}
