namespace Boink.Errors
{
    public class UndefinedSymbolError : Error
    {
        public UndefinedSymbolError(string msg, int pos) : base(msg, pos) { }
    }
}
