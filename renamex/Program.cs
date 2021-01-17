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
            var dirArg = cli.Argument("directory", "Directory with files to rename.");

            // Options
            cli.HelpOption("-? | -h | --help", inherited: true);
            //var confirmOpt = cli.Option("--confirm", "Confirm before committing changes.", CommandOptionType.NoValue);
            var dryRunOpt = cli.Option("--dry-run", "Dry Run. Print changes, but do not apply them.", CommandOptionType.NoValue);
            var filterOpt = cli.Option("--filter <SEARCHPATTERN>", "Filter the files to operate on.", CommandOptionType.SingleValue);
            var modExtOpt = cli.Option("--include-ext", "Include file extension in renaming. They are excluded by default.", CommandOptionType.NoValue);
            var interAcOpt = cli.Option("-i|--interactive", "Interactive mode. Will open a text file to further modify file names before committing the new names.", CommandOptionType.NoValue);
            var prependOpt = cli.Option("-p|--prepend <TXT>", "Prepend text to each file name.", CommandOptionType.SingleValue);
            var printOpt = cli.Option("--print", "Print results to console.", CommandOptionType.NoValue);
            var replaceOpt = cli.Option("-r|--replace <TXT>", "Replace text in file name. Can be used multiple times.", CommandOptionType.MultipleValue);
            var repWithOpt = cli.Option("-rw|--replace-with <TXT>", "Text to replace with. Must be used when using --replace option.", CommandOptionType.SingleOrNoValue);
            var titCaseOpt = cli.Option("-t|--title-case", "Capitalize the first letter of every word.", CommandOptionType.NoValue);

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
                var dir = new DirectoryInfo(dirArg.Value?.Trim('"') ?? Environment.CurrentDirectory);

                // Verify the working directory exists
                if (!dir.Exists)
                {
                    CConsole.ErrorLine($"Directory \"{dir.FullName}\" does not exist.");
                    return 1;
                }

                // Print working directory
                CConsole.Info("Working directory: ");
                CConsole.WriteLine(dir.FullName);

                var rules = new List<IRenamingRule>();

                if (replaceOpt.HasValue())
                {
                    if (repWithOpt.HasValue() == false)
                    {
                        CConsole.ErrorLine("Missing --replace-with option.");
                        return 1;
                    }
                    rules.Add(new ReplaceTextRule(replaceOpt.Values!, repWithOpt.Value()!));
                }

                if (prependOpt.HasValue())
                {
                    rules.Add(new PrependTextRule(prependOpt.Value()!));
                }

                if (titCaseOpt.HasValue())
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
                if (filterOpt.HasValue())
                    enumeratedFiles = dir.EnumerateFiles(filterOpt.Value()!, new EnumerationOptions() { IgnoreInaccessible = true });
                else
                    enumeratedFiles = dir.EnumerateFiles();

                foreach (var handler in enumeratedFiles.Select(f => new FileHandler(f, modExtOpt.HasValue())))
                {
                    handlers.Add(handler);
                    handler.ApplyOptions(rules);
                    if (handler.ExistingFile.Name.Length > longestFileName)
                        longestFileName = handler.ExistingFile.Name.Length;
                }

                Func<string, string, string> getOldAndNewName = (oldName, newName) => $"{oldName.PadRight(longestFileName)} => {newName}";


                if (interAcOpt.HasValue())
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
                if (dryRunOpt.HasValue())
                {
                    foreach (var handler in handlers)
                    {
                        Console.WriteLine(getOldAndNewName(handler.ExistingFile.Name, handler.NewName));
                    }
                }
                else
                {
                    var history = new DirectoryHistory(dir.FullName).Load();
                    var opLog = new OperationLog(DateTime.UtcNow);

                    foreach (var handler in handlers)
                    {
                        if (handler.ExistingFile.Name == handler.NewName)
                            continue;

                        File.Move(handler.ExistingFile.FullName, dir.PathCombine(handler.NewName), overwrite: false);
                        opLog.Entries.Add(new OperationLogEntry(handler.ExistingFile.Name, handler.NewName));
                        if (printOpt.HasValue())
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
