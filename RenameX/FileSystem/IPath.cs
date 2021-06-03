namespace RenameX.FileSystem
{
    public interface IPath
    {
        string? GetDirectoryName(string? path);
        string GetFileName(string path);
        string GetFileNameWithoutExtension(string path);
        string GetTempFileName();
    }
}
