using System;
using System.Collections.Generic;
using System.Linq;

namespace RenameX.Rules
{
    public class ReplaceTextRule : IRenamingRule
    {
        public ReplaceTextRule()
        {
        }

        public ReplaceTextRule(IEnumerable<string> wordsToReplace, string replaceWith)
        {
            WordsToReplace = wordsToReplace.ToArray();
            ReplaceWith = replaceWith;
        }


        public string[]? WordsToReplace { get; set; }
        public string? ReplaceWith { get; set; }

        public void GetUserInput()
        {
            Console.WriteLine("* Text you want to replace:");
            Console.Write("- Separate multiple entries with a | (Leave empty to disable this option): ");
            var textToReplace = Console.ReadLine();

            if (string.IsNullOrEmpty(textToReplace))
                WordsToReplace = null;
            else
                WordsToReplace = textToReplace.Split('|');

            if (WordsToReplace != null)
            {
                Console.Write("- Replace with: ");
                ReplaceWith = Console.ReadLine();
            }
        }

        public string Apply(string fileName)
        {
            if (WordsToReplace?.Any() == true)
            {
                foreach (var word in WordsToReplace)
                {
                    fileName = fileName.Replace(word, ReplaceWith);
                }
            }
            return fileName;
        }
    }
}
