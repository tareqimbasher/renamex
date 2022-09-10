using System.IO;

namespace RenameX.FileSystem;

internal class RealPath : IPath
{
    public string GetTempFileName() => Path.GetTempFileName();
}