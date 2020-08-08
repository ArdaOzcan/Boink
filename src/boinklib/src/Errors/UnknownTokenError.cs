namespace Boink.Errors
{
    public class UnknownTokenError : Error
    {
        public UnknownTokenError(string msg, int pos) : base(msg, pos) { }
    }
}
