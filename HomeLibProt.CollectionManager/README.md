# HomeLibProt.CollectionManager

Console application for managing collection

## Requirements

- .NET SDK 8.0+

## Build from source files

```
dotnet build .\HomeLibProt.CollectionManager\HomeLibProt.CollectionManager.fsproj
```

## Publish from source files

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
    version <options>     Print version.

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
USAGE: HomeLibProt.CollectionManager generateinpx [--help] --pathtolibrary <string> --pathtoinpx <string> --pathtodatabase <string> --site <flibusta|librusec> --librarytype <fb2|all>

OPTIONS:

    --pathtolibrary, -i <string>
                          Path to library archives on local file system
    --pathtoinpx, -o <string>
                          Path to where save inpx on local file system
    --pathtodatabase, -d <string>
                          Path to database on local file system
    --site, -s <flibusta|librusec>
                          Source of books
    --librarytype, -l <fb2|all>
                          Type of library
    --help                display this list of options.
```

Example

```
HomeLibProt.CollectionManager.exe generateinpx -i C:\BookDemo\Books\ -d C:\BookDemo\sql_dump.db -o C:\BookDemo\Inpx.inpx -s flibusta -l fb2
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

## Scripts

### FlibustaGetDailyUpdateFb2.ps1 and FlibustaGetDailyUpdateFb2.sh

#### Expected arguments

```
FlibustaGetDailyUpdateFb2.ps1 -InpxPath {InpxPath} -LibraryPath {LibraryPath}
```

```
FlibustaGetDailyUpdateFb2.sh {InpxPath} {LibraryPath}
```

#### Steps

1. Creates directory `Updates/Fb2` in `LibrarPath` directory
2. Creates directory `flibusta_sql_dumps` near `HomeLibProt.CollectionManager`
3. Get last book number from `LibraryPath` then downloads last fb2 archives to `Updates/Fb2` directory
4. Creating daily archive from `Updates/Fb2` in `LibraryPath` with name `{currentYear}-{currentMonth}-f.fb2-{firstBookNumber}-{lastBookNumber}.zip`
5. Downloading sql dumps from Flibusta to `flibusta_sql_dumps` directory
6. Importing sql dumps to sqlite database
7. Generating inpx from sqlite database to `flibusta_sql_dumps/flibusta_fb2_local.inpx"`
8. Copy generated inpx to `InpxPath`
9. Run `PostUpdate.(ps1|sh)` script

### FlibustaGetDailyUpdateFb2AndUsr.ps1 and FlibustaGetDailyUpdateFb2AndUsr.sh

#### Expected arguments

```
FlibustaGetDailyUpdateFb2AndUsr.ps1 -InpxPath {InpxPath} -LibraryPath {LibraryPath}
```

```
FlibustaGetDailyUpdateFb2AndUsr.sh {InpxPath} {LibraryPath}
```

#### Steps

1. Creates directory `Updates/Fb2` in `LibrarPath` directory
2. Creates directory `Updates/Binary` in `LibrarPath` directory
3. Creates directory `flibusta_sql_dumps` near `HomeLibProt.CollectionManager`
4. Get last book number from `LibraryPath` then downloads last fb2 archives to `Updates/Fb2` directory
5. Get last book number from `LibraryPath` then downloads last usr archives to `Updates/Binary` directory
6. Creating daily archive from `Updates/Fb2` in `LibraryPath` with name `{currentYear}-{currentMonth}-f.fb2-{firstBookNumber}-{lastBookNumber}.zip`
7. Creating daily archive from `Updates/Binary` in `LibraryPath` with name `{currentYear}-{currentMonth}-f.usr-{firstBookNumber}-{lastBookNumber}.zip`
8. Downloading sql dumps from Flibusta to `flibusta_sql_dumps` directory
9. Importing sql dumps to sqlite database
10. Generating inpx from sqlite database to `flibusta_sql_dumps/flibusta_all_local.inpx"`
11. Copy generated inpx to `InpxPath`
12. Run `PostUpdate.(ps1|sh)` script

### FlibustaGetMonthlyUpdateFb2.ps1 and FlibustaGetMonthlyUpdateFb2.sh

#### Expected arguments

```
FlibustaGetMonthlyUpdateFb2.ps1 -InpxPath {InpxPath} -LibraryPath {LibraryPath}
```

```
FlibustaGetMonthlyUpdateFb2.sh {InpxPath} {LibraryPath}
```

#### Steps

