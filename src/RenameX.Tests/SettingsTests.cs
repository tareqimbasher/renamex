using RenameX.RenamingStrategies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace RenameX.Tests;

public class SettingsTests
{
    [Fact]
    public void DataPassedToSettingsShouldBeUnchanged_NullData()
    {
        string workingDir = "/tmp";

        var settings = new Settings(
            workingDirectory: new DirectoryInfo(workingDir),
            filter: null,
            replaceTexts: null,
            replaceWithText: null,
            prependText: null,
            titleCase: true,
            interactiveMode: true,
            modifyExtensions: true,
            verbose: true,
            dryRun: true,
            disableHistoryLog: true
        );

        Assert.Equal(new DirectoryInfo(workingDir).FullName, settings.Directory.FullName);
        Assert.Null(settings.Filter);
        Assert.NotNull(settings.ReplaceTexts);
        Assert.Empty(settings.ReplaceTexts);
        Assert.Null(settings.ReplaceWithText);
        Assert.Null(settings.PrependText);
        Assert.True(settings.TitleCase);
        Assert.True(settings.InteractiveMode);
        Assert.True(settings.ModifyExtensions);
        Assert.True(settings.Verbose);
        Assert.True(settings.DryRun);
        Assert.True(settings.DisableHistoryLog);
    }

    [Fact]
    public void DataPassedToSettingsShouldBeUnchanged_NotNullData()
    {
        string filter = "*.jpg";
        var replaceTexts = new List<string?>() { "Text 1", "Text 2", null };
        string replaceWith = "New Text";
        string prependText = "Prepend Text";

        var settings = new Settings(
            workingDirectory: new DirectoryInfo("/tmp"),
            filter: filter,
            replaceTexts: replaceTexts,
            replaceWithText: replaceWith,
            prependText: prependText,
            titleCase: true,
            interactiveMode: true,
            modifyExtensions: true,
            verbose: true,
            dryRun: true,
            disableHistoryLog: true
        );

        Assert.Equal(filter, settings.Filter);
        Assert.Equal(replaceTexts, settings.ReplaceTexts);
        Assert.Equal(replaceWith, settings.ReplaceWithText);
        Assert.Equal(prependText, settings.PrependText);
    }


    public static IEnumerable<object?[]> GetWorkingDirectoryData()
    {
        return new[]
        {
            new object?[] { "/tmp", true, null },
            new object?[] { "/tmp/does-not-exist", false, $"Directory \"{new DirectoryInfo("/tmp/does-not-exist").FullName}\" does not exist." }
        };
    }

    [Theory]
    [MemberData(nameof(GetWorkingDirectoryData))]
    public void SettingsValidityTests_WorkingDirectory(string workingDir, bool settingsExpectedValid, string? expectedErrorMessage)
    {
        var settings = new Settings(
            workingDirectory: new DirectoryInfo(workingDir),
            filter: null,
            replaceTexts: null,
            replaceWithText: null,
            prependText: null,
            titleCase: false,
            interactiveMode: false,
            modifyExtensions: false,
            verbose: true,
            dryRun: true,
            disableHistoryLog: true
        );

        Assert.Equal(settingsExpectedValid, settings.Validate(out var errors));

        if (!settingsExpectedValid)
        {
            Assert.Equal(expectedErrorMessage, errors.Single());
        }
    }


    public static IEnumerable<object?[]> GetReplaceTextData()
    {
        return new[]
        {
            new object?[] { null, null, true, null },
            new object?[] { new[] { "replace" }, "replace with", true, null },
            new object?[] { new[] { "replace" }, null, false, "Missing --replace-with option." },
            new object?[] { null, "replace with", false, "Missing --replace option." },
            new object?[] { new[] { "replace" }, "replace / with", false, "Invalid file name characters passed to --replace-with option." },
            new object?[] { new[] { "replace" }, "replace , with", true, null },
            new object?[] { new[] { "replace" }, "replace . with", true, null },
            new object?[] { new[] { "replace/text" }, "replace with", true, null },
        };
    }

