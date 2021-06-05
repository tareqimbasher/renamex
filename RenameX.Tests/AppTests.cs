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
        private static IEnumerable<object[]> GetArgsParsingData()
        {
            return new object[][]
            {
                new object[] { "--filter *.jpg", new Func<Settings, bool>[] { settings => settings.Filter == "*.jpg" } },
                new object[] { "--prepend Test", new Func<Settings, bool>[] { settings => settings.PrependText == "Test" } },
                //new object[] { "-t", new Func<Settings, bool>[] { settings => settings.TitleCase } },
            };
        }

        [Theory]
        [MemberData(nameof(GetArgsParsingData))]
        public void ArgsParsing(string argsStr, IEnumerable<Func<Settings, bool>> evals)
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

            cli.Execute(args);
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
