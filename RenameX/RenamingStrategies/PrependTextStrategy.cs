using System;

namespace RenameX.RenamingStrategies
{
    public class PrependTextStrategy : IRenamingStrategy
    {
        public PrependTextStrategy(string? textToPrepend)
        {
            TextToPrepend = textToPrepend;
        }

        public string? TextToPrepend { get; set; }

        public string TransformName(string name)
        {
            if (string.IsNullOrEmpty(TextToPrepend))
                return name;

            return name.StartsWith(TextToPrepend) ? name : (TextToPrepend + name);
        }

        //public override void Apply(FileHandler file)
        //{
        //    if (TextToPrepend == null)
        //        return;

        //    if (!file.OldName.StartsWith(TextToPrepend))
        //    {
        //        file.Apply(name => TextToPrepend + name);
        //    }
        //}
    }
}
