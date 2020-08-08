namespace Boink.Errors
{
    public class MultipleDefinitionError : Error
    {
        public MultipleDefinitionError(string msg, int pos) : base(msg, pos) { }
    }
}
