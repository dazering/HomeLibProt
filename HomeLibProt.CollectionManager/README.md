# HomeLibProt.CollectionManager

Console application for managing collection

## Requirements

- .NET SDK 8.0+

## Build

```
dotnet build .\HomeLibProt.CollectionManager\HomeLibProt.CollectionManager.fsproj
```

## Publish

Linux

```
dotnet publish .\HomeLibProt.CollectionManager\HomeLibProt.CollectionManager.fsproj -o .\HomeLibProt.CollectionManager\publish\linux-x64 -r linux-x64
```

Windows

```
dotnet publish .\HomeLibProt.CollectionManager\HomeLibProt.CollectionManager.fsproj -o .\HomeLibProt.CollectionManager\publish\win-x64 -r win-x64
```

## Usage

```
SUBCOMMANDS:

    importsqldumps <options>
                          Import inpx file to database.

    Use 'HomeLibProt.CollectionManager <subcommand> --help' for additional information.

OPTIONS:

    --help                display this list of options.
```

### Import SQL dumps

```
USAGE: HomeLibProt.CollectionManager importsqldumps [--help] --pathtosqldumps <string> --pathtodatabase <string> --site <flibusta>

OPTIONS:

    --pathtosqldumps, -i <string>
                          Path to where save sql dumps on local file system
    --pathtodatabase, -d <string>
                          Path to database on local file system
    --site, -s <flibusta> Source of sql dumps
    --help                display this list of options.
```

Example

```
HomeLibProt.CollectionManager.exe importsqldumps -i C:\BookDemo\flibusta_sql\ -d C:\BookDemo\sql_dump.db -s flibusta
```
