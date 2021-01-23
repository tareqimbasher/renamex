﻿using RenameX.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenameX
{
    public class FileHandler
    {
        public FileHandler(FileInfo existingFile, bool modifyExtension)
        {
            ExistingFile = existingFile;
            ModifyExtension = modifyExtension;
        }

        private FileInfo ExistingFile { get; }
        public string OldName => ExistingFile.Name;
        public bool ModifyExtension { get; }
        public string NewName { get; set; }

        public void ApplyOptions(IEnumerable<IRenamingRule> rules)
        {
            var nameToModify = ModifyExtension ? ExistingFile.Name : Path.GetFileNameWithoutExtension(ExistingFile.Name);

            foreach (var rule in rules)
            {
                nameToModify = rule.Apply(nameToModify);
            }

            NewName = ModifyExtension ? nameToModify : (nameToModify + ExistingFile.Extension);
        }

        public string GetOldToNewNameString(int oldNamePadding = 0)
            => $"{OldName.PadRight(oldNamePadding)} => {NewName}";
    }
}
