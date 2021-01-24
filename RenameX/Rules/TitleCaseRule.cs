using System;
using System.Globalization;

namespace RenameX.Rules
{
    public class TitleCaseRule : IRenamingRule
    {
        public TitleCaseRule()
        {
        }

        public TitleCaseRule(bool convertToPascalCase)
        {
            ConvertToTitleCase = convertToPascalCase;
        }

        public bool ConvertToTitleCase { get; set; }

        public void GetUserInput()
        {
            Console.WriteLine("* Do you want to capitalize the beginning of every word? ");
            Console.Write("- [y/N]: ");
            ConvertToTitleCase = Console.ReadLine()?.ToLower() == "y";
        }

        public string Apply(string fileName)
        {
            if (ConvertToTitleCase)
            {
                TextInfo info = CultureInfo.CurrentCulture.TextInfo;
                return info.ToTitleCase(fileName);
            }
            return fileName;
        }
    }
}
