namespace RenameX.History;

public class OperationLogEntry
{
    public OperationLogEntry(string oldName, string newName)
    {
        OldName = oldName;
        NewName = newName;
    }

    public string OldName { get; set; }
    public string NewName { get; set; }
}