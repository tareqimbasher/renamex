using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenameX
{
    public static class Extensions
    {
        public static string PathCombine(this DirectoryInfo directory, params string[] paths)
        {
            string fullPath = directory.FullName;
            foreach (var path in paths)
            {
                fullPath = Path.Combine(fullPath, path);
            }
            return fullPath;
        }

        public static string GetHistoryFileName(this DirectoryInfo directoryInfo)
        {
            return directoryInfo.FullName.RemoveInvalidPathChars("_") + ".json";
        }

        public static string RemoveInvalidPathChars(this string path, string replaceInvalidCharsWith)
        {
            string result = path;
            var invalidChars = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars()).ToHashSet();
            foreach (var c in invalidChars)
                result = result.Replace(c.ToString(), replaceInvalidCharsWith);
            return result;
        }
    }
}
