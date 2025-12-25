module HomeLibProt.CommandLineImporter.Arguments

open Argu

type CLIArguments =
    | [<CliPrefix(CliPrefix.None)>] ImportInpx of ParseResults<ImportInpxArgs>
    | [<CliPrefix(CliPrefix.None)>] ReimportAHS of ParseResults<ReimportAHSArgs>

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | ImportInpx _ -> "Import inpx file to database."
            | ReimportAHS _ -> "Reimport author hierarchical search based on existing database."

and ImportInpxArgs =
    | [<ExactlyOnce; AltCommandLine("-i")>] PathToInpx of string
    | [<ExactlyOnce; AltCommandLine("-a")>] PathToArchives of string
    | [<ExactlyOnce; AltCommandLine("-d")>] PathToDatabase of string
    | [<AltCommandLine("-b")>] BatchSize of int
    | [<AltCommandLine("-l")>] MaxCountLeafs of int

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | PathToInpx _ -> "Path to Inpx file on local file system"
            | PathToArchives _ -> "Path to Zip archives directory on local file system"
            | PathToDatabase _ -> "Path to database on local file system"
            | BatchSize _ -> "[Optional] Max count inp records imported per time. Default: 100."
            | MaxCountLeafs _ -> "[Optional] Max count leafs for author hierarchical search. Default: 50."

and ReimportAHSArgs =
    | [<ExactlyOnce; AltCommandLine("-d")>] PathToDatabase of string
    | [<AltCommandLine("-l")>] MaxCountLeafs of int

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | PathToDatabase _ -> "Path to database on local file system"
            | MaxCountLeafs _ -> "[Optional] Max count leafs for author hierarchical search. Default: 50."
