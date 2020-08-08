namespace Boink.Errors
{
    public class UnsupportedOperationError : Error
    {
        public UnsupportedOperationError(string msg, int pos) : base(msg, pos) { }
    }
}
