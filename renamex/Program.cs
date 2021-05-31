using McMaster.Extensions.CommandLineUtils;
using RenameX.History;
using System;
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
                Description = "Bulk file renaming utility.",
                UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect,
                MakeSuggestionsInErrorMessage = true
            };

            // Arguments
            var dirArg = cli.Argument("directory", "Directory with files to rename. If not specified, uses current working directory.");

            // Options
            cli.HelpOption("-? | -h | --help", inherited: true);

            var filterOption = cli.Option("-f|--filter <SEARCHPATTERN>",
                "Filter the files to rename. Selects all files in specified directory by default.",
                CommandOptionType.SingleValue);

            var prependOption = cli.Option("-p|--prepend <TXT>",
                "Prepend text to each file name. If specified text already exists " +
                "at the start of a file name, it will not be additionally prepended.",
                CommandOptionType.SingleValue);

            var replaceOption = cli.Option("-r|--replace <TXT>",
                "Replace the specified text in file name. " +
                "Can be used multiple times to specify multiple text values to replace.",
                CommandOptionType.MultipleValue);

            var replaceWithOption = cli.Option("-rw|--replace-with <TXT>",
                "Text to replace with. Required when using --replace option.",
                CommandOptionType.SingleOrNoValue);

            var titleCaseOption = cli.Option("-t|--title-case",
                "Capitalize the first letter of every word.",
                CommandOptionType.NoValue);

            var interactiveOption = cli.Option("-i|--interactive",
                "Allows the command to stop and wait for user input or action (for example to confirm renaming)." +
                "Also allows user to further modify the calculated new file names in a text editor.",
                CommandOptionType.NoValue);

            var verboseOption = cli.Option("-v|--verbose",
                "Prints file name changes.",
                CommandOptionType.NoValue);

            var modifyExtensionsOption = cli.Option(
                "--include-ext",
                "Include file extension in renaming. They are excluded by default.",
                CommandOptionType.NoValue);

            var disableLoggingOption = cli.Option(
                "--no-log",
                "Disables logging rename in history log.",
                CommandOptionType.NoValue);

            var dryRunOption = cli.Option("--dry",
                "Dry Run. Will not make any changes.",
                CommandOptionType.NoValue);

            // Commands
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

                    if (!history.Logs.Any())
                        CConsole.Success("No history!");

                    foreach (var log in history.Logs)
                    {
                        CConsole.InfoLine($"Date: {log.DateUtc.ToLocalTime()}");
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

            var undo = cli.Command("undo", cmd =>
            {
                cmd.Description = "Undo the last rename operation.";
                cmd.Arguments.Add(dirArg);
                cmd.Options.Add(interactiveOption);
                cmd.Options.Add(verboseOption);
                cmd.Options.Add(disableLoggingOption);
                cmd.Options.Add(dryRunOption);

                cmd.OnExecute(() =>
                {
                    var workingDir = GetWorkingDirectory(dirArg);

                    CConsole.Info("Working directory: ");
                    CConsole.WriteLine(workingDir.FullName);

                    var history = new DirectoryHistory(workingDir).Load();

                    if (!history.Logs.Any())
                    {
                        CConsole.Success("No history!");
                        return 1;
                    }

                    var lastRenameOp = history.Logs.OrderBy(x => x.DateUtc).Last();
                    foreach (var entry in lastRenameOp.Entries)
                    {
                        if (File.Exists(workingDir.PathCombine(entry.NewName)))
                            File.Move(workingDir.PathCombine(entry.NewName), workingDir.PathCombine(entry.OldName));
                    }
                    return 0;
                });
            });

            cli.OnExecute(() =>
            {
                var settings = new Settings(
                    GetWorkingDirectory(dirArg),
                    filterOption.HasValue() ? filterOption.Value() : null,
                    replaceOption.HasValue() ? replaceOption.Values : null,
                    replaceWithOption.HasValue() ? replaceWithOption.Value() : null,
                    prependOption.HasValue() ? prependOption.Value() : null,
                    titleCaseOption.HasValue(),
                    interactiveOption.HasValue(),
                    modifyExtensionsOption.HasValue(),
                    verboseOption.HasValue(),
                    dryRunOption.HasValue() || args.Contains("--dry"),
                    disableLoggingOption.HasValue()
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
