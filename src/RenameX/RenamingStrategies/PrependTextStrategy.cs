namespace RenameX.RenamingStrategies;

public class PrependTextStrategy : IRenamingStrategy
{
    public PrependTextStrategy(string? textToPrepend)
    {
        TextToPrepend = textToPrepend;
    }

    public string? TextToPrepend { get; set; }

    public string? TransformName(string? name)
    {
        if (string.IsNullOrEmpty(TextToPrepend))
            return name;

        if (name == null)
            name = string.Empty;

        return name.StartsWith(TextToPrepend) ? name : (TextToPrepend + name);
    }
}