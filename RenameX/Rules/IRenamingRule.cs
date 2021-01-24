namespace RenameX.Rules
{
    public interface IRenamingRule
    {
        void GetUserInput();
        string Apply(string fileName);
    }
}