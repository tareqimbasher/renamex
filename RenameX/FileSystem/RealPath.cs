using System.IO;

namespace RenameX.FileSystem
{
    internal class RealPath : IPath
    {
        public string? GetDirectoryName(string? path) => Path.GetDirectoryName(path);

        public string GetFileName(string path) => Path.GetFileName(path);

        public string GetFileNameWithoutExtension(string path) => Path.GetFileNameWithoutExtension(path);

        public string GetTempFileName() => Path.GetTempFileName();
    }
}
