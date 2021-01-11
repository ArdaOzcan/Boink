namespace Boink.Errors
{
    public class UnknownLibraryError : Error
    {
        public UnknownLibraryError(string msg, int pos, string filePath) : base(msg, pos, filePath) { }
    }
}
