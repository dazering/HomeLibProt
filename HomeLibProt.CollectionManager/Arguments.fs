module HomeLibProt.CollectionManager.Arguments

open Argu

type Site =
    | Flibusta = 1
    | Librusec = 2

type ArchiveTypeDownload =
    | All
    | Fb2
    | Binary

type CLIArguments =
    | [<CliPrefix(CliPrefix.None)>] ImportSqlDumps of ParseResults<ImportSqlDumps>
    | [<CliPrefix(CliPrefix.None)>] DownloadSqlDumps of ParseResults<DownloadSqlDumps>
    | [<CliPrefix(CliPrefix.None)>] GenerateInpx of ParseResults<GenerateInpx>
    | [<CliPrefix(CliPrefix.None)>] DownloadBooks of ParseResults<DownloadBooks>
    | [<CliPrefix(CliPrefix.None)>] MergeBooks of ParseResults<MergeBooks>

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | ImportSqlDumps _ -> "Import sql dumps to database."
            | DownloadSqlDumps _ -> "Download sql dumps."
            | GenerateInpx _ -> "Generate inpx."
            | DownloadBooks _ -> "Download books."
            | MergeBooks _ -> "Merge book archives."

and ImportSqlDumps =
    | [<ExactlyOnce; AltCommandLine("-i")>] PathToSqlDumps of string
    | [<ExactlyOnce; AltCommandLine("-d")>] PathToDatabase of string
    | [<ExactlyOnce; AltCommandLine("-s")>] Site of Site
    | [<AltCommandLine("-k")>] KeepSqlDumps

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | PathToSqlDumps _ -> "Path to sql dumps on local file system"
            | PathToDatabase _ -> "Path to database on local file system"
            | Site _ -> "Source of sql dumps"
            | KeepSqlDumps -> "[Optional] If not set after import sql dumps will be deleted"

and DownloadSqlDumps =
    | [<ExactlyOnce; AltCommandLine("-i")>] PathToSqlDumps of string
    | [<ExactlyOnce; AltCommandLine("-s")>] Site of Site
    | [<ExactlyOnce; AltCommandLine("-r")>] Retries of uint

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | PathToSqlDumps _ -> "Path to where save sql dumps on local file system"
            | Site _ -> "Source of sql dumps"
            | Retries _ -> "Count of retries"

and GenerateInpx =
    | [<ExactlyOnce; AltCommandLine("-i")>] PathToLibrary of string
    | [<ExactlyOnce; AltCommandLine("-o")>] PathToInpx of string
    | [<ExactlyOnce; AltCommandLine("-d")>] PathToDatabase of string


    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | PathToLibrary _ -> "Path to library archives on local file system"
            | PathToInpx _ -> "Path to where save inpx on local file system"
            | PathToDatabase _ -> "Path to database on local file system"

and DownloadBooks =
    | [<ExactlyOnce; AltCommandLine("-i")>] PathToLibrary of string
    | [<ExactlyOnce; AltCommandLine("-o")>] OutputPath of string
    | [<ExactlyOnce; AltCommandLine("-s")>] Site of Site
    | [<ExactlyOnce; AltCommandLine("-r")>] Retries of uint
    | [<ExactlyOnce; AltCommandLine("-a")>] ArchiveTypeDownload of ArchiveTypeDownload

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | PathToLibrary _ -> "Path to library archives on local file system"
            | OutputPath _ -> "Path to where save downloaded archives on local file system"
            | Site _ -> "Source of archives"
            | Retries _ -> "Count of retries"
            | ArchiveTypeDownload _ -> "Type of archive to download"

and MergeBooks =
    | [<ExactlyOnce; AltCommandLine("-i")>] PathToLibrary of string
    | [<ExactlyOnce; AltCommandLine("-o")>] OutputPath of string
    | [<ExactlyOnce; AltCommandLine("-s")>] ArchiveSize of int
    | [<ExactlyOnce; AltCommandLine("-f")>] ArchiveFilter of string
    | [<AltCommandLine("-p")>] Prefix of string
    | [<AltCommandLine("-k")>] KeepOldArchives

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | PathToLibrary _ -> "Path to library archives on local file system"
            | OutputPath _ -> "Path to where save new archives on local file system"
            | ArchiveSize _ -> "Size of new archives"
            | ArchiveFilter _ -> "Filter library archives"
            | Prefix _ -> "[Optional] Prefix of new archives"
            | KeepOldArchives -> "[Optional] If not set after copying old archives will be deleted"
