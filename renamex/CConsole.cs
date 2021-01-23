using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenameX
{
    public static class CConsole
    {
        public static void Write(string text) => Write(text, Console.ForegroundColor);

        public static void Write(string text, ConsoleColor consoleColor)
        {
            var current = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor;
            Console.Write(text);
            Console.ForegroundColor = current;
        }
        public static void WriteLine(string text) => Write(text + "\n", Console.ForegroundColor);
        public static void WriteLine(string text, ConsoleColor consoleColor) => Write(text + "\n", consoleColor);

        public static void Info(string text) => Write(text, ConsoleColor.Cyan);
        public static void InfoLine(string text) => Info(text + "\n");

        public static void Success(string text) => Write(text, ConsoleColor.Green);
        public static void SuccessLine(string text) => Success(text + "\n");

        public static void Warning(string text) => Write($"{text}", ConsoleColor.Yellow);
        public static void WarningLine(string text) => Warning(text + "\n");

        public static void Error(string text) => Write(text, ConsoleColor.Red);
        public static void ErrorLine(string text) => Error(text + "\n");
    }
}
