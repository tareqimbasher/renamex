using System.Collections.Generic;
using System.IO;

namespace RenameX.FileSystem;

internal class RealDirectory : IDirectory
{
    public bool Exists(string path) => Directory.Exists(path);
    public string[] GetDirectories(string path) => Directory.GetDirectories(path);
    public string[] GetFiles(string path) => Directory.GetFiles(path);
    public string[] GetFiles(string path, string searchPattern) => Directory.GetFiles(path, searchPattern);
    public IEnumerable<string> EnumerateFiles(string path) => Directory.EnumerateFiles(path);
    public IEnumerable<string> EnumerateFiles(string path, string searchPattern) => Directory.EnumerateFiles(path, searchPattern);
}