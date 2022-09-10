using System.Collections.Generic;
using RenameX.RenamingStrategies;
using RenameX.Tests.Infrastructure;
using Xunit;

namespace RenameX.Tests;

public class FileHandlerTests
{
    [Theory]
    [InlineData("text file.txt", "text file 2.txt")]
    [InlineData("text file.txt", "text file 2.docx")]
    public void DirectlyUpdateNewName(string oldName, string newName)
    {
        var fs = GetFileSystem();
        var handler = new FileHandler(fs, $"/home/documents/{oldName}", modifyExtension: false);

        handler.DirectlyUpdateNewName(newName);

        Assert.Equal(oldName, handler.OldName);
        Assert.Equal(newName, handler.NewName);
    }

    [Theory]
    [InlineData("Old.txt", true, "New.txt")]
    [InlineData("Old.txt", false, "New.txt")]
    [InlineData("Old", true, "New")]
    [InlineData("Old", false, "New")]
    public void NewNameDiffersFromOld(string oldName, bool modifyExt, string newName)
    {
        var fs = GetFileSystem();
        var handler = new FileHandler(fs, $"/home/documents/{oldName}", modifyExtension: modifyExt);

        handler.DirectlyUpdateNewName(newName);

        Assert.True(handler.NewNameDiffersFromOld);
    }


    public static IEnumerable<object?[]> GetApplyStrategiesData()
    {
        return new[]
        {
            new object[] { "Old.txt", new[] { new ReplaceTextStrategy(new[] { "Old" }, "New") }, false, "New.txt" },
            new object[] { "Old.txt", new[] { new ReplaceTextStrategy(new[] { "Old" }, "New") }, true, "New.txt" },
            new object[] { "Txt.txt", new[] { new ReplaceTextStrategy(new[] { "xt" }, "iff") }, false, "Tiff.txt" },
            new object[] { "Txt.txt", new[] { new ReplaceTextStrategy(new[] { "xt" }, "iff") }, true, "Tiff.tiff" },
        };
    }

    [Theory]
    [MemberData(nameof(GetApplyStrategiesData))]
    public void ApplyStrategies(string oldName, IRenamingStrategy[] strategies, bool modifyExt, string expectedNewName)
    {
        var fs = GetFileSystem();
        var handler = new FileHandler(fs, $"/home/documents/{oldName}", modifyExtension: modifyExt);

        handler.Apply(strategies);

        Assert.Equal(expectedNewName, handler.NewName);
    }


    public static IEnumerable<object?[]> GetCommitData()
    {
        return new[]
        {
            new object[] { "photo 1.jpg", new[] { new ReplaceTextStrategy(new[] { "Photo" }, "Pic") }, "photo 1.jpg", FileCommitResult.NameUnchanged },
            new object[] { "photo 1.jpg", new[] { new ReplaceTextStrategy(new[] { "photo" }, "Pic") }, "Pic 1.jpg", FileCommitResult.Success },
            new object[] { "photo 1.jpg", new[] { new ReplaceTextStrategy(new[] { " " }, "-") }, "photo-1.jpg", FileCommitResult.Success },
            new object[] { "photo 1.jpg", new[] { new ReplaceTextStrategy(new[] { "1" }, "2") }, "photo 2.jpg", FileCommitResult.FileAlreadyExists },
            new object[] { "photo 1.jpg", new[] { new ReplaceTextStrategy(new[] { "1" }, "/") }, "photo /.jpg", FileCommitResult.Error },
        };
    }

    [Theory]
    [MemberData(nameof(GetCommitData))]
    public void Commit(string oldName, IRenamingStrategy[] strategies, string expectedNewName, FileCommitResult expectedResult)
    {
        var fs = GetFileSystem();
        var handler = new FileHandler(fs, $"/home/pictures/Fishing Trip/{oldName}", modifyExtension: false);
        handler.Apply(strategies);
        Assert.Equal(expectedNewName, handler.NewName);

        FileCommitResult result = handler.Commit(out var error);

        Assert.Equal(expectedResult, result);
        if (expectedResult == FileCommitResult.Error)
            Assert.NotNull(error);
        else
            Assert.Null(error);
    }


    private MemoryFileSystem GetFileSystem()
    {
        var fs = new MemoryFileSystem();
        MemoryDriveData.FillWithDefaultData(fs.Drive);
        return fs;
    }
}