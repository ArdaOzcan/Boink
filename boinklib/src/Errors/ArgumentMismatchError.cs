namespace Boink.Errors
{
    public class ArgumentMismatchError : Error
    {
        public ArgumentMismatchError(string msg, int pos, string filePath) : base(msg, pos, filePath) { }
    }
}
