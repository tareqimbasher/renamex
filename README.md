# renamex
A bulk file renaming utility.

**(Still under development, more features are coming!)**

## Features
* Prepend text
* Replace one or more strings of text
* Title case
* Keeps track of rename operations
* Ability to undo renaming
* Interactive mode

## Usage

```cli
renamex <DIR>
        [--filter <SEARCHPATTERN>] 
        [--prepend <TXT>]
        [--replace <TXT>]
        [--replace-with <TXT>]
        [--title-case]
        [--include-ext]
        [--interactive]
        [--verbose]
        [--no-log]
        [--dry-run]
```


## Commands

### History
```cli
renamex history <DIR>
```

Prints the history log of the specified directory.


### Undo
```cli
renamex undo <DIR>
        [--interactive]
        [--verbose]
        [--no-log]
        [--dry-run]
```

Undo the last renaming changes to the specified directory, reverting file names as they were before the last renaming that took place. Unless the `--no-log` option is set, a new entry will be added to the history log for this revert of file names. This log entry also means that this "undo" can also be "undone."


## Options

#### Directory
`<DIR>`

The directory that contains the files to rename. If left unspecified, will use the current working directory.

#### Filtering
`--filter | -f <SEARCHPATTERN>`

Filter files using a wildcard expression. Only matching files will be renamed. The search string can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.

Examples of wildcard search patterns:
* `*.jpg`: selects files ending with the `.jpg` extension
* `*december*`: selects files containing the word `december` in the name
* `notes.txt`: selects a file with the name `notes.txt`
* `Report - Apr 202?.xlsx`: selects files that start with `Report - Apr 202` and then match any character (`?`) and end with `.xlsx`


#### Prepending Text
`--prepend | -p <TXT>`

Prepends the specified text to the beginning of each file name. If a file already starts with the specified text, the text will not be prepended again.

#### Replacing Text
`--replace | -r <TXT>` and `--replace-with | -rw <TXT>`

Replaces all instances of the specified text with another. These 2 options must be used together, you cannot use one option without specifying the other.

You can specify the `--replace` option multiple times to replace multiple strings of text with the same `--replace-with` text.

#### Casing
`--title-case | -t`

Capitalizes the first character of every word in the name. A new word is identified if it is preceded with a space or if it is the first word in the name.

#### File Extensions
`--include-ext`

If this option is used then file extensions will be included in the renaming procedure. If not, extensions will remain unchanged.

#### Interactive Mode
`--interactive`

Prompts for confirmation before renaming and opens a text editor with the planned name changes for the user to edit. Each file name, and its planned, new, name will be displayed on a separate line.

* If user changes a name, the new name specified by the user will be used.
* If user removes a line, that file will be skipped and will not be renamed.

#### Verbose
`--verbose`

Displays diagnostics information.

#### Disable History Logging
`--no-log`

By default when a rename operation occurs, a log entry will be added to the history of the folder (`<DIR>`) with information about the changes and provides a chance to review or undo those changes. This option will prohibit the writing of that log to history.

#### Dry Run
`--dry-run | --dry`

If used, the app will not rename any files. Use with the `--verbose` option to see what the app would do in a real run.