namespace Boink.Analysis.Tokenization
{
    /// <summary>
    /// Simple data structure to hold information about a token.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Type of the token.
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// Value of the token.
        /// </summary>
        public object Val { get; }

        /// <summary>
        /// Position of the token in the text.
        /// </summary>
        public int Pos { get; }

        /// <summary>
        /// Construct a Token object.
        /// </summary>
        /// <param name="type">Type of the token.</param>
        /// <param name="val">Value of the Token, usually the string that the token refers to.</param>
        /// <param name="pos">1D starting position of the token.</param>
        public Token(TokenType type, object val, int pos)
        {
            Type = type;
            Val = val;
            Pos = pos;
        }

        public override string ToString() => $"<{Type.ToString()}: '{Val}'>";
    }
}
