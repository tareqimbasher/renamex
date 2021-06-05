using RenameX.FileSystem;
using RenameX.RenamingStrategies;
using System;
using System.Collections.Generic;

namespace RenameX
{
    public class FileHandler
    {
        private readonly IFileSystem _fileSystem;
        private readonly string _existingFilePath;
        private readonly string _existingFileExtension;

        public FileHandler(IFileSystem fileSystem, string existingFilePath, bool modifyExtension)
        {
            _fileSystem = fileSystem;
            _existingFilePath = existingFilePath;
            _existingFileExtension = fileSystem.Path.GetExtension(existingFilePath);

            ModifyExtension = modifyExtension;
            OldName = fileSystem.Path.GetFileName(existingFilePath);
            NewName = OldName;
        }

        public string OldName { get; }
        public bool ModifyExtension { get; }
        public string NewName { get; private set; }

        public bool NewNameDiffersFromOld => NewName != OldName;

        public void Apply(IEnumerable<IRenamingStrategy> strategies)
        {
            var nameToModify = ModifyExtension ? OldName : _fileSystem.Path.GetFileNameWithoutExtension(OldName);

            foreach (var strategy in strategies)
            {
                nameToModify = strategy.TransformName(nameToModify);
            }

            NewName = ModifyExtension ? nameToModify : (nameToModify + _existingFileExtension);
        }

        public void DirectlyUpdateNewName(string newName)
        {
            NewName = newName;
        }

        public FileCommitResult Commit(out string? error)
        {
            error = null;

            if (!NewNameDiffersFromOld)
                return FileCommitResult.NameUnchanged;

            try
            {
                string workingDirPath = _fileSystem.Path.GetDirectoryName(_existingFilePath)!;
                string newFilePath = _fileSystem.Path.Combine(workingDirPath, NewName);

                // Prevent overwriting existing files
                if (_fileSystem.File.Exists(newFilePath))
                {
                    CConsole.Warning($"A file with name '{NewName}' already exists. File will not be renamed.");
                    return FileCommitResult.FileAlreadyExists;
                }

                // Perform actual rename
                _fileSystem.File.Move(
                    _fileSystem.Path.Combine(workingDirPath, OldName),
                    _fileSystem.Path.Combine(workingDirPath, NewName),
                    overwrite: false);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return FileCommitResult.Error;
            }

            return FileCommitResult.Success;
        }

        public string GetOldToNewNameString(int oldNamePadding = 0)
            => $"{OldName.PadRight(oldNamePadding)} => {NewName}";
    }
}
