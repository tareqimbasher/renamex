using McMaster.Extensions.CommandLineUtils;
using RenameX.FileSystem;
using RenameX.History;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RenameX
{
    public class App
    {
        private readonly IFileSystem _fileSystem;

        public App(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public int Run(string[] args)
        {
            var cli = new CommandLineApplication
            {
                Name = "renamex",
                Description = "Bulk file renaming utility.",
                UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect,
                MakeSuggestionsInErrorMessage = true
            };

            Bootstrap(cli, args);

            var settings = Settings();

            if (!settings.Directory.Exists)
            {
                CConsole.ErrorLine($"Directory '{settings.Directory.FullName}' does not exist.");
                return 1;
            }

            try
            {
                return cli.Execute(args);
            }
            catch (Exception ex)
            {
                CConsole.ErrorLine(ex.Message);
                return 1;
            }
        }

        public Func<Settings> Settings { get; private set; }

        public void Bootstrap(CommandLineApplication cli, string[] args)
        {
            // Arguments
            var dirArg = cli.Argument("directory", "Directory with files to rename. If not specified, uses current working directory.");

            // Options
            cli.HelpOption("-? | -h | --help", inherited: true);

            cli.AddFilterOption()
                .AddPrependOption()
                .AddReplaceOption()
                .AddReplaceWithOption()
                .AddTitleCaseOption()
                .AddInteractiveOption()
                .AddVerboseOption()
                .AddModifyExtensionsOption()
                .AddDisableLoggingOption()
                .AddDryRunOption();

            Settings = () =>
            {
                return new Settings(
                    GetWorkingDirectory(dirArg),
                    CliArgs.FilterOption.HasValue() ? CliArgs.FilterOption.Value() : null,
                    CliArgs.ReplaceOption.HasValue() ? CliArgs.ReplaceOption.Values : null,
                    CliArgs.ReplaceWithOption.HasValue() ? CliArgs.ReplaceWithOption.Value() : null,
                    CliArgs.PrependOption.HasValue() ? CliArgs.PrependOption.Value() : null,
                    CliArgs.TitleCaseOption.HasValue(),
                    CliArgs.InteractiveOption.HasValue(),
                    CliArgs.ModifyExtensionsOption.HasValue(),
                    CliArgs.VerboseOption.HasValue(),
                    CliArgs.DryRunOption.HasValue() || args.Contains("--dry") || args.Contains("--dry-run"),
                    CliArgs.DisableLoggingOption.HasValue()
                );
            };

            // Commands
            var history = cli.Command("history", cmd =>
            {
                cmd.Description = "Print history of rename operations in the specified directory.";
                cmd.Arguments.Add(dirArg);

                cmd.OnExecute(() => ExecuteHistory(Settings()));
            });

            var undo = cli.Command("undo", cmd =>
            {
                cmd.Description = "Undo the last rename operation.";
                cmd.Arguments.Add(dirArg);
                cmd.AddInteractiveOption()
                 .AddVerboseOption()
                 .AddDisableLoggingOption()
                 .AddDryRunOption();

                cmd.OnExecute(() => ExecuteUndo(Settings()));
            });

            cli.OnExecute(() => ExecuteRename(Settings()));
        }

        private int ExecuteRename(Settings settings)
        {
            // Verify settings
            if (!settings.Validate(out var errors))
            {
                foreach (var error in errors)
                {
                    CConsole.ErrorLine(error);
                }
                return 1;
            }

            var operation = new RenameOperation(_fileSystem, settings);
            return operation.Run() ? 0 : 1;
        }

        private int ExecuteHistory(Settings settings)
        {
            CConsole.Info("Working directory: ");
            CConsole.WriteLine(settings.Directory.FullName);

            var history = new DirectoryHistory(settings.Directory, _fileSystem).Load();

            if (!history.Logs.Any())
                CConsole.SuccessLine("No history!");

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

            return 0;
        }

        private int ExecuteUndo(Settings settings)
        {
            CConsole.Info("Working directory: ");
            CConsole.WriteLine(settings.Directory.FullName);

            var workingDir = settings.Directory;
            var history = new DirectoryHistory(workingDir, _fileSystem).Load();

            if (!history.Logs.Any())
            {
                CConsole.SuccessLine("No history!");
                return 1;
            }

            var lastRenameOp = history.Logs.OrderBy(x => x.DateUtc).Last();
            foreach (var entry in lastRenameOp.Entries)
            {
                if (_fileSystem.File.Exists(_fileSystem.Path.Combine(workingDir.FullName, entry.NewName)))
                {
                    _fileSystem.File.Move(
                        _fileSystem.Path.Combine(workingDir.FullName, entry.NewName),
                        _fileSystem.Path.Combine(workingDir.FullName, entry.OldName));
                }
            }
            return 0;
        }

        private DirectoryInfo GetWorkingDirectory(CommandArgument dirArg)
            // If working directory is not specified, use the current directory
            => new DirectoryInfo(dirArg.Value?.Trim('"') ?? Environment.CurrentDirectory);
    }
}
