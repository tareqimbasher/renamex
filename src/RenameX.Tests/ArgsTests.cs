using System;
using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using RenameX.Tests.Infrastructure;
using Xunit;

namespace RenameX.Tests;

public class ArgsTests
{
    public static IEnumerable<object?[]> GetArgsParsingData()
    {
        return new[]
        {
            new object?[] { "--filter", false, null },
            new object?[] { "--filter *.jpg", true, new Func<Settings, bool>[] { settings => settings.Filter == "*.jpg" } },
            new object?[] { "--filter:*.jpg", true, new Func<Settings, bool>[] { settings => settings.Filter == "*.jpg" } },
            new object?[] { "-f *.jpg", true, new Func<Settings, bool>[] { settings => settings.Filter == "*.jpg" } },
            new object?[] { "-f:*.jpg", true, new Func<Settings, bool>[] { settings => settings.Filter == "*.jpg" } },

            new object?[] { "--prepend", false, null },
            new object?[] { "--prepend Test", true, new Func<Settings, bool>[] { settings => settings.PrependText == "Test" } },
            new object?[] { "--prepend:Test", true, new Func<Settings, bool>[] { settings => settings.PrependText == "Test" } },
            new object?[] { "-p Test", true, new Func<Settings, bool>[] { settings => settings.PrependText == "Test" } },
            new object?[] { "-p:Test", true, new Func<Settings, bool>[] { settings => settings.PrependText == "Test" } },

            new object?[] { "--replace", false, null },
            new object?[] { "--replace Test", true, new Func<Settings, bool>[] { settings => settings.ReplaceTexts.Single() == "Test" } },
            new object?[] { "--replace:Test", true, new Func<Settings, bool>[] { settings => settings.ReplaceTexts.Single() == "Test" } },
            new object?[] { "-r Test", true, new Func<Settings, bool>[] { settings => settings.ReplaceTexts.Single() == "Test" } },
            new object?[] { "-r:Test", true, new Func<Settings, bool>[] { settings => settings.ReplaceTexts.Single() == "Test" } },

            new object?[] { "--replace-with Test", true, new Func<Settings, bool>[] { settings => settings.ReplaceWithText == "Test" } },
            new object?[] { "--replace-with:Test", true, new Func<Settings, bool>[] { settings => settings.ReplaceWithText == "Test" } },
            new object?[] { "-rw Test", true, new Func<Settings, bool>[] { settings => settings.ReplaceWithText == "Test" } },
            new object?[] { "-rw:Test", true, new Func<Settings, bool>[] { settings => settings.ReplaceWithText == "Test" } },

            new object?[] { "--title-case", true, new Func<Settings, bool>[] { settings => settings.TitleCase } },
            new object?[] { "-t", true, new Func<Settings, bool>[] { settings => settings.TitleCase } },
                
            new object?[] { "--interactive", true, new Func<Settings, bool>[] { settings => settings.InteractiveMode } },
            new object?[] { "-i", true, new Func<Settings, bool>[] { settings => settings.InteractiveMode } },
                
            new object?[] { "--verbose", true, new Func<Settings, bool>[] { settings => settings.Verbose } },
            new object?[] { "-v", true, new Func<Settings, bool>[] { settings => settings.Verbose } },
                
            new object?[] { "--include-ext", true, new Func<Settings, bool>[] { settings => settings.ModifyExtensions } },
                
            new object?[] { "--no-log", true, new Func<Settings, bool>[] { settings => settings.DisableHistoryLog } },
                
            new object?[] { "--dry", true, new Func<Settings, bool>[] { settings => settings.DryRun } },
            new object?[] { "--dry-run", true, new Func<Settings, bool>[] { settings => settings.DryRun } },

            new object?[] { "--filter:*.jpg --prepend:PreTxt --replace:RepTxt1 --replace:RepTxt2 --replace-with:RwTxt -t", true, new Func<Settings, bool>[] 
                { 
                    settings => settings.Filter == "*.jpg",
                    settings => settings.PrependText == "PreTxt",
                    settings => settings.ReplaceTexts[0] == "RepTxt1" && settings.ReplaceTexts[1] == "RepTxt2",
                    settings => settings.ReplaceWithText == "RwTxt",
                    settings => settings.TitleCase,
                }
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetArgsParsingData))]
    public void ArgsParsing(string argsStr, bool expectedSuccess, IEnumerable<Func<Settings, bool>> evaluations)
    {
        var args = new[] { "test" }.Union(argsStr.Split(' ')).ToArray();

        (CommandLineApplication cli, CommandLineApplication cmd) = GetCliApp();

        var app = new App(new MemoryFileSystem());
        app.Bootstrap(cli, args);

        cmd.OnExecute(() =>
        {
            var settings = app.Settings();

            foreach (var eval in evaluations)
            {
                Assert.True(eval(settings));
            }
        });

        var exception = Record.Exception(() => cli.Execute(args));
        if (expectedSuccess)
            Assert.Null(exception);
        else
            Assert.NotNull(exception);
    }

    private (CommandLineApplication cli, CommandLineApplication cmd) GetCliApp()
    {
        var cli = new CommandLineApplication
        {
            Name = "renamex-tests",
            Description = "Testing cli app.",
            UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect,
            MakeSuggestionsInErrorMessage = true
        };

        var testCmd = cli.Command("test", cmd =>
        {
            cmd.AddFilterOption()
                .AddPrependOption()
                .AddReplaceOption()
                .AddReplaceWithOption()
                .AddTitleCaseOption()
                .AddInteractiveOption()
                .AddVerboseOption()
                .AddModifyExtensionsOption()
                .AddDisableLoggingOption()
                .AddDryRunOption();
        });

        return (cli, testCmd);
    }
}