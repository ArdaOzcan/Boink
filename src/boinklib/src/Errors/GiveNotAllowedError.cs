namespace Boink.Errors
{
    public class GiveNotAllowedError : Error
    {
        public GiveNotAllowedError(string msg, int pos) : base(msg, pos) { }
    }
}
