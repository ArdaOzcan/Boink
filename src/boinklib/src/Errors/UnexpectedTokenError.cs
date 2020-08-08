namespace Boink.Errors
{
    public class UnexpectedTokenError : Error
    {
        public UnexpectedTokenError(string msg, int pos) : base(msg, pos) { }
    }
}
