﻿using RenameX.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenameX
{
    public class RenameSettings
    {
        public RenameSettings(
            string workingDirectoryPath,
            string? filter,
            List<string?>? replaceTexts,
            string? replaceWithText,
            string? prependText,
            bool titleCase,
            bool interactiveMode,
            bool modifyExtensions,
            bool verbose,
            bool dryRun
        )
        {
            Directory = new DirectoryInfo(workingDirectoryPath);
            Filter = filter;
            ReplaceTexts = replaceTexts;
            ReplaceWithText = replaceWithText;
            PrependText = prependText;
            TitleCase = titleCase;
            InteractiveMode = interactiveMode;
            ModifyExtensions = modifyExtensions;
            Verbose = verbose;
            DryRun = dryRun;
        }

        public DirectoryInfo Directory { get; }
        public string? Filter { get; }
        public List<string?>? ReplaceTexts { get; }
        public string? ReplaceWithText { get; }
        public string? PrependText { get; }
        public bool TitleCase { get; }
        public bool InteractiveMode { get; }
        public bool ModifyExtensions { get; }
        public bool Verbose { get; }
        public bool DryRun { get; }

        public bool Validate(out List<string> errors)
        {
            errors = new List<string>();

            if (!Directory.Exists)
            {
                errors.Add($"Directory \"{Directory.FullName}\" does not exist.");
                return false;
            }

            if (ReplaceTexts?.Any() == true && ReplaceWithText == null)
            {
                errors.Add("Missing --replace-with option.");
                return false;
            }

            if (ReplaceWithText != null)
            {
                if (Path.GetInvalidFileNameChars().Any(c => ReplaceWithText.Contains(c)))
                {
                    errors.Add("Invalid file name characters passed to --replace-with option.");
                    return false;
                }
            }

            return true;
        }

        public IEnumerable<IRenamingRule> GenerateRenamingRules()
        {
            var rules = new List<IRenamingRule>();

            if (ReplaceTexts?.Any() == true)
            {
                rules.Add(new ReplaceTextRule(ReplaceTexts!, ReplaceWithText!));
            }

            if (PrependText != null)
            {
                rules.Add(new PrependTextRule(PrependText));
            }

            if (TitleCase)
            {
                rules.Add(new TitleCaseRule(true));
            }

            return rules;
        }
    }
}