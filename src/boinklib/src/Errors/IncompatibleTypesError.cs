namespace Boink.Errors
{
    public class IncompatibleTypesError : Error
    {
        public IncompatibleTypesError(string msg, int pos, string filePath) : base(msg, pos, filePath) { }
    }
}
