using System;
using System.Collections.Generic;

namespace RenameX.History
{
    public class OperationLog
    {
        public OperationLog() : this(DateTime.UtcNow)
        {
        }

        public OperationLog(DateTime dateUtc)
        {
            DateUtc = dateUtc;
            Entries = new List<OperationLogEntry>();
        }

        public DateTime DateUtc { get; set; }
        public List<OperationLogEntry> Entries { get; set; }
    }
}
