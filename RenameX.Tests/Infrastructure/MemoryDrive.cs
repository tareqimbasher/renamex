using RenameX.FileSystem;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RenameX.Tests.Infrastructure
{
    public class MemoryDrive
    {
        private readonly IFileSystem _fileSystem;

        public MemoryDrive(IFileSystem fileSystem)
        {
            Directories = new List<MemoryDirectory>();
            _fileSystem = fileSystem;
        }

        public List<MemoryDirectory> Directories { get; }

        public MemoryDrive AddDirectory(MemoryDirectory directory)
        {
            Directories.Add(directory);
            return this;
        }


        public bool DirectoryExists(string? path)
        {
            return Directories.Any(d => d.FullName == path);
        }

        public void EnsureDirectoryExists(string? path)
        {
            if (!DirectoryExists(path))
                throw new DirectoryNotFoundException($"Could not find a part of the path '{path}'.");
        }

        public MemoryDirectory GetDirectory(string? path)
        {
            EnsureDirectoryExists(path);
            return Directories.Single(d => d.FullName == path);
        }

        public MemoryDirectory GetDirectory(MemoryFile file)
        {
            var dirPath = _fileSystem.Path.GetDirectoryName(file.FullName);
            return GetDirectory(dirPath);
        }


        public bool FileExists(string? path)
        {
            var dirPath = _fileSystem.Path.GetDirectoryName(path);
            if (!DirectoryExists(dirPath))
                return false;

            return GetDirectory(dirPath).Files.Any(f => f.FullName == path);
        }

        public void EnsureFileExists(string? path)
        {
            if (!FileExists(path))
                throw new FileNotFoundException($"Could not find a part of the path '{path}'.");
        }

        public MemoryFile GetFile(string? path)
        {
            EnsureFileExists(path);
            var dirPath = _fileSystem.Path.GetDirectoryName(path);
            return GetDirectory(dirPath).Files.Single(f => f.FullName == path);
        }
    }
}
