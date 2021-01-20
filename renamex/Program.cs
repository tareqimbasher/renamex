using McMaster.Extensions.CommandLineUtils;
using RenameX.History;
using RenameX.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RenameX
{
    class Program
    {
        static int Main(string[] args)
        {
            // Ensure app data folder is created
            if (!Consts.AppDataDirectory.Exists)
                Consts.AppDataDirectory.Create();

            // Configure CLI app
            var cli = new CommandLineApplication
            {
                Name = "renamex",
                Description = "File renaming utility.",
                UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect,
                MakeSuggestionsInErrorMessage = true
            };

            // Arguments
            var dirArg = cli.Argument("directory", "Directory with files to rename. Uses current working directory by default.");

            // Options
            cli.HelpOption("-? | -h | --help", inherited: true);
            //var confirmOpt = cli.Option("--confirm", "Confirm before committing changes.", CommandOptionType.NoValue);
            var interactiveOption = cli.Option("-i|--interactive",
                "Interactive mode. Before committing file name changes, " +
                "further modify the calculated new file names in a text editor.",
                CommandOptionType.NoValue);
            var prependOption = cli.Option("-p|--prepend <TXT>",
                "Prepend text to each file name. If specified text already exists " +
                "at the start of a file name, it will not be additionally prepended.", CommandOptionType.SingleValue);
            var replaceOption = cli.Option("-r|--replace <TXT>",
                "Replace the specified text in file name. " +
                "Can be used multiple times to specify multiple text values to replace.", CommandOptionType.MultipleValue);
            var replaceWithOption = cli.Option("-rw|--replace-with <TXT>",
                "Text to replace with. Required when using --replace option.",
                CommandOptionType.SingleOrNoValue);
            var titleCaseOption = cli.Option("-t|--title-case",
                "Capitalize the first letter of every word.",
                CommandOptionType.NoValue);
            var dryRunOption = cli.Option("--dry",
                "Dry Run. Will not make any changes.",
                CommandOptionType.NoValue);
            var filterOption = cli.Option("--filter <SEARCHPATTERN>",
                "Filter the files to rename. Selects all files in specified directory by default.",
                CommandOptionType.SingleValue);
            var modifyExtensionsOption = cli.Option(
                "--include-ext",
                "Include file extension in renaming. They are excluded by default.", CommandOptionType.NoValue);
            var verboseOption = cli.Option("-v|--verbose",
                "Be verbose.",
                CommandOptionType.NoValue);

            var undo = cli.Command("undo", cmd =>
            {
                cmd.Description = "Undo the last rename operation.";
            });

            var history = cli.Command("history", cmd =>
            {
                cmd.Description = "Print history of rename operations in the specified directory.";
                cmd.Arguments.Add(dirArg);

                cmd.OnExecute(() =>
                {
                    var workingDir = GetWorkingDirectory(dirArg);

                    CConsole.Info("Working directory: ");
                    CConsole.WriteLine(workingDir.FullName);

                    var history = new DirectoryHistory(workingDir).Load();

                    foreach (var log in history.Logs)
                    {
                        CConsole.InfoLine($"Date (UTC): {log.DateUtc}");
                        var longestName = log.Entries.Max(x => x.OldName.Length);
                        int count = 0;
                        int numPadding = log.Entries.Count.ToString().Length;
                        foreach (var entry in log.Entries)
                        {
                            CConsole.SuccessLine($"  {(++count).ToString().PadLeft(numPadding)}. {entry.OldName.PadRight(longestName)} => {entry.NewName}");
                        }
                    }
                });
            });

            cli.OnExecute(() =>
            {
                var settings = new RenameSettings(
                    GetWorkingDirectory(dirArg),
                    filterOption.HasValue() ? filterOption.Value() : null,
                    replaceOption.HasValue() ? replaceOption.Values : null,
                    replaceWithOption.HasValue() ? replaceWithOption.Value() : null,
                    prependOption.HasValue() ? prependOption.Value() : null,
                    titleCaseOption.HasValue(),
                    interactiveOption.HasValue(),
                    modifyExtensionsOption.HasValue(),
                    verboseOption.HasValue(),
                    dryRunOption.HasValue() || args.Contains("--dry")
                );

                // Verify settings
                if (!settings.Validate(out var errors))
                {
                    foreach (var error in errors)
                    {
                        CConsole.ErrorLine(error);
                    }
                    return 1;
                }

                var operation = new RenameOperation(settings);
                return operation.Run() ? 0 : 1;
            });

            try
            {
                return cli.Execute(args);
            }
            catch (Exception ex)
            {
                CConsole.Error(ex.Message);
                return 1;
            }
        }

        private static DirectoryInfo GetWorkingDirectory(CommandArgument dirArg)
               // If working directory is not specified, use the current directory
            => new DirectoryInfo(dirArg.Value?.Trim('"') ?? Environment.CurrentDirectory);
    }
}
