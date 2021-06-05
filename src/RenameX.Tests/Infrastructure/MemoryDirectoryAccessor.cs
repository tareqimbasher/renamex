using RenameX.FileSystem;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RenameX.Tests.Infrastructure
{
    public class MemoryDirectoryAccessor : IDirectory
    {
        private readonly MemoryDrive _drive;

        public MemoryDirectoryAccessor(MemoryDrive drive)
        {
            _drive = drive;
        }

        public bool Exists(string path)
        {
            return _drive.DirectoryExists(path);
        }

        public string[] GetDirectories(string path)
        {
            _drive.EnsureDirectoryExists(path);
            return _drive.Directories
                .Where(d => d.FullName.StartsWith(path.TrimEnd('/') + "/"))
                .Select(d => d.FullName).ToArray();
        }

        public string[] GetFiles(string path)
        {
            return _drive.GetDirectory(path).Files.Select(f => f.FullName).ToArray();
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            return _drive.GetDirectory(path).Files.Select(f => f.FullName)
                .Where(f => FileNameMatchesSearchPattern(MemoryPathAccessor.GetFileNameDefault(f), searchPattern))
                .ToArray();
        }

        public IEnumerable<string> EnumerateFiles(string path)
        {
            foreach (var file in _drive.GetDirectory(path).Files.Select(f => f.FullName))
            {
                yield return file;
            }
        }

        public IEnumerable<string> EnumerateFiles(string path, string searchPattern)
        {
            foreach (var fullName in _drive.GetDirectory(path).Files.Select(f => f.FullName))
            {
                if (FileNameMatchesSearchPattern(MemoryPathAccessor.GetFileNameDefault(fullName), searchPattern))
                    yield return fullName;
            }
        }

        private bool FileNameMatchesSearchPattern(string fileName, string searchPattern)
        {
            if (searchPattern.Contains("*"))
            {
                searchPattern = Regex.Escape(searchPattern).Replace("\\*", ".*?");
                var regex = new Regex(searchPattern);

                return regex.IsMatch(fileName);
            }
            else
            {
                return fileName == searchPattern;
            }
        }
    }
}
