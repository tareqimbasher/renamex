using System;
using System.Linq;
using Xunit;

namespace RenameX.Tests.Infrastructure.Tests
{
    public class MemoryDirectoryTests
    {
        [Fact]
        public void PathTest()
        {
            Assert.Equal("/tmp", new MemoryDirectory("/tmp").FullName);
            Assert.Equal("/home/documents", new MemoryDirectory("/home/documents").FullName);
            Assert.Equal("/home/pictures/Fishing Trip", new MemoryDirectory("/home/pictures/Fishing Trip").FullName);
        }

        [Fact]
        public void FilesTest()
        {
            var directory = new MemoryDirectory("/tmp");
            Assert.NotNull(directory.Files);

            var file = new MemoryFile("/tmp/test.txt");
            directory.AddFile(file);

            Assert.Equal(file, directory.Files.Single());
            Assert.Equal(file.FullName, directory.Files.Single().FullName);
        }

        [Fact]
        public void InvalidFilesTest()
        {
            var directory = new MemoryDirectory("/tmp");
            var file1 = new MemoryFile("/tmp2/test.txt");
            var file2 = new MemoryFile("/something else/test.txt");

            Assert.Throws<Exception>(() => directory.AddFile(file1));
            Assert.Throws<Exception>(() => directory.AddFile(file2));
        }
    }
}
