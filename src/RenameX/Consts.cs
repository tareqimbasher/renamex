using System;
using System.IO;

namespace RenameX
{
    public static class Consts
    {
        static Consts()
        {
            AppDataDirectory = new DirectoryInfo(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "renamex"));
        }

        public static DirectoryInfo AppDataDirectory { get; }
    }
}
