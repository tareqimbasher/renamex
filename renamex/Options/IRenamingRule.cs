namespace RenameX.Options
{
    public interface IRenamingRule
    {
        void GetUserInput();
        string Apply(string fileName);
    }
}