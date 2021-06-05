using System;
using System.Linq;
using Xunit;

namespace RenameX.Tests.Infrastructure.Tests
{
    public class MemoryFileAccessorTests
    {
        [Fact]
        public void FileExists()
        {
            var drive = GetDrive();
            var file = new MemoryFileAccessor(drive);

            Assert.True(file.Exists("/home/work/presentation.ppt"));
            Assert.Equal(drive.FileExists("/home/work/presentation.ppt"), file.Exists("/home/work/presentation.ppt"));
        }

        [Fact]
        public void Delete()
        {
            var drive = GetDrive();
            var file = new MemoryFileAccessor(drive);
            string fileToDelete = "/home/powerpoint.ppt";

            Assert.True(file.Exists(fileToDelete));
            var dir = drive.GetDirectory("/home");
            Assert.Single(dir.Files.Where(f => f.FullName == fileToDelete));

            file.Delete(fileToDelete);

            Assert.False(file.Exists("/home/powerpoint.ppt"));
            Assert.Empty(dir.Files.Where(f => f.FullName == fileToDelete));
        }

        [Fact]
        public void MoveToDifferentDirectory_NoOverwite()
        {
            var drive = GetDrive();
            var file = new MemoryFileAccessor(drive);

            var sourceFilePath = "/home/excel.xlsx";
            var destFilePath = "/home/work/excel.xlsx";
            var sourceFileDir = drive.GetDirectory(new MemoryFile(sourceFilePath));
            var destFileDir = drive.GetDirectory(new MemoryFile(destFilePath));

            Assert.True(file.Exists(sourceFilePath));
            Assert.Single(sourceFileDir.Files.Where(f => f.FullName == sourceFilePath));
            Assert.Empty(sourceFileDir.Files.Where(f => f.FullName == destFilePath));
            Assert.Empty(destFileDir.Files.Where(f => f.FullName == destFilePath));

            file.Move(sourceFilePath, destFilePath);

            Assert.False(file.Exists(sourceFilePath));
            Assert.True(file.Exists(destFilePath));
            Assert.Empty(sourceFileDir.Files.Where(f => f.FullName == sourceFilePath));
            Assert.Empty(sourceFileDir.Files.Where(f => f.FullName == destFilePath));
            Assert.Single(destFileDir.Files.Where(f => f.FullName == destFilePath));
        }

        [Theory]
        [InlineData("/home/excel.xlsx", "/home/excel2.xlsx")]
        [InlineData("/home/excel.xlsx", "/home/excel 2.xlsx")]
        [InlineData("/home/work/presentation.ppt", "/home/work/the best slideshow.ppt")]
        public void MoveToDifferentNameInSameDirectory_NoOverwite(string sourceFilePath, string destFilePath)
        {
            var drive = GetDrive();
            var file = new MemoryFileAccessor(drive);

            var dir = drive.GetDirectory(new MemoryFile(sourceFilePath));

            Assert.True(file.Exists(sourceFilePath));
            Assert.False(file.Exists(destFilePath));
            Assert.Single(dir.Files.Where(f => f.FullName == sourceFilePath));
            Assert.Empty(dir.Files.Where(f => f.FullName == destFilePath));

            file.Move(sourceFilePath, destFilePath);

            Assert.False(file.Exists(sourceFilePath));
            Assert.True(file.Exists(destFilePath));
            Assert.Empty(dir.Files.Where(f => f.FullName == sourceFilePath));
            Assert.Single(dir.Files.Where(f => f.FullName == destFilePath));
        }

        [Theory]
        [InlineData("/home/pic 1.jpg", "/home/work/pic 1.jpg", true)]
        [InlineData("/home/pic 1.jpg", "/home/work/pic 1.jpg", false)]
        public void MoveToDifferentDirectory_WithOverwite(string sourceFilePath, string destFilePath, bool overwrite)
        {
            var drive = GetDrive();
            var file = new MemoryFileAccessor(drive);

            var sourceFile = drive.GetFile(sourceFilePath);
            var sourceFileDir = drive.GetDirectory(sourceFile);

            var destFile = drive.GetFile(destFilePath);
            var destFileDir = drive.GetDirectory(destFile);

            void checkInitialState()
            {
                Assert.True(file.Exists(sourceFilePath));
                Assert.Single(sourceFileDir.Files.Where(f => f.FullName == sourceFilePath));
                Assert.Contains(sourceFile, sourceFileDir.Files);
                Assert.DoesNotContain(destFile, sourceFileDir.Files);
                Assert.Empty(sourceFileDir.Files.Where(f => f.FullName == destFilePath));

                Assert.True(file.Exists(destFilePath));
                Assert.Single(destFileDir.Files.Where(f => f.FullName == destFilePath));
                Assert.Contains(destFile, destFileDir.Files);
                Assert.DoesNotContain(sourceFile, destFileDir.Files);
                Assert.Empty(destFileDir.Files.Where(f => f.FullName == sourceFilePath));
            }

            if (!overwrite)
            {
                Assert.Throws<InvalidOperationException>(() => file.Move(sourceFilePath, destFilePath, overwrite: overwrite));

                // Initial state should not have changed
                checkInitialState();
                return;
            }

            file.Move(sourceFilePath, destFilePath, overwrite: overwrite);

            Assert.False(file.Exists(sourceFilePath));
            Assert.Empty(sourceFileDir.Files.Where(f => f.FullName == sourceFilePath));
            Assert.DoesNotContain(sourceFile, sourceFileDir.Files);
            Assert.DoesNotContain(destFile, sourceFileDir.Files);
            Assert.Empty(sourceFileDir.Files.Where(f => f.FullName == destFilePath));

            Assert.True(file.Exists(destFilePath));
            Assert.Single(destFileDir.Files.Where(f => f.FullName == destFilePath));
            Assert.DoesNotContain(destFile, destFileDir.Files);
            Assert.Contains(sourceFile, destFileDir.Files);
            Assert.Empty(destFileDir.Files.Where(f => f.FullName == sourceFilePath));
        }

