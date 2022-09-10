using System.Collections.Generic;
using System.IO;

namespace RenameX.FileSystem;

internal class RealFile : IFile
{
    public bool Exists(string path) => File.Exists(path);
    public void Delete(string path) => File.Delete(path);
    public void Move(string sourceFileName, string destFileName) => File.Move(sourceFileName, destFileName);
    public void Move(string sourceFileName, string destFileName, bool overwrite) => File.Move(sourceFileName, destFileName, overwrite);
    public string ReadAllText(string path) => File.ReadAllText(path);
    public string[] ReadAllLines(string path) => File.ReadAllLines(path);
    public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);
    public void WriteAllLines(string path, string[] contents) => File.WriteAllLines(path, contents);
    public void WriteAllLines(string path, IEnumerable<string> contents) => File.WriteAllLines(path, contents);
}