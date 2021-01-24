using RenameX.History;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RenameX
{
    public class RenameOperation
    {
        public RenameOperation(RenameSettings settings)
        {
            Settings = settings;
        }

        public RenameSettings Settings { get; }

        public bool Run()
        {
            var workingDir = Settings.Directory;

            // Print working directory
            if (Settings.Verbose)
            {
                CConsole.Info("Working directory: ");
                CConsole.WriteLine(workingDir.FullName);
            }

            var rules = Settings.GenerateRenamingRules();

            // At least one renaming rule must have been specified
            if (!rules.Any())
            {
                CConsole.ErrorLine("No options specified.");
                return false;
            }

            var enumeratedFiles = Settings.Filter != null ?
                workingDir.EnumerateFiles(Settings.Filter, new EnumerationOptions() { IgnoreInaccessible = true }) :
                workingDir.EnumerateFiles();

            var handlers = new List<FileHandler>();
            int longestFileName = 0;

            foreach (var handler in enumeratedFiles.Select(f => new FileHandler(f, Settings.ModifyExtensions)))
            {
                handlers.Add(handler);
                handler.ApplyRules(rules);
                if (handler.OldName.Length > longestFileName)
                    longestFileName = handler.OldName.Length;
            }

            if (Settings.InteractiveMode)
            {
                var tmpFile = Path.GetTempFileName();
                File.Move(tmpFile, tmpFile + ".txt");
                tmpFile += ".txt";

                File.WriteAllLines(
                    tmpFile,
                    handlers.Select(h => h.GetOldToNewNameString(longestFileName)).ToArray());

                CConsole.InfoLine("Waiting for text editor to close...");

                Process.Start(new ProcessStartInfo(tmpFile) { UseShellExecute = true })?.WaitForExit();

                var editedNames = File.ReadAllLines(tmpFile).Select(l => l.Split("=>").Select(x => x.Trim()).ToArray()).ToArray();
                File.Delete(tmpFile);

                handlers = handlers.Where(h =>
                {
                    var editedName = editedNames.FirstOrDefault(en => en[0] == h.OldName);
                    if (editedName == null)
                        return false;

                    h.NewName = editedName[1];
                    return true;
                })
                .ToList();
            }

            // Filter out files where the name hasn't changed
            handlers = handlers.Where(h => h.OldName != h.NewName).ToList();

            if (!handlers.Any())
            {
                CConsole.Info("No files were renamed!");
                return true;
            }

            // If dry run
            if (Settings.DryRun)
            {
                if (Settings.Verbose)
                {
                    foreach (var handler in handlers)
                    {
                        CConsole.SuccessLine($"[DRYRUN] {handler.GetOldToNewNameString(longestFileName)}");
                    }
                }
            }
            else
            {
                if (Settings.InteractiveMode)
                {
                    CConsole.Info($"\nThis will rename {handlers.Count} files. Are you sure you want to continue? [y/N]: ");
                    if (Console.ReadLine()?.ToLower() != "y")
                    {
                        CConsole.WarningLine("Cancelled.");
                        return false;
                    }
                }

                var history = new DirectoryHistory(workingDir).Load();
                var opLog = new OperationLog(DateTime.UtcNow);

                foreach (var handler in handlers)
                {
                    // If name has not changed, skip it
                    if (handler.OldName == handler.NewName)
                        continue;

                    // Prevent overwriting existing files
                    string newFilePath = workingDir.PathCombine(handler.NewName);
                    if (File.Exists(newFilePath))
                    {
                        CConsole.Warning($"A file with name '{handler.NewName}' already exists. File will not be renamed.");
                        continue;
                    }

                    // Perform actual rename
                    File.Move(workingDir.PathCombine(handler.OldName), workingDir.PathCombine(handler.NewName), overwrite: false);

                    // Register an operation log entry
                    opLog.Entries.Add(new OperationLogEntry(handler.OldName, handler.NewName));

                    // If verbose, print the rename that took place
                    if (Settings.Verbose)
                        CConsole.SuccessLine($"[RENAMED] {handler.GetOldToNewNameString(longestFileName)}");
                }

                if (!opLog.Entries.Any())
                {
                    CConsole.Info("No files were renamed!");
                }
                else if (!Settings.DisableHistoryLog)
                {
                    history.Logs.Add(opLog);
                    history.Save();
                }
            }

            return true;
        }
    }
}
