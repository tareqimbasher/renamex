using RenameX.FileSystem;
using RenameX.History;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RenameX;

public class RenameOperation
{
    private readonly IFileSystem _fileSystem;
    private readonly string[] _args;

    public RenameOperation(IFileSystem fileSystem, Settings settings, string[] args)
    {
        _fileSystem = fileSystem;
        Settings = settings;
        _args = args;
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
            CConsole.InfoLine("No actions have been taken.");
            return true;
        }

        var handlers = new List<FileHandler>();

        foreach (var handler in GetFilesToRename().Select(f => new FileHandler(_fileSystem, f, Settings.ModifyExtensions)))
        {
            handlers.Add(handler);
            handler.Apply(strategies);
        }

        if (!handlers.Any())
        {
            CConsole.InfoLine("No files to rename.");
            return true;
        }

        int longestFileName = handlers.Max(h => h.OldName.Length);

        if (Settings.InteractiveMode)
        {
            StartInteractiveEditing(handlers, longestFileName);
        }

        // Filter out files where the name hasn't changed
        handlers = handlers.Where(h => h.NewNameDiffersFromOld).ToList();

        if (!handlers.Any())
        {
            CConsole.InfoLine("No files were renamed.");
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

            var history = new DirectoryHistory(workingDir, _fileSystem).Load();
            var opLog = new OperationLog(DateTime.UtcNow, _args);

            foreach (var handler in handlers)
            {
                FileCommitResult result = handler.Commit(out var error);

                if (result == FileCommitResult.Success)
                {
                    // Register an operation log entry
                    opLog.Entries.Add(new OperationLogEntry(handler.OldName, handler.NewName));

                    // If verbose, print the rename that took place
                    if (Settings.Verbose)
                        CConsole.SuccessLine($"[RENAMED] {handler.GetOldToNewNameString(longestFileName)}");
                }
                else if (result == FileCommitResult.FileAlreadyExists)
                {
                    CConsole.WarningLine($"File '{handler.OldName}' was not renamed. A file named '{handler.NewName}' already exists.");
                }
                else if (result == FileCommitResult.Error)
                {
                    CConsole.WarningLine($"'{handler.OldName}' was not renamed to '{handler.NewName}'. Error: {error}");
                }
            }

            if (!opLog.Entries.Any())
            {
                CConsole.InfoLine("No files were renamed.");
            }
            else if (!Settings.DisableHistoryLog)
            {
                history.Logs.Add(opLog);
                history.Save();
            }
        }

        return true;
    }

    private void StartInteractiveEditing(List<FileHandler> handlers, int longestFileName)
    {
        var tmpFile = _fileSystem.Path.GetTempFileName();
        _fileSystem.File.Move(tmpFile, tmpFile + ".txt");
        tmpFile += ".txt";

        try
        {
            _fileSystem.File.WriteAllLines(
                tmpFile,
                handlers.Select(h => h.GetOldToNewNameString(longestFileName)).ToArray());

            CConsole.InfoLine("Waiting for text editor to close...");

            var process = Process.Start(new ProcessStartInfo(tmpFile) { UseShellExecute = true });

            if (process == null)
            {
                CConsole.ErrorLine($"Could not start tmp file at: {tmpFile}");
                return;
            }
                
            process.WaitForExit();

            var editMap = _fileSystem.File.ReadAllLines(tmpFile)
                .Select(l => l.Split("=>").Select(x => x.Trim()).ToArray())
                .ToArray();

            var handlersToRemove = new List<FileHandler>();

            foreach (var handler in handlers)
            {
                var edit = editMap.FirstOrDefault(l => l[0] == handler.OldName);

                // If user removes the line in interactive mode, they don't want to rename that file
                if (edit == null)
                {
                    handlersToRemove.Add(handler);
                    continue;
                }

                string newName = edit[1];

                handler.DirectlyUpdateNewName(newName);
            }

            foreach (var handler in handlersToRemove)
            {
                handlers.Remove(handler);
            }
        }
        finally
        {
            if (_fileSystem.File.Exists(tmpFile))
                _fileSystem.File.Delete(tmpFile);
        }
    }

    public IEnumerable<string> GetFilesToRename()
    {
        return Directory.EnumerateFiles(
            Settings.Directory.FullName,
            Settings.Filter ?? "*",
            new EnumerationOptions()
            {
                IgnoreInaccessible = true
                //AttributesToSkip // TODO provide option
            });
    }
}