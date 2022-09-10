using System.IO;

namespace RenameX;

public static class Extensions
{
    //public static string PathCombine(this DirectoryInfo directory, IPath pathUtil, params string[] paths)
    //{
    //    string fullPath = directory.FullName;
    //    foreach (var path in paths)
    //    {
    //        fullPath = pathUtil.Combine(fullPath, path);
    //    }
    //    return fullPath;
    //}

    public static string GetHistoryFileName(this DirectoryInfo directoryInfo)
    {
        return directoryInfo.FullName.Trim('/').Trim('\\').Replace("\\", "_").Replace("/", "_") + ".json";
    }
}