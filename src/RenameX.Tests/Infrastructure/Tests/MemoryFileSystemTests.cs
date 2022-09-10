using Xunit;

namespace RenameX.Tests.Infrastructure.Tests;

public class MemoryFileSystemTests
{
    [Fact]
    public void Constructors()
    {
        var fs = new MemoryFileSystem();

        Assert.NotNull(fs.Drive);
        Assert.True(fs.Directory is MemoryDirectoryAccessor);
        Assert.True(fs.File is MemoryFileAccessor);
        Assert.True(fs.Path is MemoryPathAccessor);
    }
}