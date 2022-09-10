using System.IO;

namespace RenameX.Tests.Infrastructure;

public class MemoryFile
{
    public MemoryFile(string fullName)
    {
        FullName = fullName;
        Contents = string.Empty;
    }

    public MemoryFile(string fullName, string contents) : this(fullName)
    {
        Contents = contents;
    }

    public string FullName { get; set; }
    public string Name => Path.GetFileName(FullName);
    public string Contents { get; set; }
}