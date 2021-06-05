using RenameX.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RenameX.Tests.Infrastructure
{
    public class MemoryFileAccessor : IFile
    {
        private readonly MemoryDrive _drive;

        public MemoryFileAccessor(MemoryDrive drive)
        {
            _drive = drive;
        }

        public bool Exists(string path)
        {
            var dirPath = MemoryPathAccessor.GetDirectoryNameDefault(path);
            return _drive.GetDirectory(dirPath).Files.Any(f => f.FullName == path);
        }

        public void Delete(string path)
        {
            var file = _drive.GetFile(path);
            var directory = _drive.GetDirectory(MemoryPathAccessor.GetDirectoryNameDefault(path));
            directory.Files.Remove(file);
        }

        public void Move(string sourceFileName, string destFileName)
        {
            Move(sourceFileName, destFileName, true);
        }

        public void Move(string sourceFileName, string destFileName, bool overwrite)
        {
            bool destFileExists = _drive.FileExists(destFileName);

            if (destFileExists && !overwrite)
                throw new InvalidOperationException($"{nameof(destFileName)} already exists.");

            var sourceFile = _drive.GetFile(sourceFileName);
            var sourceDir = _drive.GetDirectory(sourceFile);

            sourceDir.Files.Remove(sourceFile);
            sourceFile.FullName = destFileName;

            var destDir = _drive.GetDirectory(MemoryPathAccessor.GetDirectoryNameDefault(destFileName));

            if (destFileExists)
            {
                var destFile = _drive.GetFile(destFileName);
                destDir.Files.Remove(destFile);
            }

            destDir.AddFile(sourceFile);
        }

        public string ReadAllText(string path)
        {
            var file = _drive.GetFile(path);
            return file.Contents;
        }

        public string[] ReadAllLines(string path)
        {
            var file = _drive.GetFile(path);
            return file.Contents.Split(Environment.NewLine);
        }

        public void WriteAllText(string path, string contents)
        {
            var file = _drive.GetFile(path);
            file.Contents = contents;
        }

        public void WriteAllLines(string path, string[] contents)
        {
            WriteAllText(path, string.Join(Environment.NewLine, contents));
        }

        public void WriteAllLines(string path, IEnumerable<string> contents)
        {
            WriteAllLines(path, contents.ToArray());
        }
    }
}
