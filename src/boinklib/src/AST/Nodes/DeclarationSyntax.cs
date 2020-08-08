using System;
using System.Collections.Generic;
using Boink.Analysis.Tokenization;
using Boink.Types;

namespace Boink.AST.Nodes
{
    /// <summary>
    /// SyntaxNode that represents a variable declaration.
    /// </summary>
    public class DeclarationSyntax : SyntaxNode, IExpressionSyntax
    {
        public override int Pos => VarType.Pos;

        public TypeSyntax VarType { get; private set; }

        public string Name { get; private set; }

        public SyntaxNode Expr { get; }

        public override Type Type
        {
            get
            {
                switch (VarType.TypeToken.Type)
                {
                    case TokenType.DynamicType:
                        return typeof(dyn_);
                    case TokenType.IntType:
                        return typeof(int_);
                    case TokenType.BoolType:
                        return typeof(bool_);
                    case TokenType.FloatType:
                        return typeof(float_);
                    default:
                        return null;
                }
            }
        }

        public override Dictionary<string, object> JsonDict
        {
            get
            {
                var result = new Dictionary<string, object>();
                var dict = new Dictionary<string, object>();

                dict.Add("Variable Type", VarType.JsonDict);
                dict.Add("Name", Name);
                if (Expr != null)
                    dict.Add("Expression", Expr.JsonDict);

                result.Add("DeclarationSyntax", dict);

                return result;
            }
        }


        /// <summary>
        /// Construct a DeclarationSyntax object.
        /// </summary>
        /// <param name="varType">Syntax node of the variable type.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="expr">Value to be assigned to the variable is there is any.</param>
        public DeclarationSyntax(TypeSyntax varType, string name, SyntaxNode expr)
        {
            VarType = varType;
            Name = name;
            Expr = expr;
        }
    }
}
