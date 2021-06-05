using System.Collections.Generic;

namespace RenameX.FileSystem
{
    public interface IFile
    {
        bool Exists(string path);
        void Delete(string path);
        void Move(string sourceFileName, string destFileName);
        void Move(string sourceFileName, string destFileName, bool overwrite);
        string ReadAllText(string path);
        string[] ReadAllLines(string path);
        void WriteAllText(string path, string contents);
        void WriteAllLines(string path, string[] contents);
        void WriteAllLines(string path, IEnumerable<string> contents);
    }
}
