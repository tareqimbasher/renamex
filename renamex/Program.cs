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
            var verboseOption = cli.Option("--verbose",
                "Be verbose.",
                CommandOptionType.NoValue);

            var undo = cli.Command("undo", cmd =>
            {
                cmd.Description = "Undo the last rename operation.";
            });

            var history = cli.Command("history", cmd =>
            {
                cmd.Description = "Print history of rename operations in the specified directory.";
            });

            cli.OnExecute(() =>
            {
                // If working directory is not specified, use the current directory
                var workingDir = new DirectoryInfo(dirArg.Value?.Trim('"') ?? Environment.CurrentDirectory);

                var settings = new RenameSettings(
                    workingDir.FullName,
                    filterOption.HasValue() ? filterOption.Value() : null,
                    replaceOption.HasValue() ? replaceOption.Values : null,
                    replaceWithOption.HasValue() ? replaceWithOption.Value() : null,
                    prependOption.HasValue() ? prependOption.Value() : null,
                    titleCaseOption.HasValue(),
                    interactiveOption.HasValue(),
                    modifyExtensionsOption.HasValue(),
                    verboseOption.HasValue(),
                    dryRunOption.HasValue()
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

                // Print working directory
                CConsole.Info("Working directory: ");
                CConsole.WriteLine(workingDir.FullName);

                var rules = new List<IRenamingRule>();

                if (settings.ReplaceTexts?.Any() == true)
                {
                    if (settings.ReplaceWithText == null)
                    {
                        CConsole.ErrorLine("Missing --replace-with option.");
                        return 1;
                    }
                    rules.Add(new ReplaceTextRule(replaceOption.Values!, replaceWithOption.Value()!));
                }

                if (prependOption.HasValue())
                {
                    rules.Add(new PrependTextRule(prependOption.Value()!));
                }

                if (titleCaseOption.HasValue())
                {
                    rules.Add(new TitleCaseRule(true));
                }

                // At least one renaming rule must have been specified
                if (!rules.Any())
                {
                    CConsole.ErrorLine("No options specified.");
                    return 1;
                }


                var handlers = new List<FileHandler>();
                int longestFileName = 0;

                IEnumerable<FileInfo> enumeratedFiles;
                if (filterOption.HasValue())
                    enumeratedFiles = workingDir.EnumerateFiles(filterOption.Value()!, new EnumerationOptions() { IgnoreInaccessible = true });
                else
                    enumeratedFiles = workingDir.EnumerateFiles();

                foreach (var handler in enumeratedFiles.Select(f => new FileHandler(f, modifyExtensionsOption.HasValue())))
                {
                    handlers.Add(handler);
                    handler.ApplyOptions(rules);
                    if (handler.ExistingFile.Name.Length > longestFileName)
                        longestFileName = handler.ExistingFile.Name.Length;
                }

                Func<string, string, string> getOldAndNewName = (oldName, newName) => $"{oldName.PadRight(longestFileName)} => {newName}";


                if (interactiveOption.HasValue())
                {
                    var tmpFile = Path.GetTempFileName();
                    File.Move(tmpFile, tmpFile + ".txt");
                    tmpFile += ".txt";

                    Console.WriteLine(tmpFile);

                    File.WriteAllLines(
                        tmpFile,
                        handlers.Select(h => getOldAndNewName(h.ExistingFile.Name, h.NewName)).ToArray());

                    Process.Start(new ProcessStartInfo(tmpFile) { UseShellExecute = true })?.WaitForExit();

                    var editedNames = File.ReadAllLines(tmpFile).Select(l => l.Split("=>").Select(x => x.Trim()).ToArray()).ToArray();
                    File.Delete(tmpFile);

                    handlers = handlers.Where(h =>
                    {
                        var editedName = editedNames.FirstOrDefault(en => en[0] == h.ExistingFile.Name);
                        if (editedName == null)
                            return false;

                        h.NewName = editedName[1];
                        return true;
                    })
                    .ToList();
                }


                // If dry run
                if (dryRunOption.HasValue())
                {
                    foreach (var handler in handlers)
                    {
                        if (verboseOption.HasValue())
                            Console.WriteLine(getOldAndNewName(handler.ExistingFile.Name, handler.NewName));
                    }
                }
                else
                {
                    var history = new DirectoryHistory(workingDir.FullName).Load();
                    var opLog = new OperationLog(DateTime.UtcNow);

                    foreach (var handler in handlers)
                    {
                        if (handler.ExistingFile.Name == handler.NewName)
                            continue;

                        File.Move(handler.ExistingFile.FullName, workingDir.PathCombine(handler.NewName), overwrite: false);
                        opLog.Entries.Add(new OperationLogEntry(handler.ExistingFile.Name, handler.NewName));
                        if (verboseOption.HasValue())
                            Console.WriteLine(getOldAndNewName(handler.ExistingFile.Name, handler.NewName));
                    }

                    if (!opLog.Entries.Any())
                    {
                        CConsole.Info("No files were renamed!");
                    }
                    else
                    {
                        history.Logs.Add(opLog);
                        history.Save();
                    }

                    return 0;
                }


                return 0;
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
    }
}
