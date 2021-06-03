using System;
using System.Collections.Generic;

namespace RenameX.Tests.Infrastructure
{
    public class MemoryDirectory
    {
        public MemoryDirectory(string fullName)
        {
            FullName = fullName;
            Files = new List<MemoryFile>();
        }

        public string FullName { get; }
        public List<MemoryFile> Files { get; }

        public MemoryDirectory AddFile(MemoryFile file)
        {
            if (MemoryPathAccessor.GetDirectoryNameDefault(file.FullName) != FullName)
                throw new Exception("File path does not corrolate to the this directory's path.");

            Files.Add(file);
            return this;
        }
    }
}
