using McMaster.Extensions.CommandLineUtils;

namespace RenameX;

public static class CliArgs
{
    public static CommandOption FilterOption { get; }
    public static CommandOption PrependOption { get; }
    public static CommandOption ReplaceOption { get; }
    public static CommandOption ReplaceWithOption { get; }
    public static CommandOption TitleCaseOption { get; }
    public static CommandOption InteractiveOption { get; }
    public static CommandOption VerboseOption { get; }
    public static CommandOption ModifyExtensionsOption { get; }
    public static CommandOption DisableLoggingOption { get; }
    public static CommandOption DryRunOption { get; }

    static CliArgs()
    {
        FilterOption = new CommandOption("-f|--filter <SEARCHPATTERN>", CommandOptionType.SingleValue)
        {
            Description = "Filter the files to rename. Selects all files in specified directory by default."
        };

        PrependOption = new CommandOption("-p|--prepend <TXT>", CommandOptionType.SingleValue)
        {
            Description = "Prepend text to each file name. If specified text already exists " +
                          "at the start of a file name, it will not be additionally prepended."
        };

        ReplaceOption = new CommandOption("-r|--replace <TXT>", CommandOptionType.MultipleValue)
        {
            Description = "Replace the specified text in file name. " +
                          "Can be used multiple times to specify multiple text values to replace."
        };

        ReplaceWithOption = new CommandOption("-rw|--replace-with <TXT>", CommandOptionType.SingleValue)
        {
            Description = "Text to replace with. Required when using --replace option."
        };

        TitleCaseOption = new CommandOption("-t|--title-case", CommandOptionType.NoValue)
        {
            Description = "Capitalize the first letter of every word."
        };

        InteractiveOption = new CommandOption("-i|--interactive", CommandOptionType.NoValue)
        {
            Description = "Allows the command to stop and wait for user input or action (for example to confirm renaming)." +
                          "Also allows user to further modify the calculated new file names in a text editor."
        };

        VerboseOption = new CommandOption("-v|--verbose", CommandOptionType.NoValue)
        {
            Description = "Be verbose."
        };

        ModifyExtensionsOption = new CommandOption("--include-ext", CommandOptionType.NoValue)
        {
            Description = "Include file extension in renaming. Extensions are excluded by default."
        };

        DisableLoggingOption = new CommandOption("--no-log", CommandOptionType.NoValue)
        {
            Description = "Disables logging rename in history log."
        };

        DryRunOption = new CommandOption("--dry|--dry-run", CommandOptionType.NoValue)
        {
            Description = "Dry Run. Will not make any changes."
        };
    }

    public static CommandLineApplication AddFilterOption(this CommandLineApplication cli)
    {
        cli.AddOption(FilterOption);
        return cli;
    }

    public static CommandLineApplication AddPrependOption(this CommandLineApplication cli)
    {
        cli.AddOption(PrependOption);
        return cli;
    }

    public static CommandLineApplication AddReplaceOption(this CommandLineApplication cli)
    {
        cli.AddOption(ReplaceOption);
        return cli;
    }

    public static CommandLineApplication AddReplaceWithOption(this CommandLineApplication cli)
    {
        cli.AddOption(ReplaceWithOption);
        return cli;
    }

    public static CommandLineApplication AddTitleCaseOption(this CommandLineApplication cli)
    {
        cli.AddOption(TitleCaseOption);
        return cli;
    }

    public static CommandLineApplication AddInteractiveOption(this CommandLineApplication cli)
    {
        cli.AddOption(InteractiveOption);
        return cli;
    }

    public static CommandLineApplication AddVerboseOption(this CommandLineApplication cli)
    {
        cli.AddOption(VerboseOption);
        return cli;
    }

    public static CommandLineApplication AddModifyExtensionsOption(this CommandLineApplication cli)
    {
        cli.AddOption(ModifyExtensionsOption);
        return cli;
    }

    public static CommandLineApplication AddDisableLoggingOption(this CommandLineApplication cli)
    {
        cli.AddOption(DisableLoggingOption);
        return cli;
    }

    public static CommandLineApplication AddDryRunOption(this CommandLineApplication cli)
    {
        cli.AddOption(DryRunOption);
        return cli;
    }
}