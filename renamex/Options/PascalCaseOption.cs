using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenameX.Options
{
    public class PascalCaseOption : IOption
    {
        public PascalCaseOption()
        {
        }

        public PascalCaseOption(bool convertToPascalCase)
        {
            ConvertToPascalCase = convertToPascalCase;
        }

        public bool ConvertToPascalCase { get; set; }

        public void GetUserInput()
        {
            Console.WriteLine("* Do you want to capitalize the beginning of every word? ");
            Console.Write("- [y/N]: ");
            ConvertToPascalCase = Console.ReadLine()?.ToLower() == "y";
        }

        public string Apply(string fileName)
        {
            if (ConvertToPascalCase)
            {
                TextInfo info = CultureInfo.CurrentCulture.TextInfo;
                return info.ToTitleCase(fileName);
            }
            return fileName;
        }
    }
}
