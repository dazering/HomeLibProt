module HomeLibProt.CollectionManager.Arguments

open Argu

type Site =
    | Flibusta = 1
    | Librusec = 2

type CLIArguments =
    | [<CliPrefix(CliPrefix.None)>] ImportSqlDumps of ParseResults<ImportSqlDumps>

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | ImportSqlDumps _ -> "Import inpx file to database."

and ImportSqlDumps =
    | [<ExactlyOnce; AltCommandLine("-i")>] PathToSqlDumps of string
    | [<ExactlyOnce; AltCommandLine("-d")>] PathToDatabase of string
    | [<ExactlyOnce; AltCommandLine("-s")>] Site of Site
    | [<AltCommandLine("-k")>] KeepSqlDumps

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | PathToSqlDumps _ -> "Path to where save sql dumps on local file system"
            | PathToDatabase _ -> "Path to database on local file system"
            | Site _ -> "Source of sql dumps"
            | KeepSqlDumps -> "[Optional] If not set after import sql dumps will be deleted"