        [Theory]
        [InlineData("/home/pic 1.jpg", "/home/pic 2.jpg", true)]
        [InlineData("/home/pic 1.jpg", "/home/pic 2.jpg", false)]
        public void MoveToDifferentNameInSameDirectory_WithOverwite(string sourceFilePath, string destFilePath, bool overwrite)
        {
            var drive = GetDrive();
            var file = new MemoryFileAccessor(drive);

            var sourceFile = drive.GetFile(sourceFilePath);
            var destFile = drive.GetFile(destFilePath);
            var dir = drive.GetDirectory(sourceFile);

            void checkInitialState()
            {
                Assert.True(file.Exists(sourceFilePath));
                Assert.True(file.Exists(destFilePath));
                Assert.Single(dir.Files.Where(f => f.FullName == sourceFilePath));
                Assert.Single(dir.Files.Where(f => f.FullName == destFilePath));
                Assert.Contains(sourceFile, dir.Files);
                Assert.Contains(destFile, dir.Files);
            }

            if (!overwrite)
            {
                Assert.Throws<InvalidOperationException>(() => file.Move(sourceFilePath, destFilePath, overwrite: overwrite));

                // Initial state should not have changed
                checkInitialState();
                return;
            }

            file.Move(sourceFilePath, destFilePath, overwrite: overwrite);

            Assert.False(file.Exists(sourceFilePath));
            Assert.True(file.Exists(destFilePath));
            Assert.Empty(dir.Files.Where(f => f.FullName == sourceFilePath));
            Assert.Single(dir.Files.Where(f => f.FullName == destFilePath));
            Assert.Contains(sourceFile, dir.Files);
            Assert.DoesNotContain(destFile, dir.Files);
        }

        [Fact]
        public void ReadAllText()
        {
            var file = new MemoryFileAccessor(GetDrive());

            Assert.Equal($"first line{Environment.NewLine}second line", file.ReadAllText("/home/text file.txt"));
        }

        [Fact]
        public void ReadAllLines()
        {
            var file = new MemoryFileAccessor(GetDrive());

            Assert.Equal(new[] { "first line", "second line" }, file.ReadAllLines("/home/text file.txt"));
        }

        [Fact]
        public void WriteAllText()
        {
            var drive = GetDrive();
            var file = new MemoryFileAccessor(drive);
            var filePath = "/home/text file.txt";
            var text = $"How do you do?{Environment.NewLine}My second line";

            file.WriteAllText(filePath, text);

            Assert.Equal(text, drive.GetFile(filePath).Contents);
        }

        [Fact]
        public void WriteAllLines_ArrayParam()
        {
            var drive = GetDrive();
            var file = new MemoryFileAccessor(drive);
            var filePath = "/home/text file.txt";
            var lines = new[] { "How do you do?", "My second line" };

            file.WriteAllLines(filePath, lines);

            Assert.Equal(lines, drive.GetFile(filePath).Contents.Split(Environment.NewLine));
        }

        [Fact]
        public void WriteAllLines_IEnumerableParam()
        {
            var drive = GetDrive();
            var file = new MemoryFileAccessor(drive);
            var filePath = "/home/text file.txt";
            var lines = new[] { "How do you do?", "My second line" }.AsEnumerable();

            file.WriteAllLines(filePath, lines);

            Assert.Equal(lines, drive.GetFile(filePath).Contents.Split(Environment.NewLine));
        }


        private MemoryDrive GetDrive()
        {
            return new MemoryDrive(new MemoryFileSystem())
                .AddDirectory(new MemoryDirectory("/tmp"))
                .AddDirectory(new MemoryDirectory("/home")
                    .AddFile(new MemoryFile("/home/text file.txt", $"first line{Environment.NewLine}second line"))
                    .AddFile(new MemoryFile("/home/excel.xlsx"))
                    .AddFile(new MemoryFile("/home/powerpoint.ppt"))
                    .AddFile(new MemoryFile("/home/pic 1.jpg"))
                    .AddFile(new MemoryFile("/home/pic 2.jpg"))
                )
                .AddDirectory(new MemoryDirectory("/home/work")
                    .AddFile(new MemoryFile("/home/work/presentation.ppt"))
                    .AddFile(new MemoryFile("/home/work/spreadsheet.xlsx"))
                    .AddFile(new MemoryFile("/home/work/pic 1.jpg"))
                )
                .AddDirectory(new MemoryDirectory("/home/family photos"))
                .AddDirectory(new MemoryDirectory("/homework"))
            ;
        }
    }
}
