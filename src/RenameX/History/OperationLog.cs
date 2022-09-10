using System;
using System.Collections.Generic;

namespace RenameX.History;

public class OperationLog
{
    public OperationLog(DateTime dateUtc, string[] args)
    {
        DateUtc = dateUtc;
        Args = args;
        Entries = new List<OperationLogEntry>();
    }

    public DateTime DateUtc { get; set; }
    public string[] Args { get; }
    public List<OperationLogEntry> Entries { get; set; }
}