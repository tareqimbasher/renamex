using RenameX.Rules;
using System.Collections.Generic;
using System.IO;

namespace RenameX
{
    public class FileHandler
    {
        private readonly FileInfo _existingFile;

        public FileHandler(FileInfo existingFile, bool modifyExtension)
        {
            _existingFile = existingFile;
            ModifyExtension = modifyExtension;
        }

        public string OldName => _existingFile.Name;
        public bool ModifyExtension { get; }
        public string NewName { get; set; }

        public void ApplyRules(IEnumerable<IRenamingRule> rules)
        {
            var nameToModify = ModifyExtension ? OldName : Path.GetFileNameWithoutExtension(OldName);

            foreach (var rule in rules)
            {
                nameToModify = rule.Apply(nameToModify);
            }

            NewName = ModifyExtension ? nameToModify : (nameToModify + _existingFile.Extension);
        }

        public string GetOldToNewNameString(int oldNamePadding = 0)
            => $"{OldName.PadRight(oldNamePadding)} => {NewName}";
    }
}
