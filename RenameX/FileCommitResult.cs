using System;
using System.Collections.Generic;
using System.Text;

namespace RenameX
{
    public enum FileCommitResult
    {
        Success = 1,
        NameUnchanged = 2,
        FileAlreadyExists = 3,
        Error = 3
    }
}
