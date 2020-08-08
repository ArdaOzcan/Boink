namespace Boink.Errors
{
    public class UnknownLibraryError : Error
    {
        public UnknownLibraryError(string msg, int pos) : base(msg, pos) { }
    }
}
