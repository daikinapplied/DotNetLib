using System;
using System.Collections.Generic;
using Serilog.Sinks.SystemConsole.Themes;

namespace Daikin.DotNetLib.Serilog
{
    public static class LogTheme
    {
        // https://github.com/serilog/serilog/wiki/Formatting-Output
        public const string TemplateSimple = "[{Timestamp:HH:mm:ss} {Level:u3} {EventId}] > {Message:lj}{Exception}{NewLine}";
        public const string TemplateDetail = "[{Timestamp:HH:mm:ss} {Level:u3} {EventId}] > {Message:lj}{NewLine}{Properties}{NewLine}{NewLine}";

        // https://github.com/serilog/serilog-sinks-console/tree/dev/src/Serilog.Sinks.Console/Sinks/SystemConsole/Themes
        public static SystemConsoleTheme System { get; } = new SystemConsoleTheme(
           new Dictionary<ConsoleThemeStyle, SystemConsoleThemeStyle>
           {
               [ConsoleThemeStyle.Text] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.White },
               [ConsoleThemeStyle.SecondaryText] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Gray },
               [ConsoleThemeStyle.TertiaryText] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.DarkGray },
               [ConsoleThemeStyle.Invalid] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Red },
               [ConsoleThemeStyle.Null] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Blue },
               [ConsoleThemeStyle.Name] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Gray },
               [ConsoleThemeStyle.String] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.DarkMagenta },
               [ConsoleThemeStyle.Number] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Magenta },
               [ConsoleThemeStyle.Boolean] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.DarkGreen },
               [ConsoleThemeStyle.Scalar] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Green },
               [ConsoleThemeStyle.LevelVerbose] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.White, Background = ConsoleColor.DarkGray },
               [ConsoleThemeStyle.LevelDebug] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Black, Background = ConsoleColor.Gray },
               [ConsoleThemeStyle.LevelInformation] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Black, Background = ConsoleColor.White },
               [ConsoleThemeStyle.LevelWarning] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Black, Background = ConsoleColor.Yellow },
               [ConsoleThemeStyle.LevelError] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.White, Background = ConsoleColor.Red },
               [ConsoleThemeStyle.LevelFatal] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.White, Background = ConsoleColor.DarkRed },
           }
        );
    }
}

