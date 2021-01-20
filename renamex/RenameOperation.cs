using RenameX.History;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                handler.ApplyOptions(rules);
                if (handler.ExistingFile.Name.Length > longestFileName)
                    longestFileName = handler.ExistingFile.Name.Length;
            }

            string getOldAndNewName(string oldName, string newName) => $"{oldName.PadRight(longestFileName)} => {newName}";

            if (Settings.InteractiveMode)
            {
                var tmpFile = Path.GetTempFileName();
                File.Move(tmpFile, tmpFile + ".txt");
                tmpFile += ".txt";

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
            if (Settings.DryRun)
            {
                foreach (var handler in handlers)
                {
                    if (Settings.Verbose)
                        Console.WriteLine(getOldAndNewName(handler.ExistingFile.Name, handler.NewName));
                }
            }
            else
            {
                var history = new DirectoryHistory(workingDir).Load();
                var opLog = new OperationLog(DateTime.UtcNow);

                foreach (var handler in handlers)
                {
                    if (handler.ExistingFile.Name == handler.NewName)
                        continue;

                    File.Move(handler.ExistingFile.FullName, workingDir.PathCombine(handler.NewName), overwrite: false);
                    opLog.Entries.Add(new OperationLogEntry(handler.ExistingFile.Name, handler.NewName));
                    if (Settings.Verbose)
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
            }


            return true;
        }
    }
}
