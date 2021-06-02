using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RenameX.Tests
{
    public class InMemoryFileSystem
    {
        private List<FileInfo> _files;

        public InMemoryFileSystem()
        {
            _files = new List<FileInfo>();
        }

        public IEnumerable<FileInfo> EnumerateFiles(string searchPattern, EnumerationOptions enumerationOptions)
        {
            return _files;
        }
    }
}
