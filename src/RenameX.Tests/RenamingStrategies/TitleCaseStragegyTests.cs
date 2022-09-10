using RenameX.RenamingStrategies;
using Xunit;

namespace RenameX.Tests.RenamingStrategies;

public class TitleCaseStragegyTests
{
    [Fact]
    public void Enabled_OldNameIsNotNull_OldNameShouldBeTitleCased()
    {
        string oldName = "calculus 2 - Unit one";
        var strategy = new TitleCaseStrategy(true);

        var newName = strategy.TransformName(oldName);

        Assert.Equal("Calculus 2 - Unit One", newName);
    }

    [Fact]
    public void Enabled_OldNameIsNull_OldNameAndNewNameShouldBeEqual()
    {
        string? oldName = null;
        var strategy = new TitleCaseStrategy(true);

        var newName = strategy.TransformName(oldName);

        Assert.Equal(oldName, newName);
    }

    [Fact]
    public void Enabled_OldNameIsWhiteSpace_OldNameAndNewNameShouldBeEqual()
    {
        string oldName = " ";
        var strategy = new TitleCaseStrategy(true);

        var newName = strategy.TransformName(oldName);

        Assert.Equal(oldName, newName);
    }

    [Fact]
    public void Enabled_OldNameIsEmpty_OldNameAndNewNameShouldBeEqual()
    {
        string oldName = string.Empty;
        var strategy = new TitleCaseStrategy(true);

        var newName = strategy.TransformName(oldName);

        Assert.Equal(oldName, newName);
    }

    [Fact]
    public void Disabled_OldNameIsNotNull_OldNameShouldBeTitleCased()
    {
        string oldName = "calculus 2 - Unit one";
        var strategy = new TitleCaseStrategy(false);

        var newName = strategy.TransformName(oldName);

        Assert.Equal(oldName, newName);
    }

    [Fact]
    public void Enabled_OldNameContainsNonEnglish_OldNameShouldBeTitleCased()
    {
        string oldName = "تجربة Upper lower";
        var strategy = new TitleCaseStrategy(true);

        var newName = strategy.TransformName(oldName);

        Assert.Equal("تجربة Upper Lower", newName);
    }
}