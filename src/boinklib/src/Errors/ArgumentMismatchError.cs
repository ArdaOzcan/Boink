namespace Boink.Errors
{
    public class ArgumentMismatchError : Error
    {
        public ArgumentMismatchError(string msg, int pos) : base(msg, pos) { }
    }
}
