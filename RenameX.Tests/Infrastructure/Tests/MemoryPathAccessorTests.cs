using System.IO;
using Xunit;

namespace RenameX.Tests.Infrastructure.Tests
{
    public class MemoryPathAccessorTests
    {
        [Fact]
        public void GetDirectoryName()
        {
            var path = GetMemoryPathAccessor();

            Assert.Equal("/tmp", path.GetDirectoryName("/tmp/test"));
            Assert.Equal("/tmp", path.GetDirectoryName("/tmp/test dir"));
            Assert.Equal("/tmp/test dir", path.GetDirectoryName("/tmp/test dir/sub dir"));
            Assert.Equal("/", path.GetDirectoryName("/tmp"));
            Assert.Null(path.GetDirectoryName("/"));
        }

        [Fact]
        public void GetFileName()
        {
            var path = GetMemoryPathAccessor();

            Assert.Equal("test.txt", path.GetFileName("test.txt"));
            Assert.Equal("test.txt", path.GetFileName("/tmp/test.txt"));
            Assert.Equal("test.txt", path.GetFileName("/tmp/sub dir/test.txt"));
            Assert.Equal("test", path.GetFileName("/tmp/sub dir/test"));
        }

        [Fact]
        public void GetFileNameWithoutExtension()
        {
            var path = GetMemoryPathAccessor();

            Assert.Equal("test", path.GetFileNameWithoutExtension("test.txt"));
            Assert.Equal("test", path.GetFileNameWithoutExtension("/tmp/test.txt"));
            Assert.Equal("test", path.GetFileNameWithoutExtension("/tmp/sub dir/test.txt"));
            Assert.Equal("test", path.GetFileNameWithoutExtension("/tmp/sub dir/test"));
        }

        [Fact]
        public void GetTempFileName()
        {
            var drive = new MemoryDrive(new MemoryFileSystem());
            var path = new MemoryPathAccessor(drive);

            string? tmpFilePath = null;

            Assert.Throws<DirectoryNotFoundException>(() => tmpFilePath = path.GetTempFileName());

            drive.AddDirectory(new MemoryDirectory("/tmp"));

            var exception = Record.Exception(() => tmpFilePath = path.GetTempFileName());

            Assert.Null(exception);
            Assert.StartsWith("/tmp/", tmpFilePath);
            Assert.True(drive.FileExists(tmpFilePath));
            Assert.Equal("/tmp", drive.GetDirectory(drive.GetFile(tmpFilePath)).FullName);
            Assert.Equal(6, drive.GetFile(tmpFilePath).Name.Length);
        }


        private MemoryPathAccessor GetMemoryPathAccessor()
        {
            var drive = new MemoryDrive(new MemoryFileSystem())
                .AddDirectory(new MemoryDirectory("/tmp"));

            return new MemoryPathAccessor(drive);
        }
    }
}
