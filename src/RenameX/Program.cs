using McMaster.Extensions.CommandLineUtils;
using RenameX.FileSystem;
using RenameX.History;
using System;
using System.IO;
using System.Linq;

namespace RenameX;

public class Program
{
    public static int Main(string[] args)
    {
        // Ensure app data folder is created
        if (!Consts.AppDataDirectory.Exists)
            Consts.AppDataDirectory.Create();

        var fileSystem = new RealFileSystem();

        var app = new App(fileSystem);
        return app.Run(args);
    }
}