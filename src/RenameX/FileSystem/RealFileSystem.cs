namespace RenameX.FileSystem;

internal class RealFileSystem : IFileSystem
{
    public RealFileSystem()
    {
        Directory = new RealDirectory();
        File = new RealFile();
        Path = new RealPath();
    }

    public IDirectory Directory { get; }

    public IFile File { get; }

    public IPath Path { get; }
}