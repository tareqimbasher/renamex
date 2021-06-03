using System.Collections.Generic;

namespace RenameX.FileSystem
{
    public interface IDirectory
    {
        bool Exists(string path);
        string[] GetDirectories(string path);
        string[] GetFiles(string path);
        string[] GetFiles(string path, string searchPattern);
        IEnumerable<string> EnumerateFiles(string path);
        IEnumerable<string> EnumerateFiles(string path, string searchPattern);
    }
}
