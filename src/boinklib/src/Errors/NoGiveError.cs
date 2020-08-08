namespace Boink.Errors
{
    public class NoGiveError : Error
    {
        public NoGiveError(string msg, int pos) : base(msg, pos) { }
    }
}
