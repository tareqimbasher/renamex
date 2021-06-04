using RenameX.FileSystem;
using RenameX.RenamingStrategies;
using System;
using System.Collections.Generic;
using System.IO;

namespace RenameX
{
    public class FileHandler
    {
        private readonly IFileSystem _fileSystem;
        private readonly FileInfo _existingFile;

        public FileHandler(IFileSystem fileSystem, FileInfo existingFile, bool modifyExtension)
        {
            _fileSystem = fileSystem;
            _existingFile = existingFile;
            ModifyExtension = modifyExtension;
            NewName = OldName;
        }

        public string OldName => _existingFile.Name;
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

            NewName = ModifyExtension ? nameToModify : (nameToModify + _existingFile.Extension);
        }

        public void DirectlyUpdateNewName(string newName)
        {
            NewName = newName;
        }

        public FileCommitResult Commit()
        {
            if (!NewNameDiffersFromOld)
                return FileCommitResult.NameUnchanged;

            DirectoryInfo workingDir = _existingFile.Directory;
            string newFilePath = _fileSystem.Path.Combine(workingDir.FullName, NewName);

            // Prevent overwriting existing files
            if (_fileSystem.File.Exists(newFilePath))
            {
                CConsole.Warning($"A file with name '{NewName}' already exists. File will not be renamed.");
                return FileCommitResult.FileAlreadyExists;
            }

            // Perform actual rename
            try
            {
                _fileSystem.File.Move(
                    _fileSystem.Path.Combine(workingDir.FullName, OldName),
                    _fileSystem.Path.Combine(workingDir.FullName, NewName),
                    overwrite: false);
            }
            catch (Exception ex)
            {
                CConsole.Error($"Error renaming file: {OldName}. {ex.Message}");
                return FileCommitResult.Error;
            }

            return FileCommitResult.Success;
        }

        public string GetOldToNewNameString(int oldNamePadding = 0)
            => $"{OldName.PadRight(oldNamePadding)} => {NewName}";
    }
}
