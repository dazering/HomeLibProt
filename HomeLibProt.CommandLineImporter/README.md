# HomeLibProt.CommandLineImporter

Console application for importing inpx to sqlite database

## Requirements

- .NET SDK 8.0+

## Build

```
dotnet build .\HomeLibProt.CommandLineImporter\HomeLibProt.CommandLineImporter.fsproj
```

## Publish

Linux

```
dotnet publish .\HomeLibProt.CommandLineImporter\HomeLibProt.CommandLineImporter.fsproj -o .\HomeLibProt.CommandLineImporter\publish\linux-x64 -r linux-x64
```

Windows

```
dotnet publish .\HomeLibProt.CommandLineImporter\HomeLibProt.CommandLineImporter.fsproj -o .\HomeLibProt.CommandLineImporter\publish\win-x64 -r win-x64
```

## Usage

```
SUBCOMMANDS:

    importinpx <options>  Import inpx file to database.
    reimportahs <options> Reimport author hierarchical search based on existing database.

    Use 'HomeLibProt.CommandLineImporter <subcommand> --help' for additional information.

OPTIONS:

    --help                display this list of options.
```

### Import inpx

```
USAGE: HomeLibProt.CommandLineImporter importinpx [--help] --pathtoinpx <string> --pathtodatabase <string> [--batchsize <int>] [--maxcountleafs <int>]

OPTIONS:

    --pathtoinpx, -i <string>
                          Path to Inpx file on local file system
    --pathtodatabase, -d <string>
                          Path to database on local file system
    --batchsize, -b <int> [Optional] Max count inp records imported per time. Default: 100.
    --maxcountleafs, -l <int>
                          [Optional] Max count leafs for author hierarchical search. Default: 50.
    --help                display this list of options.
```

### Reimport author hierarchical search

```
USAGE: HomeLibProt.CommandLineImporter reimportahs [--help] --pathtodatabase <string> [--maxcountleafs <int>]

OPTIONS:

    --pathtodatabase, -d <string>
                          Path to database on local file system
    --maxcountleafs, -l <int>
                          [Optional] Max count leafs for author hierarchical search. Default: 50.
    --help                display this list of options.
```

## Comparison of importers

Tested with inpx contains 1M lines, 100 inp files, 1M authors, genres, series and keywords. Every 10th line contains 10 authors, genres, series and keywords from previous one.

|                           App                           |  SSD  | HDD   | Memory usage |
| :-----------------------------------------------------: | :---: | ----- | ------------ |
|             HomeLibProt.CommandLineImporter             | 1:20  | 2:37  | 25 MB        |
|    [Inpx-web](https://github.com/bookpauk/inpx-web)     | 1:18  | 3:35  | 1095.98 MB   |
| [MyHomeLib](https://github.com/OleksiyPenkov/MyHomeLib) | ~3;45 | ~3:57 | 19 MB        |