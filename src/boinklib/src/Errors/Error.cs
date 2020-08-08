namespace Boink.Errors
{
    /// <summary>Base class for all errors in Boink language.</summary>
    public class Error
    {
        public string Msg { get; }
        
        public int Pos { get; }

        /// <summary>Construct an Error object.</summary>
        /// <param name="msg">Message of the error.</param>
        /// <param name="pos">1D position of the error.</param>
        public Error(string msg, int pos)
        {
            Msg = msg;
            Pos = pos;
        }
    }
}
