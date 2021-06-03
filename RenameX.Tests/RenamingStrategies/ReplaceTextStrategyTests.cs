using RenameX.RenamingStrategies;
using System;
using System.Collections.Generic;
using Xunit;

namespace RenameX.Tests.RenamingStrategies
{
    public class ReplaceTextStrategyTests
    {
        [Fact]
        public void NoTextsToReplace_OldNameAndNewNameShouldBeEqual()
        {
            string? oldName = "Old Name";
            var textsToReplace = new List<string>();

            var strategy = new ReplaceTextStrategy(textsToReplace, "ReplaceWith");

            var newName = strategy.TransformName(oldName);

            Assert.Equal(oldName, newName);
        }

        [Fact]
        public void TextsToReplaceIsNull_Throws()
        {
            List<string>? textsToReplace = null;

#pragma warning disable CS8604 // Possible null reference argument.
            Assert.Throws<ArgumentNullException>(() => new ReplaceTextStrategy(textsToReplace, "ReplaceWith"));
#pragma warning restore CS8604
        }

        [Fact]
        public void OldNameIsNull_OldNameAndNewNameShouldBeEqual()
        {
            string? oldName = null;
            var textsToReplace = new List<string>() { "Test" };
            var replaceWith = "Replaced Text";

            var strategy = new ReplaceTextStrategy(textsToReplace, replaceWith);

            var newName = strategy.TransformName(oldName);

            Assert.Equal(oldName, newName);
        }

        [Fact]
        public void OldNameIsEmptyOrWhiteSpace_OldNameAndNewNameShouldBeEqual()
        {
            string? oldName = " ";
            var textsToReplace = new List<string>() { "Test" };
            var replaceWith = "Replaced Text";

            var strategy = new ReplaceTextStrategy(textsToReplace, replaceWith);

            var newName = strategy.TransformName(oldName);

            Assert.Equal(oldName, newName);
        }

        [Theory]
        [InlineData("Old Name", new string?[] { null }, "New", false)]
        [InlineData("Old Name", new[] { "" }, "New", false)]
        [InlineData("Old Name", new[] { "Old", "" }, "New", true)]
        [InlineData("Old Name", new[] { "", "Old" }, "New", true)]
        [InlineData("Old Name", new[] { "Old" }, "New", true)]
        public void TextToReplaceIsEmptyString_OldNameAndNewNameShouldBeEqual(string oldName, string?[] textsToReplace, string replaceWith, bool oldNameShouldChange)
        {
            var strategy = new ReplaceTextStrategy(textsToReplace, replaceWith);

            var newName = strategy.TransformName(oldName);

            Assert.Equal(oldNameShouldChange, oldName != newName);
        }

        [Theory]
        [InlineData("Old Name", new[] { "Old", "", null }, "New", "New Name")]
        [InlineData("Old Name", new[] { "O", "", null }, "New", "Newld Name")]
        [InlineData("Old Name", new[] { "o" }, "New", "Old Name")]
        [InlineData("Old Name", new[] { "a" }, "e", "Old Neme")]
        [InlineData("Old Name", new[] { "a" }, "a", "Old Name")]
        [InlineData("Old Name", new[] { "na" }, "a", "Old Name")]
        [InlineData("Old Name", new[] { "n", "a" }, "a", "Old Name")]
        [InlineData("Old Name", new[] { " " }, "", "OldName")]
        [InlineData("Old Name", new[] { " " }, null, "OldName")]
        [InlineData("Old Name", new[] { " " }, "-", "Old-Name")]
        [InlineData("Old Name", new[] { "a", "a" }, "a", "Old Name")]
        [InlineData("Old Name", new[] { "Name", "name" }, "name", "Old name")]
        [InlineData("Older Name", new[] { "e" }, "a", "Oldar Nama")]
        public void ReplaceText(string oldName, string?[] textsToReplace, string replaceWith, string expectedNewName)
        {
            var strategy = new ReplaceTextStrategy(textsToReplace, replaceWith);

            var newName = strategy.TransformName(oldName);

            Assert.Equal(expectedNewName, newName);
        }
    }
}
