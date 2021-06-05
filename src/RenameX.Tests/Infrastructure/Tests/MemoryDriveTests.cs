using System.IO;
using System.Linq;
using Xunit;

namespace RenameX.Tests.Infrastructure.Tests
{
    public class MemoryDriveTests
    {
        [Fact]
        public void AddDirectory()
        {
            var dir = new MemoryDirectory("/tmp");

            var drive = GetEmptyDrive().AddDirectory(dir);

            Assert.Single(drive.Directories);
            Assert.Equal(dir, drive.Directories.Single());
        }

        [Fact]
        public void DirectoryExists()
        {
            var directory0 = new MemoryDirectory("/tmp");
            var directory1 = new MemoryDirectory("/tmp/Dir1");
            var directory2 = new MemoryDirectory("/tmp/Dir 2");

            var drive = GetEmptyDrive()
                .AddDirectory(directory0)
                .AddDirectory(directory1)
                .AddDirectory(directory2);

            Assert.True(drive.DirectoryExists("/tmp"));
            Assert.True(drive.DirectoryExists("/tmp/Dir1"));
            Assert.True(drive.DirectoryExists("/tmp/Dir 2"));
            Assert.False(drive.DirectoryExists("/doesnotexist"));
            Assert.False(drive.DirectoryExists("/tmp/folderdoesnotexist"));
        }

        [Fact]
        public void EnsureDirectoryExists()
        {
            var drive = GetEmptyDrive()
                .AddDirectory(new MemoryDirectory("/tmp"));

            var exception = Record.Exception(() => drive.EnsureDirectoryExists("/tmp"));

            Assert.Null(exception);
            Assert.Throws<DirectoryNotFoundException>(() => drive.EnsureDirectoryExists("doesnotexist"));
            Assert.Throws<DirectoryNotFoundException>(() => drive.EnsureDirectoryExists("/doesnotexist"));
            Assert.Throws<DirectoryNotFoundException>(() => drive.EnsureDirectoryExists("/tmp/folderdoesnotexist"));
            Assert.Throws<DirectoryNotFoundException>(() => drive.EnsureDirectoryExists("/tmp/folderdoes notexist"));
        }

        [Fact]
        public void GetDirectoryByDirectoryPath()
        {
            var directory1 = new MemoryDirectory("/tmp/Dir1");
            var directory2 = new MemoryDirectory("/tmp/Dir 2");

            var drive = GetEmptyDrive()
                .AddDirectory(directory1)
                .AddDirectory(directory2);

            Assert.Equal(directory1, drive.GetDirectory("/tmp/Dir1"));
            Assert.Equal(directory2, drive.GetDirectory("/tmp/Dir 2"));
            Assert.Equal("/tmp/Dir1", drive.GetDirectory("/tmp/Dir1").FullName);
            Assert.Equal("/tmp/Dir 2", drive.GetDirectory("/tmp/Dir 2").FullName);
            Assert.Throws<DirectoryNotFoundException>(() => drive.GetDirectory("/tmp/doesnotexist"));
        }

        [Fact]
        public void GetDirectoryByFile()
        {
            var file = new MemoryFile("/tmp/test.txt");
            var directory = new MemoryDirectory("/tmp").AddFile(file);

            var drive = GetEmptyDrive().AddDirectory(directory);

            Assert.Equal(directory, drive.GetDirectory(file));
            Assert.Equal(directory.FullName, drive.GetDirectory(file).FullName);

            Assert.Throws<DirectoryNotFoundException>(() => drive.GetDirectory(new MemoryFile("/doesnotexist/test.txt")));
        }

        [Fact]
        public void FileExists()
        {
            var drive = GetDriveWithDefaultData();

            Assert.True(drive.FileExists("/home/documents/text file.txt"));
            Assert.True(drive.FileExists("/home/pictures/Fishing Trip/photo 1.jpg"));
            Assert.False(drive.FileExists("/tmp/filedoesnotexist"));
            Assert.False(drive.FileExists("/folderdoesnotexist/filedoesnotexist"));
        }

        [Fact]
        public void EnsureFileExists()
        {
            var drive = GetDriveWithDefaultData();

            var exception = Record.Exception(() => drive.EnsureFileExists("/home/documents/text file.txt"));

            Assert.Null(exception);
            Assert.Throws<FileNotFoundException>(() => drive.EnsureFileExists("/tmp/filedoesnotexist"));
            Assert.Throws<FileNotFoundException>(() => drive.EnsureFileExists("/folderdoesnotexist/filedoesnotexist"));
        }

        [Fact]
        public void GetFile()
        {
            var file = new MemoryFile("/tmp/test.txt");

            var drive = GetEmptyDrive()
                .AddDirectory(new MemoryDirectory("/tmp").AddFile(file));

            Assert.Equal(file, drive.GetFile("/tmp/test.txt"));
            Assert.Throws<FileNotFoundException>(() => drive.GetFile("/doesnotexist/test.txt"));
            Assert.Throws<FileNotFoundException>(() => drive.GetFile("/tmp/doesnotexist.txt"));
        }





        private MemoryDrive GetEmptyDrive()
        {
            return new MemoryDrive(new MemoryFileSystem());
        }

        private MemoryDrive GetDriveWithDefaultData()
        {
            return MemoryDriveData.FillWithDefaultData(GetEmptyDrive());
        }
    }
}
