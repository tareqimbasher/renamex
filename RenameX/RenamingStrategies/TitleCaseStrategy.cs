using System;
using System.Globalization;
using System.IO;

namespace RenameX.RenamingStrategies
{
    public class TitleCaseStrategy : IRenamingStrategy
    {
        public TitleCaseStrategy(bool convertToTitleCase)
        {
            ConvertToTitleCase = convertToTitleCase;
        }

        public bool ConvertToTitleCase { get; set; }

        public string TransformName(string name)
        {
            if (!ConvertToTitleCase)
                return name;

            TextInfo info = CultureInfo.CurrentCulture.TextInfo;
            return info.ToTitleCase(name);
        }


        //public override void Apply(FileHandler file)
        //{
        //    if (ConvertToTitleCase)
        //    {
        //        file.Apply(name =>
        //        {
        //            var nameToModify = name;
        //            var ext = Path.GetExtension(name);
        //            if (Settings.ModifyExtensions == false)
        //                nameToModify = Path.GetFileNameWithoutExtension(name);

        //            TextInfo info = CultureInfo.CurrentCulture.TextInfo;
        //            nameToModify = info.ToTitleCase(nameToModify);

        //            return nameToModify + (Settings.ModifyExtensions ? "" : ext);
        //        });
        //    }
        //}
    }
}
