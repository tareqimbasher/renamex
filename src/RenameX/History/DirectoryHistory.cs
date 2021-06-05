using RenameX.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RenameX.History
{
    public class DirectoryHistory
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        private readonly DirectoryInfo _directory;
        private readonly IFileSystem _fileSystem;
        private readonly FileInfo _historyFile;


        public DirectoryHistory(DirectoryInfo directory, IFileSystem fileSystem)
        {
            _directory = directory;
            _fileSystem = fileSystem;
            _historyFile = new FileInfo(fileSystem.Path.Combine(Consts.AppDataDirectory.FullName, _directory.GetHistoryFileName()));
            Logs = new List<OperationLog>();
        }

        public List<OperationLog> Logs { get; set; }

        public DirectoryHistory Load()
        {
            if (!_historyFile.Exists)
                return this;

            var json = _fileSystem.File.ReadAllText(_historyFile.FullName);
            if (string.IsNullOrWhiteSpace(json))
                json = "[]";

            var logs = JsonSerializer.Deserialize<List<OperationLog>>(json, _jsonOptions);
            if (logs == null)
                throw new Exception("Could not deserialize operation logs");

            Logs = logs.OrderBy(x => x.DateUtc).ToList();
            return this;
        }

        public DirectoryHistory Save()
        {
            _fileSystem.File.WriteAllText(_historyFile.FullName, JsonSerializer.Serialize(Logs, _jsonOptions));
            return this;
        }
    }
}
