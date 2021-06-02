using System;
using System.Collections.Generic;
using System.Linq;

namespace RenameX.RenamingStrategies
{
    public class ReplaceTextStrategy : IRenamingStrategy
    {
        public ReplaceTextStrategy(IEnumerable<string?> textsToReplace, string? replaceWith)
        {
            TextsToReplace = textsToReplace ?? throw new ArgumentNullException(nameof(textsToReplace));
            ReplaceWith = replaceWith ?? string.Empty;
        }

        public IEnumerable<string?> TextsToReplace { get; set; }
        public string? ReplaceWith { get; set; }

        public string? TransformName(string? name)
        {
            if (!TextsToReplace.Any() || name == null)
                return name;

            foreach (var text in TextsToReplace)
            {
                if (text == null || text == string.Empty)
                    continue;

                name = name.Replace(text, ReplaceWith);
            }

            return name;
        }
    }
}
