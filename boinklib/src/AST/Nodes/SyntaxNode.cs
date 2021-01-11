using System;
using System.Collections.Generic;

namespace Boink.AST.Nodes
{
    /// <summary>
    /// Base class for all syntax nodes.
    /// </summary>
    public abstract class SyntaxNode
    {
        /// <summary>
        /// Start position of the syntax.
        /// </summary>
        /// <returns>1D start position of the syntax node.</returns>
        public abstract int Pos { get; }

        /// <summary>
        /// Return the type of the syntax if any.
        /// <para>### Example:
        /// <code>
        /// (1 + 2)
        /// </code>
        /// -> BinaryOperationSyntax: int + int, Type == int
        /// <code>
        /// (1 / 2)
        /// </code>
        /// -> BinaryOperationSyntax: int / int, Type == float
        /// </para>
        /// </summary>
        public virtual Type Type => null;

        /// <summary>
        /// Dictionary representation of this syntax node as a Dictionary.
        /// </summary>
        public virtual Dictionary<string, object> JsonDict => null;
    }
}
