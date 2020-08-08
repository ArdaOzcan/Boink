using System.Collections.Generic;

namespace Boink.Logging
{
    /// <summary>
    /// Provide an interface for classes that can hold and write log information.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Hold all logs as strings.
        /// </summary>
        /// <value>A log as a string.</value>
        List<string> Logs { get; set; }
        void WriteAll();
        void AddLog(string log);
    }
}
