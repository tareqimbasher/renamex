using System.Globalization;

namespace RenameX.RenamingStrategies;

public class TitleCaseStrategy : IRenamingStrategy
{
    public TitleCaseStrategy(bool convertToTitleCase)
    {
        ConvertToTitleCase = convertToTitleCase;
    }

    public bool ConvertToTitleCase { get; set; }

    public string? TransformName(string? name)
    {
        if (!ConvertToTitleCase || string.IsNullOrWhiteSpace(name))
            return name;

        TextInfo info = CultureInfo.CurrentCulture.TextInfo;
        return info.ToTitleCase(name);
    }
}