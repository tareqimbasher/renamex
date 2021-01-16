namespace RenameX.Options
{
    public interface IOption
    {
        void GetUserInput();
        string Apply(string fileName);
    }
}