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
        public RenameOperation(Settings settings)
        {
            Settings = settings;
        }

        public Settings Settings { get; }

        public bool Run()
        {
            var workingDir = Settings.Directory;

            // Print working directory
            if (Settings.Verbose)
            {
                CConsole.Info("Working directory: ");
                CConsole.WriteLine(workingDir.FullName);
            }

            var strategies = Settings.GenerateRenamingStrategies();

            if (!strategies.Any())
            {
                CConsole.ErrorLine("No actions have been taken.");
                return false;
            }

            var filesToRename = GetFilesToRename();

            var handlers = new List<FileHandler>();
            int longestFileName = 0;

            foreach (var handler in filesToRename.Select(f => new FileHandler(f, Settings.ModifyExtensions)))
            {
                handlers.Add(handler);

                if (handler.OldName.Length > longestFileName)
                    longestFileName = handler.OldName.Length;

                handler.Apply(strategies);
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

                Process.Start(new ProcessStartInfo(tmpFile) { UseShellExecute = true }).WaitForExit();

                var editedNames = File.ReadAllLines(tmpFile).Select(l => l.Split("=>").Select(x => x.Trim()).ToArray()).ToArray();
                File.Delete(tmpFile);

                handlers = handlers.Where(h =>
                {
                    var editedName = editedNames.FirstOrDefault(en => en[0] == h.OldName);
                    if (editedName == null)
                        return false;

                    h.DirectlyUpdateNewName(editedName[1]);
                    return true;
                })
                .ToList();
            }

            // Filter out files where the name hasn't changed
            handlers = handlers.Where(h => h.NewNameDiffersFromOld).ToList();

            if (!handlers.Any())
            {
                CConsole.Info("No files were renamed.");
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
                    FileCommitResult result = handler.Commit();

                    if (result == FileCommitResult.Success)
                    {
                        // Register an operation log entry
                        opLog.Entries.Add(new OperationLogEntry(handler.OldName, handler.NewName));

                        // If verbose, print the rename that took place
                        if (Settings.Verbose)
                            CConsole.SuccessLine($"[RENAMED] {handler.GetOldToNewNameString(longestFileName)}");
                    }
                }

                if (!opLog.Entries.Any())
                {
                    CConsole.Info("No files were renamed.");
                }
                else if (!Settings.DisableHistoryLog)
                {
                    history.Logs.Add(opLog);
                    history.Save();
                }
            }

            return true;
        }

        public IEnumerable<FileInfo> GetFilesToRename()
        {
            return Settings.Directory.EnumerateFiles(
                Settings.Filter ?? string.Empty,
                new EnumerationOptions()
                {
                    IgnoreInaccessible = true
                    //AttributesToSkip // TODO provide option
                });
        }
    }
}