1. Creating monthly archive from `LibraryPath` in `LibraryPath` with name `f.fb2-{firstBookNumber}-{lastBookNumber}.zip` from archives coppesponding filter `{currentYear}-{currentMonth}-f.fb2-*.zip`
2. Downloading sql dumps from Flibusta to `flibusta_sql_dumps` directory
3. Importing sql dumps to sqlite database
4. Generating inpx from sqlite database to `flibusta_sql_dumps/flibusta_fb2_local.inpx"`
5. Copy generated inpx to `InpxPath`
6. Run `PostUpdate.(ps1|sh)` script

### FlibustaGetMonthlyUpdateFb2AndUsr.ps1 and FlibustaGetMonthlyUpdateFb2AndUsr.sh

#### Expected arguments

```
FlibustaGetMonthlyUpdateFb2AndUsr.ps1 -InpxPath {InpxPath} -LibraryPath {LibraryPath}
```

```
FlibustaGetMonthlyUpdateFb2AndUsr.sh {InpxPath} {LibraryPath}
```

#### Steps

1. Creating monthly archive from `LibraryPath` in `LibraryPath` with name `f.fb2-{firstBookNumber}-{lastBookNumber}.zip` from archives coppesponding filter `{currentYear}-{currentMonth}-f.fb2-*.zip`
2. Creating monthly archive from `LibraryPath` in `LibraryPath` with name `f.fb2-{firstBookNumber}-{lastBookNumber}.zip` from archives coppesponding filter `{currentYear}-{currentMonth}-f.usr-*.zip`
3. Downloading sql dumps from Flibusta to `flibusta_sql_dumps` directory
4. Importing sql dumps to sqlite database
5. Generating inpx from sqlite database to `flibusta_sql_dumps/flibusta_fb2_local.inpx"`
6. Copy generated inpx to `InpxPath`
7. Run `PostUpdate.(ps1|sh)` script

## PostUpdate script

It's possible to place PostUpdate script to run any command after updates library

For windows:

```
| -- HomeLibProt.CollectionManager
|    | -- Scripts
|    | -- | -- FlibustaGetDailyUpdateFb2.ps1
|    | -- | -- FlibustaGetDailyUpdateFb2AndUsr.ps1
|    | -- | -- FlibustaGetMonthlyUpdateFb2.ps1
|    | -- | -- FlibustaGetMonthlyUpdateFb2AndUsr.ps1
|    | -- HomeLibProt.CollectionManager.exe
| -- PostUpdate.ps1
```

And Linix:

```
| -- HomeLibProt.CollectionManager
|    | -- Scripts
|    | -- | -- FlibustaGetDailyUpdateFb2.sh
|    | -- | -- FlibustaGetDailyUpdateFb2AndUsr.sh
|    | -- | -- FlibustaGetMonthlyUpdateFb2.sh
|    | -- | -- FlibustaGetMonthlyUpdateFb2AndUsr.sh
|    | -- HomeLibProt.CollectionManager.exe
| -- PostUpdate.sh
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
systemctl enable --now HomeLibProt.FlibustaGetMonthlyUpdate.timer
```

```
systemctl enable --now HomeLibProt.FlibustaGetDailyUpdate.timer
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

1. Copy fb2 archives to `{pathToLibrary}/Update/Fb2` directory

2. Copy usr archives to `{pathToLibrary}/Update/Binary` directory

3. Run command

```
HomeLibProt.CollectionManager.exe  mergebooks -i {pathToUpdates} -o {pathToLibrary} -s 10000 -p "{currentYear}-{currentMonth}-f.fb2-" -f "*.zip"
```

Example

```
HomeLibProt.CollectionManager.exe  mergebooks -i C:\BookDemo\Books\Update\Fb2 -o C:\BookDemo\Books\ -s 10000 -p "26-06-f.fb2-" -f "*.zip"
```

4. Run command

```
HomeLibProt.CollectionManager.exe  mergebooks -i {pathToUpdates} -o {pathToLibrary} -s 10000 -p "{currentYear}-{currentMonth}-f.usr-" -f "*.zip"
```

Example

```
HomeLibProt.CollectionManager.exe  mergebooks -i C:\BookDemo\Books\Update\Binary -o C:\BookDemo\Books\ -s 10000 -p "26-06-f.usr-" -f "*.zip"
```

## Logs

```
| -- logs
| -- | errors-{yyyyMMdd}.txt
| -- | fatal-{yyyyMMdd}.txt
| -- | information-{yyyyMMdd}.txt
| -- Scripts
|    | -- ....
| -- HomeLibProt.CollectionManager.exe
```

Logs are writing to the directory logs near `HomeLibProt.CollectionManager.exe`

`errors-{yyyyMMdd}.txt` contains error messages not fatal for application

`fatal-{yyyyMMdd}.txt` contains error messages fatal for application

`information-{yyyyMMdd}.txt` contains verbose and fatal messages