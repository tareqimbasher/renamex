using McMaster.Extensions.CommandLineUtils;
using RenameX.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RenameX
{
    class Program
    {
        static int Main(string[] args)
        {
            // include only certain files
            // undo capability
            // interactive

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
            cli.HelpOption("-? | -h | --help");
            var prependOpt = cli.Option("-p|--prepend <TXT>", "Prepend text to each file name.", CommandOptionType.SingleValue);
            var replaceOpt = cli.Option("-r|--replace <TXT>", "Replace text in file name. Can be declared multiple times.", CommandOptionType.MultipleValue);
            var repWithOpt = cli.Option("-rw|--replace-with <TXT>", "Text to replace with. Must be used when using --replace option.", CommandOptionType.SingleValue);
            var titCaseOpt = cli.Option("-t|--title-case", "Capitalize the first letter of every word.", CommandOptionType.NoValue);
            var filterOpt = cli.Option("--filter <SEARCHPATTERN>", "Filter files to apply renaming to.", CommandOptionType.SingleValue);
            var modExtOpt = cli.Option("--mod-ext", "Include file extensions in renaming.", CommandOptionType.NoValue);
            var dryRunOpt = cli.Option("--dry-run", "Dry Run. Prints changes, and does not apply them.", CommandOptionType.NoValue);

            cli.OnExecute(() =>
            {
                var workingDirPath = dirArg.Value?.Trim('"') ?? Environment.CurrentDirectory;

                var dir = new DirectoryInfo(workingDirPath);
                if (!dir.Exists)
                {
                    CConsole.ErrorLine($"Directory \"{workingDirPath}\" does not exist.");
                    return 1;
                }

                CConsole.Info("Working directory: ");
                CConsole.WriteLine(dir.FullName);

                var actions = new List<IOption>();

                if (replaceOpt.HasValue())
                {
                    if (repWithOpt.HasValue() == false)
                    {
                        CConsole.ErrorLine("Missing --replace-with option.");
                        return 1;
                    }
                    actions.Add(new ReplaceTextOption(replaceOpt.Values!, repWithOpt.Value()!));
                }

                if (prependOpt.HasValue())
                {
                    actions.Add(new PrependTextOption(prependOpt.Value()!));
                }

                if (titCaseOpt.HasValue())
                {
                    actions.Add(new PascalCaseOption(true));
                }


                if (!actions.Any())
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
                    handler.ApplyOptions(actions);
                    if (handler.ExistingFile.Name.Length > longestFileName)
                        longestFileName = handler.ExistingFile.Name.Length;
                }

                foreach (var handler in handlers)
                {
                    Console.WriteLine($"{handler.ExistingFile.Name.PadRight(longestFileName)} => {handler.NewName}");
                }

                // If dry run

                return 0;
            });

            try
            {
                return cli.Execute(args);
            }
            catch
            {
                return 1;
            }


            //var dir = new DirectoryInfo(args.FirstOrDefault()?.Trim('"') ?? Environment.CurrentDirectory);
            //if (!dir.Exists)
            //{
            //    Console.WriteLine($"Directory {dir.FullName} does not exist.");
            //    return 1;
            //}

            //var options = new IOption[]
            //{
            //    new ReplaceTextOption(),
            //    new PrependTextOption(),
            //    new PascalCaseOption(),
            //};

            //foreach (var option in options)
            //{
            //    option.GetUserInput();
            //    Console.WriteLine();
            //}

            //foreach (var file in dir.GetFiles())
            //{
            //    var oldName = Path.GetFileNameWithoutExtension(file.FullName);
            //    var newName = oldName;
            //    foreach (var option in options)
            //    {
            //        newName = option.Apply(newName);
            //    }
            //    Console.WriteLine($"{oldName,-60} => {newName}");
            //}
            //Console.WriteLine();
        }
    }
}
