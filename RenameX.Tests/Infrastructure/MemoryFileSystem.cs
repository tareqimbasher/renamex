using RenameX.FileSystem;

namespace RenameX.Tests.Infrastructure
{
    public class MemoryFileSystem : IFileSystem
    {
        public MemoryFileSystem()
        {
            Drive = new MemoryDrive(this);

            Directory = new MemoryDirectoryAccessor(Drive);
            File = new MemoryFileAccessor(Drive);
            Path = new MemoryPathAccessor(Drive);
        }

        public MemoryDrive Drive { get; }

        public IDirectory Directory { get; }

        public IFile File { get; }

        public IPath Path { get; }
    }
}
