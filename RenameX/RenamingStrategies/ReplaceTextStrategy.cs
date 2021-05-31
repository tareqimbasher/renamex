using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RenameX.RenamingStrategies
{
    public class ReplaceTextStrategy : IRenamingStrategy
    {

        public ReplaceTextStrategy(IEnumerable<string> textsToReplace, string? replaceWith)
        {
            TextsToReplace = textsToReplace ?? throw new ArgumentNullException(nameof(textsToReplace));
            ReplaceWith = replaceWith ?? string.Empty;
        }

        public IEnumerable<string> TextsToReplace { get; set; }
        public string? ReplaceWith { get; set; }

        public string TransformName(string name)
        {
            if (!TextsToReplace.Any())
                return name;

            foreach (var text in TextsToReplace)
            {
                name = name.Replace(text, ReplaceWith);
            }

            return name;
        }

        //public override void Apply(FileHandler file)
        //{
        //    if (TextsToReplace?.Any() == true)
        //    {
        //        file.Apply(name =>
        //        {
        //            var nameToModify = name;
        //            var ext = Path.GetExtension(name);
        //            if (Settings.ModifyExtensions == false)
        //                nameToModify = Path.GetFileNameWithoutExtension(name);

        //            foreach (var word in TextsToReplace)
        //            {
        //                nameToModify = nameToModify.Replace(word, ReplaceWith);
        //            }

        //            return nameToModify + (Settings.ModifyExtensions ? "" : ext);
        //        });
        //    }
        //}
    }
}
