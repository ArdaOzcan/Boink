namespace Boink.Errors
{
    public class GiveNotAllowedError : Error
    {
        public GiveNotAllowedError(string msg, int pos, string filePath) : base(msg, pos, filePath) { }
    }
}
