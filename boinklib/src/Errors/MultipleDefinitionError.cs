namespace Boink.Errors
{
    public class MultipleDefinitionError : Error
    {
        public MultipleDefinitionError(string msg, int pos, string filePath) : base(msg, pos, filePath) { }
    }
}
