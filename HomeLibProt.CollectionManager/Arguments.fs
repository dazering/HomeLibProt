module HomeLibProt.CollectionManager.Arguments

open Argu

type Site =
    | Flibusta = 1
    | Librusec = 2

type CLIArguments =
    | [<CliPrefix(CliPrefix.None)>] ImportSqlDumps of ParseResults<ImportSqlDumps>
    | [<CliPrefix(CliPrefix.None)>] DownloadSqlDumps of ParseResults<DownloadSqlDumps>
    | [<CliPrefix(CliPrefix.None)>] GenerateInpx of ParseResults<GenerateInpx>

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | ImportSqlDumps _ -> "Import sql dumps to database."
            | DownloadSqlDumps _ -> "Download sql dumps."
            | GenerateInpx _ -> "Generate inpx."

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
