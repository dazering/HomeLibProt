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
    mergebooks <options>  Merge book archives.

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

### Merge book archives

```
USAGE: HomeLibProt.CollectionManager mergebooks [--help] --pathtolibrary <string> --outputpath <string> --archivesize <int> --archivefilter <string> [--prefix <string>] [--keepoldarchives]

OPTIONS:

    --pathtolibrary, -i <string>
                          Path to library archives on local file system
    --outputpath, -o <string>
                          Path to where save new archives on local file system
    --archivesize, -s <int>
                          Size of new archives
    --archivefilter, -f <string>
                          Filter library archives
    --prefix, -p <string> [Optional] Prefix of new archives
    --keepoldarchives, -k [Optional] If not set after copying old archives will be deleted
    --help                display this list of options.
```

Example

```
HomeLibProt.CollectionManager.exe  mergebooks -i C:\BookDemo\Books\ -o C:\BookDemo\Books\ -s 10000 -p "f.fb2-" -f "*.zip" -k
```

## Automation

### Linux - Systemd

See [example files](./SystemdExample)

1. Place service and timer files to `/etc/systemd/system/`

2. Use corresponding paths in scripts and arguments

3. Run commands:

```
systemctl daemon-reload
```

```
systemctl enable --now HomeLibProt.FlibustaGetMonthlyUpdateFb2AndUsr.timer
```

```
systemctl enable --now HomeLibProt.FlibustaGetDailyUpdateFb2AndUsr.timer
```

4. Check if timers appear in scheduled list

```
systemctl list-timers
```

### Windows

1. Run in powershell command

```
schtasks /create /tn 'HomeLibProt.GetDailyUpdate' /tr "powershell -WindowStyle Hidden -NonInteractive -ExecutionPolicy Bypass C:\BookDemo\Scripts\FlibustaGetDailyUpdateFb2AndUsr.ps1 -InpxPath C:\BookDemo\Books\Flibusta_all_local.inpx -LibraryPath C:\BookDemo\Books\" /sc daily /st 06:00
```

2. Run in powershell command

```
schtasks /create /tn 'HomeLibProt.GetMonthlyUpdate' /tr "powershell -WindowStyle Hidden -NonInteractive -ExecutionPolicy Bypass C:\BookDemo\Scripts\FlibustaGetMonthlyUpdateFb2AndUsr.ps1 -InpxPath C:\BookDemo\Books\Flibusta_all_local.inpx -LibraryPath C:\BookDemo\Books\" /sc monthly /d 1 /st 07:00
```

3. Open Task Scheduler

4. For every task `Properties > Settings` check in `Run task as soon as possible after a scheduled start is missed`

## Migration

1. Copy fb2 archives to `C:\BookDemo\Books\Updates\Fb2`

2. Copy usr archives to `C:\BookDemo\Books\Updates\Binary`

3. Run command

```
C:\BookDemo\HomeLibProt.CollectionManager.exe  mergebooks -i {pathToUpdates} -o {pathToLibrary} -s 10000 -p "{year}-{month}-f.fb2-" -f "*.zip"
```

Example

```
C:\BookDemo\HomeLibProt.CollectionManager.exe  mergebooks -i C:\BookDemo\Books\Update\Fb2 -o C:\BookDemo\Books\ -s 10000 -p "26-06-f.fb2-" -f "*.zip"
```

4. Run command

```
C:\BookDemo\HomeLibProt.CollectionManager.exe  mergebooks -i {pathToUpdates} -o {pathToLibrary} -s 10000 -p "{year}-{month}-f.usr-" -f "*.zip"
```

Example

```
C:\BookDemo\HomeLibProt.CollectionManager.exe  mergebooks -i C:\BookDemo\Books\Update\Binary -o C:\BookDemo\Books\ -s 10000 -p "26-06-f.usr-" -f "*.zip"
```
