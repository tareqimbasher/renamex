using System.IO;

namespace RenameX.FileSystem
{
    public interface IPath
    {
        string? GetDirectoryName(string? path) => Path.GetDirectoryName(path);
        string GetFileName(string path) => Path.GetFileName(path);
        string GetFileNameWithoutExtension(string path) => Path.GetFileNameWithoutExtension(path);
        string Combine(params string[] paths) => Path.Combine(paths);
        string GetTempFileName();
    }
}
