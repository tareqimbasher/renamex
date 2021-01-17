using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace RenameX.History
{
    public class DirectoryHistory
    {
        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        private readonly DirectoryInfo _directory;
        private readonly FileInfo _historyFile;


        public DirectoryHistory(string directoryPath)
        {
            _directory = new DirectoryInfo(directoryPath);
            _historyFile = new FileInfo(Consts.AppDataDirectory.PathCombine(_directory.GetHistoryFileName()));
            Logs = new List<OperationLog>();
        }

        public List<OperationLog> Logs { get; set; }

        public DirectoryHistory Load()
        {
            if (!_historyFile.Exists)
                return this;

            var json = File.ReadAllText(_historyFile.FullName);
            if (string.IsNullOrWhiteSpace(json))
                json = "[]";

            var logs = JsonSerializer.Deserialize<List<OperationLog>>(json);
            if (logs == null)
                throw new Exception("Could not deserialize operation logs");

            Logs = logs.OrderBy(x => x.DateUtc).ToList();
            return this;
        }

        public DirectoryHistory Save()
        {
            File.WriteAllText(_historyFile.FullName, JsonSerializer.Serialize(Logs, _jsonOptions));
            return this;
        }
    }
}
