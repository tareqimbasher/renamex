using Xunit;

namespace RenameX.Tests.Infrastructure.Tests;

public class MemoryFileTests
{
    [Fact]
    public void PathTest()
    {
        Assert.Equal("/tmp/test.txt", new MemoryFile("/tmp/test.txt").FullName);
        Assert.Equal("test.txt", new MemoryFile("/tmp/test.txt").Name);

        Assert.Equal("/home/pictures/Fishing Trip Photo.jpg", new MemoryFile("/home/pictures/Fishing Trip Photo.jpg").FullName);
        Assert.Equal("Fishing Trip Photo.jpg", new MemoryFile("/home/pictures/Fishing Trip Photo.jpg").Name);
    }
}