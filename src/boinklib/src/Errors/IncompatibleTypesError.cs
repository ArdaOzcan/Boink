namespace Boink.Errors
{
    public class IncompatibleTypesError : Error
    {
        public IncompatibleTypesError(string msg, int pos) : base(msg, pos) { }
    }
}
