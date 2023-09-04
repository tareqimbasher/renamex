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
    public void Disabled_OldNameIsNotNull_OldNameAndNewNameShouldBeEqual()
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

    [Theory]
    [InlineData("the red fox", "The Red Fox")]
    [InlineData("The red fox", "The Red Fox")]
    [InlineData("the  red       fox", "The  Red       Fox")]
    [InlineData("the red. fox.", "The Red. Fox.")]
    [InlineData("THe RED FoX", "The RED Fox")]
    [InlineData("I saw a little red fox", "I Saw A Little Red Fox")]
    public void DoesRenameCorrectly(string oldName, string expectedNewName)
    {
        var strategy = new TitleCaseStrategy(true);

        var newName = strategy.TransformName(oldName);
        
        Assert.Equal(expectedNewName, newName);
    }
}