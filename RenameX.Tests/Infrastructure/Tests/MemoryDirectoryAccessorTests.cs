using Xunit;

namespace RenameX.Tests.Infrastructure.Tests
{
    public class MemoryDirectoryAccessorTests
    {
        [Fact]
        public void DirectoryExists()
        {
            var drive = GetDrive();
            var dir = new MemoryDirectoryAccessor(drive);

            Assert.True(dir.Exists("/home/work"));
            Assert.Equal(drive.DirectoryExists("/home/family photos"), dir.Exists("/home/family photos"));
        }

        [Fact]
        public void GetDirectories()
        {
            var drive = GetDrive();
            var dir = new MemoryDirectoryAccessor(drive);

            var directories = dir.GetDirectories("/home");
            Assert.Equal(directories, new[] { "/home/work", "/home/family photos" });
        }

        [Fact]
        public void GetFiles()
        {
            var drive = GetDrive();
            var dir = new MemoryDirectoryAccessor(drive);

            var directories = dir.GetFiles("/home/work");
            Assert.Equal(directories, new[] { "/home/work/presentation.ppt", "/home/work/spreadsheet.xlsx" });
        }

        [Theory]
        [InlineData("text*", new[] { "/home/text file.txt" })]
        [InlineData("*.txt", new[] { "/home/text file.txt" })]
        [InlineData("*.jpg", new[] { "/home/pic 1.jpg", "/home/pic 2.jpg" })]
        [InlineData("*x*", new[] { "/home/text file.txt", "/home/excel.xlsx" })]
        [InlineData("text file.txt", new[] { "/home/text file.txt" })]
        [InlineData("text", new string[] { })]
        [InlineData("text file", new string[] { })]
        [InlineData(".jpg", new string[] { })]
        public void GetFilesWithSearchPattern(string searchPattern, string[] expectedMatches)
        {
            var drive = GetDrive();
            var dir = new MemoryDirectoryAccessor(drive);

            var files = dir.GetFiles("/home", searchPattern);
            Assert.Equal(expectedMatches, files);
        }

        [Fact]
        public void EnumerateFiles()
        {
            var drive = GetDrive();
            var dir = new MemoryDirectoryAccessor(drive);

            var directories = dir.EnumerateFiles("/home/work");
            Assert.Equal(directories, new[] { "/home/work/presentation.ppt", "/home/work/spreadsheet.xlsx" });
        }

        [Theory]
        [InlineData("text*", new[] { "/home/text file.txt" })]
        [InlineData("*.txt", new[] { "/home/text file.txt" })]
        [InlineData("*.jpg", new[] { "/home/pic 1.jpg", "/home/pic 2.jpg" })]
        [InlineData("*x*", new[] { "/home/text file.txt", "/home/excel.xlsx" })]
        [InlineData("text file.txt", new[] { "/home/text file.txt" })]
        [InlineData("text", new string[] { })]
        [InlineData("text file", new string[] { })]
        [InlineData(".jpg", new string[] { })]
        public void EnumerateFilesWithSearchPattern(string searchPattern, string[] expectedMatches)
        {
            var drive = GetDrive();
            var dir = new MemoryDirectoryAccessor(drive);

            var files = dir.EnumerateFiles("/home", searchPattern);
            Assert.Equal(expectedMatches, files);
        }

        private MemoryDrive GetDrive()
        {
            return new MemoryDrive(new MemoryFileSystem())
                .AddDirectory(new MemoryDirectory("/tmp"))
                .AddDirectory(new MemoryDirectory("/home")
                    .AddFile(new MemoryFile("/home/text file.txt"))
                    .AddFile(new MemoryFile("/home/excel.xlsx"))
                    .AddFile(new MemoryFile("/home/powerpoint.ppt"))
                    .AddFile(new MemoryFile("/home/pic 1.jpg"))
                    .AddFile(new MemoryFile("/home/pic 2.jpg"))
                )
                .AddDirectory(new MemoryDirectory("/home/work")
                    .AddFile(new MemoryFile("/home/work/presentation.ppt"))
                    .AddFile(new MemoryFile("/home/work/spreadsheet.xlsx"))
                )
                .AddDirectory(new MemoryDirectory("/home/family photos"))
                .AddDirectory(new MemoryDirectory("/homework"))
            ;
        }
    }
}
