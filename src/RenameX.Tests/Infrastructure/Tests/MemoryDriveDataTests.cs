using Xunit;

namespace RenameX.Tests.Infrastructure.Tests;

public class MemoryDriveDataTests
{
    [Fact]
    public void EnsureTmpDirectoryExists()
    {
        var drive = new MemoryDrive(new MemoryFileSystem());

        Assert.False(drive.DirectoryExists("/tmp"));

        MemoryDriveData.FillWithDefaultData(drive);

        Assert.True(drive.DirectoryExists("/tmp"));
    }
}