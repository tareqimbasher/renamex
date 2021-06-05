using RenameX.RenamingStrategies;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RenameX
{
    public class Settings
    {
        public Settings(
            DirectoryInfo workingDirectory,
            string? filter,
            IEnumerable<string?>? replaceTexts,
            string? replaceWithText,
            string? prependText,
            bool titleCase,
            bool interactiveMode,
            bool modifyExtensions,
            bool verbose,
            bool dryRun,
            bool disableHistoryLog
        )
        {
            Directory = workingDirectory;
            Filter = filter;
            ReplaceTexts = replaceTexts?.ToArray() ?? new string?[0];
            ReplaceWithText = replaceWithText;
            PrependText = prependText;
            TitleCase = titleCase;
            InteractiveMode = interactiveMode;
            ModifyExtensions = modifyExtensions;
            Verbose = verbose;
            DryRun = dryRun;
            DisableHistoryLog = disableHistoryLog;
        }

        public DirectoryInfo Directory { get; }
        public string? Filter { get; }
        public string?[] ReplaceTexts { get; }
        public string? ReplaceWithText { get; }
        public string? PrependText { get; }
        public bool TitleCase { get; }
        public bool InteractiveMode { get; }
        public bool ModifyExtensions { get; }
        public bool Verbose { get; }
        public bool DryRun { get; }
        public bool DisableHistoryLog { get; }

        public bool Validate(out List<string> errors)
        {
            errors = new List<string>();

            if (!Directory.Exists)
            {
                errors.Add($"Directory \"{Directory.FullName}\" does not exist.");
                return false;
            }

            if (ReplaceTexts.Any() && ReplaceWithText == null)
            {
                errors.Add("Missing --replace-with option.");
                return false;
            }

            if (ReplaceWithText != null && !ReplaceTexts.Any())
            {
                errors.Add("Missing --replace option.");
                return false;
            }

            if (ReplaceWithText != null)
            {
                if (Path.GetInvalidFileNameChars().Any(c => ReplaceWithText.Contains(c)) ||
                    ReplaceWithText.Contains('/'))
                {
                    errors.Add("Invalid file name characters passed to --replace-with option.");
                    return false;
                }
            }

            return true;
        }

        public IEnumerable<IRenamingStrategy> GenerateRenamingStrategies()
        {
            var rules = new List<IRenamingStrategy>();

            if (ReplaceTexts.Any())
            {
                rules.Add(new ReplaceTextStrategy(ReplaceTexts, ReplaceWithText));
            }

            if (!string.IsNullOrEmpty(PrependText))
            {
                rules.Add(new PrependTextStrategy(PrependText));
            }

            if (TitleCase)
            {
                rules.Add(new TitleCaseStrategy(true));
            }

            return rules;
        }
    }
}
