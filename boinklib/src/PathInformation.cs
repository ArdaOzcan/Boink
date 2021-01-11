using System.IO;

namespace Boink
{
    public static class PathInformation
    {        
        public readonly static string BoinkExecutablePath = System.Reflection.Assembly.GetEntryAssembly().Location;
        public readonly static string BoinkExecutableDirectory = Path.GetDirectoryName(BoinkExecutablePath);
    }
}
