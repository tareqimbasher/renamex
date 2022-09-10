using System;

namespace RenameX.Tests.Infrastructure;

public class FileNotFoundException : Exception
{
    public FileNotFoundException(string message) : base(message)
    {
    }
}