namespace Boink.Errors
{
    public class UnsupportedOperationError : Error
    {
        public UnsupportedOperationError(string msg, int pos, string filePath) : base(msg, pos, filePath) { }
    }
}
