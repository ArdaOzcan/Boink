namespace Boink.Errors
{
    public class UnknownTokenError : Error
    {
        public UnknownTokenError(string msg, int pos, string filePath) : base(msg, pos, filePath) { }
    }
}
