namespace Boink.Errors
{
    public class UndefinedSymbolError : Error
    {
        public UndefinedSymbolError(string msg, int pos, string filePath) : base(msg, pos, filePath) { }
    }
}
