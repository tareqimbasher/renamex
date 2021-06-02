using RenameX.RenamingStrategies;
using System;
using Xunit;

namespace RenameX.Tests.RenamingStrategies
{
    public class PrependTextStrategyTests
    {
        [Fact]
        public void TextToPrependIsNull_OldNameIsNull_OldNameAndNewNameShouldBeEqual()
        {
            string? oldName = null;
            var strategy = new PrependTextStrategy(null);

            var newName = strategy.TransformName(oldName);

            Assert.Equal(oldName, newName);
        }

        [Fact]
        public void TextToPrependIsNull_OldNameIsNotNull_OldNameAndNewNameShouldBeEqual()
        {
            string oldName = "Old";
            var strategy = new PrependTextStrategy(null);

            var newName = strategy.TransformName(oldName);

            Assert.Equal(oldName, newName);
        }
        
        [Fact]
        public void TextToPrependIsNotNull_OldNameIsNull_OldNameShouldEqualPrependText()
        {
            string prependText = "Prepended Text";
            string? oldName = null;
            var strategy = new PrependTextStrategy(prependText);

            var newName = strategy.TransformName(oldName);

            Assert.Equal(prependText, newName);
        }

        [Fact]
        public void TextToPrependIsNotNull_OldNameIsNotNull_OldNameShouldStartWithPrependedText()
        {
            string prependText = "Prepended Text";
            string oldName = "Old";
            var strategy = new PrependTextStrategy(prependText);

            var newName = strategy.TransformName(oldName);

            Assert.Equal($"{prependText}{oldName}", newName);
        }

        [Fact]
        public void OldNameAlreadyStartsWithPrependText_TextShouldNotBePrepended()
        {
            string prependText = "Prepended Text";
            string oldName = "Prepended Text - Old Name";
            var strategy = new PrependTextStrategy(prependText);

            var newName = strategy.TransformName(oldName);

            Assert.Equal(oldName, newName);
        }

        [Fact]
        public void TextToPrependIsUntrimmed_OldNameShouldStartWithPrependedText()
        {
            string prependText = " Prepended Text ";
            string? oldName = "Old";
            var strategy = new PrependTextStrategy(prependText);

            var newName = strategy.TransformName(oldName);

            Assert.Equal($"{prependText}{oldName}", newName);
        }

        [Fact]
        public void TextToPrependIsUntrimmed_OldNameIsUntrimmed_OldNameShouldStartWithPrependedText()
        {
            string prependText = " Prepended Text ";
            string? oldName = " Old ";
            var strategy = new PrependTextStrategy(prependText);

            var newName = strategy.TransformName(oldName);

            Assert.Equal($"{prependText}{oldName}", newName);
        }

        [Fact]
        public void TextToPrependNonEnglish_OldNameEnglish_OldNameShouldStartWithPrependedText()
        {
            string prependText = "تجربة";
            string? oldName = "Old";
            var strategy = new PrependTextStrategy(prependText);

            var newName = strategy.TransformName(oldName);

            Assert.Equal($"{prependText}{oldName}", newName);
        }

        [Fact]
        public void TextToPrependEnglish_OldNameNonEnglish_OldNameShouldStartWithPrependedText()
        {
            string prependText = "Prepended Text";
            string? oldName = "تجربة";
            var strategy = new PrependTextStrategy(prependText);

            var newName = strategy.TransformName(oldName);

            Assert.Equal($"{prependText}{oldName}", newName);
        }

        [Fact]
        public void TextToPrependNonEnglish_OldNameNonEnglish_OldNameShouldStartWithPrependedText()
        {
            string prependText = "تجربة أولى";
            string? oldName = "تجربة ثانية";
            var strategy = new PrependTextStrategy(prependText);

            var newName = strategy.TransformName(oldName);

            Assert.Equal($"{prependText}{oldName}", newName);
        }
    }
}