    [Theory]
    [MemberData(nameof(GetReplaceTextData))]
    public void SettingsValidityTests_ReplaceText(
        IEnumerable<string?>? replaceTexts,
        string replaceWith,
        bool settingsExpectedValid,
        string? expectedErrorMessage)
    {
        var settings = new Settings(
            workingDirectory: new DirectoryInfo("/tmp"),
            filter: null,
            replaceTexts: replaceTexts,
            replaceWithText: replaceWith,
            prependText: null,
            titleCase: false,
            interactiveMode: false,
            modifyExtensions: false,
            verbose: true,
            dryRun: true,
            disableHistoryLog: true
        );

        Assert.Equal(settingsExpectedValid, settings.Validate(out var errors));

        if (!settingsExpectedValid)
        {
            Assert.Equal(expectedErrorMessage, errors.Single());
        }
    }


    public static IEnumerable<object?[]> GetPrependTextData()
    {
        return new[]
        {
            new object?[] { null, true, null },
            new object?[] { "prepend text", true, null },
            new object?[] { "prepend / text", true, "Invalid file name characters passed to --prepend option." },
        };
    }

    [Theory]
    [MemberData(nameof(GetPrependTextData))]
    public void SettingsValidityTests_PrependText(string prependText, bool settingsExpectedValid, string? expectedErrorMessage)
    {
        var settings = new Settings(
            workingDirectory: new DirectoryInfo("/tmp"),
            filter: null,
            replaceTexts: null,
            replaceWithText: null,
            prependText: prependText,
            titleCase: false,
            interactiveMode: false,
            modifyExtensions: false,
            verbose: true,
            dryRun: true,
            disableHistoryLog: true
        );

        Assert.Equal(settingsExpectedValid, settings.Validate(out var errors));

        if (!settingsExpectedValid)
        {
            Assert.Equal(expectedErrorMessage, errors.Single());
        }
    }



    public static IEnumerable<object?[]> GetGenerateRenamingStrategiesData()
    {
        return new[]
        {
            new object?[] { null, null, false, Type.EmptyTypes },
            new object?[] { new[] { "replace" }, null, false, new[] { typeof(ReplaceTextStrategy) } },
            new object?[] { null, "prepend text", false, new[] { typeof(PrependTextStrategy) } },
            new object?[] { null, "", false, Type.EmptyTypes },
            new object?[] { null, null, true, new[] { typeof(TitleCaseStrategy) } },
            new object?[] { new[] { "replace" }, "prepend text", true, new[] { typeof(ReplaceTextStrategy), typeof(PrependTextStrategy), typeof(TitleCaseStrategy) } },
        };
    }

    [Theory]
    [MemberData(nameof(GetGenerateRenamingStrategiesData))]
    public void GenerateRenamingStrategies(
        IEnumerable<string?>? replaceTexts,
        string prependText,
        bool titleCase,
        Type[] expectedStrategies)
    {
        var settings = new Settings(
            workingDirectory: new DirectoryInfo("/tmp"),
            filter: null,
            replaceTexts: replaceTexts,
            replaceWithText: replaceTexts?.Any() == true ? string.Empty : null,
            prependText: prependText,
            titleCase: titleCase,
            interactiveMode: false,
            modifyExtensions: false,
            verbose: true,
            dryRun: true,
            disableHistoryLog: true
        );

        Assert.True(settings.Validate(out _));

        var strategies = settings.GenerateRenamingStrategies().Select(s => s.GetType()).ToArray();

        Assert.Equal(expectedStrategies.Length, strategies.Length);
        Assert.Equal(expectedStrategies, strategies);

        for (int i = 0; i < expectedStrategies.Length; i++)
        {
            Assert.Equal(expectedStrategies[i], strategies[i]);
        }
    }
}