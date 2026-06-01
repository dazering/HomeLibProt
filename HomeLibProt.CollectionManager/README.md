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
dotnet publish .\HomeLibProt.CollectionManager\HomeLibProt.CollectionManager.fsproj -o .\publish\HomeLibProt.CollectionManager\linux-x64 -r linux-x64
```

Windows

```
dotnet publish .\HomeLibProt.CollectionManager\HomeLibProt.CollectionManager.fsproj -o .\publish\HomeLibProt.CollectionManager\win-x64 -r win-x64
```

## Usage

```
SUBCOMMANDS:

    importsqldumps <options>
                          Import sql dumps to database.
    downloadsqldumps <options>
                          Download sql dumps.
    generateinpx <options>
                          Generate inpx.
    downloadbooks <options>
                          Download books.

    Use 'HomeLibProt.CollectionManager <subcommand> --help' for additional information.

OPTIONS:

    --help                display this list of options.
```

### Download SQL dumps

```
USAGE: HomeLibProt.CollectionManager downloadsqldumps [--help] --pathtosqldumps <string> --site <flibusta|librusec> --retries <uint>

OPTIONS:

    --pathtosqldumps, -i <string>
                          Path to where save sql dumps on local file system
    --site, -s <flibusta|librusec>
                          Source of sql dumps
    --retries, -r <uint>  Count of retries
    --help                display this list of options.
```

Example

```
HomeLibProt.CollectionManager.exe downloadsqldumps -i C:\BookDemo\flibusta_sql\ -s flibusta -r 10
```

### Import SQL dumps

```
USAGE: HomeLibProt.CollectionManager importsqldumps [--help] --pathtosqldumps <string> --pathtodatabase <string> --site <flibusta|librusec> [--keepsqldumps]

OPTIONS:

    --pathtosqldumps, -i <string>
                          Path to sql dumps on local file system
    --pathtodatabase, -d <string>
                          Path to database on local file system
    --site, -s <flibusta|librusec>
                          Source of sql dumps
    --keepsqldumps, -k    [Optional] If not set after import sql dumps will be deleted
    --help                display this list of options.
```

Example

```
HomeLibProt.CollectionManager.exe importsqldumps -i C:\BookDemo\flibusta_sql\ -d C:\BookDemo\sql_dump.db -s flibusta -k
```

### Generate INPX

```
USAGE: HomeLibProt.CollectionManager generateinpx [--help] --pathtolibrary <string> --pathtoinpx <string> --pathtodatabase <string>

OPTIONS:

    --pathtolibrary, -i <string>
                          Path to library archives on local file system
    --pathtoinpx, -o <string>
                          Path to where save inpx on local file system
    --pathtodatabase, -d <string>
                          Path to database on local file system
    --help                display this list of options.
```

Example

```
HomeLibProt.CollectionManager.exe generateinpx -i C:\BookDemo\Books\ -d C:\BookDemo\sql_dump.db -o C:\BookDemo\Inpx.inpx
```

### Download book archives

```
USAGE: HomeLibProt.CollectionManager downloadbooks [--help] --pathtolibrary <string> --outputpath <string> --site <flibusta|librusec> --retries <uint> --archivetypedownload <all|fb2|binary>

OPTIONS:

    --pathtolibrary, -i <string>
                          Path to library archives on local file system
    --outputpath, -o <string>
                          Path to where save downloaded archives on local file system
    --site, -s <flibusta|librusec>
                          Source of archives
    --retries, -r <uint>  Count of retries
    --archivetypedownload, -a <all|fb2|binary>
                          Type of archive to download
    --help                display this list of options.
```

Example

```
HomeLibProt.CollectionManager.exe downloadbooks -i C:\BookDemo\Books\ -o C:\BookDemo\Books\ -s flibusta -r 10 -a fb2
```
