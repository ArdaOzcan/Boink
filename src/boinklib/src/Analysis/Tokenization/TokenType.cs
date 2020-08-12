namespace Boink.Analysis.Tokenization
{
    /// <summary>Enum to hold information about a token's type. Derives from Enum class.</summary>
    public enum TokenType
    {
        /// <summary> End of file </summary>    
        EndOfFile,
        /// <summary> \n </summary>
        NewLine,
        /// <summary> 0, 1, 2, 123, 314 ... </summary>
        IntLiteral,
        /// <summary> 0.123123, 3.1415, 2.72 ... </summary>
        DoubleLiteral,
        /// <summary> + </summary>
        Plus,
        /// <summary> - </summary>
        Minus,
        /// <summary> * </summary>
        Star,
        /// <summary> / </summary>
        Slash,
        /// <summary> && </summary>
        AmpersandAmpersand,
        /// <summary> || </summary>
        PipePipe,
        /// <summary> hello, lol, x, y, z ... </summary>
        Word,
        /// <summary> fn </summary>
        FunctionDefinition,
        /// <summary> = </summary>
        Equals,
        /// <summary> ( </summary>
        LeftParenthesis,
        /// <summary> ) </summary>
        RightParenthesis,
        /// <summary> ; </summary>
        Semicolon,
        /// <summary> dyn </summary>
        DynamicType,
        /// <summary> int </summary>
        IntType,
        /// <summary> , </summary>
        Comma,
        /// <summary> true, false </summary>
        BoolLiteral,
        /// <summary> bool </summary>
        BoolType,
        /// <summary> float </summary>
        FloatType,
        /// <summary> -> </summary>
        RightArrow,
        /// <summary> give </summary>
        Give,
        /// <summary> if </summary>
        If,
        /// <summary> > </summary>
        Greater,
        /// <summary> >= </summary>
        GreaterEquals,
        /// <summary> < </summary>
        Less,
        /// <summary> <= </summary>
        LessEquals,
        /// <summary> == </summary>
        EqualsEquals,
        /// <summary> "hello" </summary>
        StringLiteral,
        /// <summary> string </summary>
        StringType,
        /// <summary> import </summary>
        Import,
        /// <summary> . </summary>
        Dot,
        /// <summary> double </summary>
        DoubleType
    }

}
