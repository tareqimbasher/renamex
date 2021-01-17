using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenameX.Options
{
    public class PrependTextRule : IRenamingRule
    {
        public PrependTextRule()
        {
        }

        public PrependTextRule(string startingText)
        {
            StartingText = startingText;
        }

        public string? StartingText { get; set; }

        public void GetUserInput()
        {
            Console.WriteLine("* Text you want all files to start with:");
            Console.Write("- (Leave empty to disable this option): ");
            var startingText = Console.ReadLine();

            if (string.IsNullOrEmpty(startingText))
                StartingText = null;
            else
                StartingText = startingText;
        }

        public string Apply(string fileName)
        {
            if (StartingText == null)
                return fileName;

            if (!fileName.StartsWith(StartingText))
            {
                return StartingText + fileName;
            }
            return fileName;
        }
    }
}
