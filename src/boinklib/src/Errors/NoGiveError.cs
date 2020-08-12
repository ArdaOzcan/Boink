namespace Boink.Errors
{
    public class NoGiveError : Error
    {
        public NoGiveError(string msg, int pos, string filePath) : base(msg, pos, filePath) { }
    }
}
