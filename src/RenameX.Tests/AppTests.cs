using McMaster.Extensions.CommandLineUtils;
using RenameX.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace RenameX.Tests
{
    public class AppTests
    {
        private static IEnumerable<object?[]> GetArgsParsingData()
        {
            return new object?[][]
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

                new object?[] { "--title-case", true, new Func<Settings, bool>[] { settings => settings.TitleCase == true } },
                new object?[] { "-t", true, new Func<Settings, bool>[] { settings => settings.TitleCase == true } },
                new object?[] { "--interactive", true, new Func<Settings, bool>[] { settings => settings.InteractiveMode == true } },
                new object?[] { "-i", true, new Func<Settings, bool>[] { settings => settings.InteractiveMode == true } },
                new object?[] { "--verbose", true, new Func<Settings, bool>[] { settings => settings.Verbose == true } },
                new object?[] { "-v", true, new Func<Settings, bool>[] { settings => settings.Verbose == true } },
                new object?[] { "--include-ext", true, new Func<Settings, bool>[] { settings => settings.ModifyExtensions == true } },
                new object?[] { "--no-log", true, new Func<Settings, bool>[] { settings => settings.DisableHistoryLog == true } },
                new object?[] { "--dry", true, new Func<Settings, bool>[] { settings => settings.DryRun == true } },
                new object?[] { "--dry-run", true, new Func<Settings, bool>[] { settings => settings.DryRun == true } },

                new object?[] { "--filter:*.jpg --prepend:PreTxt --replace:RepTxt1 --replace:RepTxt2 --replace-with:RwTxt", true, new Func<Settings, bool>[] 
                    { 
                        settings => settings.Filter == "*.jpg",
                        settings => settings.PrependText == "PreTxt",
                        settings => settings.ReplaceTexts[0] == "RepTxt1" && settings.ReplaceTexts[1] == "RepTxt2",
                        settings => settings.ReplaceWithText == "RwTxt",
                    }
                },
            };
        }

        [Theory]
        [MemberData(nameof(GetArgsParsingData))]
        public void ArgsParsing(string argsStr, bool expectedSuccess, IEnumerable<Func<Settings, bool>> evals)
        {
            var args = new[] { "test" }.Union(argsStr.Split(' ')).ToArray();

            (CommandLineApplication cli, CommandLineApplication cmd) = GetCliApp();

            var app = new App(new MemoryFileSystem());
            app.Bootstrap(cli, args);

            cmd.OnExecute(() =>
            {
                var settings = app.Settings();

                foreach (var eval in evals)
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
}
