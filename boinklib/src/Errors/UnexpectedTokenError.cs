namespace Boink.Errors
{
    public class UnexpectedTokenError : Error
    {
        public UnexpectedTokenError(string msg, int pos, string filePath) : base(msg, pos, filePath) { }
    }
}
