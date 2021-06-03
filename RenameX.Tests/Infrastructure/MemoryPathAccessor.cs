using RenameX.FileSystem;
using System;
using System.IO;
using System.Linq;

namespace RenameX.Tests.Infrastructure
{
    public class MemoryPathAccessor : IPath
    {
        private readonly MemoryDrive _drive;
        private static readonly Random _random = new Random();
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public MemoryPathAccessor(MemoryDrive drive)
        {
            _drive = drive;
        }

        public static string? GetDirectoryNameDefault(string? path) => Path.GetDirectoryName(path)?.Replace('\\', '/');

        public string? GetDirectoryName(string? path) => GetDirectoryNameDefault(path)?.Replace('\\', '/');


        public static string GetFileNameDefault(string path) => Path.GetFileName(path);

        public string GetFileName(string path) => GetFileNameDefault(path);


        public static string GetFileNameWithoutExtensionDefault(string path) => Path.GetFileNameWithoutExtension(path);

        public string GetFileNameWithoutExtension(string path) => GetFileNameWithoutExtensionDefault(path);


        public string GetTempFileName()
        {
            var name = new string(Enumerable.Repeat(_chars, 6).Select(s => s[_random.Next(s.Length)]).ToArray());
            var randomName = $"/tmp/{name}";

            if (_drive.FileExists(randomName))
                throw new Exception($"Random filename '{randomName}' already exists.");

            _drive.GetDirectory("/tmp").AddFile(new MemoryFile(randomName));
            return randomName;
        }
    }
}
