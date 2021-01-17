using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RenameX
{
    public static class Consts
    {
        static Consts()
        {
            AppDataDirectory = new DirectoryInfo(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "renamex"));
        }

        public static DirectoryInfo AppDataDirectory { get; }
    }
}
