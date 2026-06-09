module HomeLibProt.CollectionManager.Program

open Argu
open Serilog
open System
open System.Diagnostics
open System.Net.Http
open System.Threading.Tasks

open HomeLibProt.CollectionManager.Arguments
open HomeLibProt.CollectionManager.Books
open HomeLibProt.CollectionManager.SqlDumps
open HomeLibProt.Common.Logger
open HomeLibProt.Domain.DataAccess

let parser = ArgumentParser.Create<CLIArguments>()

let printProgressReport (logger: ILogger) (message: string) : unit = logger.Information message
let printErrorReport (logger: ILogger) (message: string) : unit = logger.Error message

let mergeBooks (logger: ILogger) (args: ParseResults<MergeBooks>) : Task<unit> =
    task {
        let pathToLibrary = args.GetResult MergeBooks.PathToLibrary
        let outputPath = args.GetResult MergeBooks.OutputPath
        let archiveFilter = args.GetResult MergeBooks.ArchiveFilter
        let archiveSize = args.GetResult MergeBooks.ArchiveSize

        let prefix =
            args.TryGetResult MergeBooks.Prefix |> Option.defaultValue System.String.Empty

        let keepOldArchives = args.Contains MergeBooks.KeepOldArchives

        let parameters: BooksMerger.BooksMergerParameters =
            { PathToLibrary = pathToLibrary
              OutputPath = outputPath
              ArchiveSize = archiveSize
              ArchiveFilter = archiveFilter
              Prefix = prefix
              KeepArchives = keepOldArchives
              ProgressReport = printProgressReport logger }

        do! BooksMerger.megreBooksAsync parameters
    }

let downloadBooks (logger: ILogger) (args: ParseResults<DownloadBooks>) : Task<unit> =
    task {
        let pathToLibrary = args.GetResult DownloadBooks.PathToLibrary
        let outputPath = args.GetResult DownloadBooks.OutputPath
        let site = args.GetResult DownloadBooks.Site
        let retries = args.GetResult DownloadBooks.Retries
        let archiveTypeDownload = args.GetResult DownloadBooks.ArchiveTypeDownload

        use httpClient = new HttpClient()

        let aTD =
            match archiveTypeDownload with
            | All -> BooksDownloader.ArchiveTypeDownload.All
            | Fb2 -> BooksDownloader.ArchiveTypeDownload.Fb2
            | Binary -> BooksDownloader.ArchiveTypeDownload.Binary

        let parameters: BooksDownloader.BooksDownloaderParameters =
            { PathToLibrary = pathToLibrary
              OutputPath = outputPath
              HttpClient = httpClient
              Retries = retries
              ArchiveTypeDownload = aTD
              ProgressReport = printProgressReport logger }

        match site with
        | Site.Flibusta -> do! BooksDownloader.downloadFlibustaArchives parameters
        | Site.Librusec -> raise (NotImplementedException $"Unsupported archive source: {site}")
        | _ -> raise (InvalidOperationException $"Unknown archive source: {site}")
    }

let generateInpx (logger: ILogger) (args: ParseResults<GenerateInpx>) : Task<unit> =
    task {
        let pathToDb = args.GetResult GenerateInpx.PathToDatabase
        let pathToLibrary = args.GetResult GenerateInpx.PathToLibrary
        let pathToInpx = args.GetResult GenerateInpx.PathToInpx

        let connection =
            pathToDb
            |> ConnectionUtils.MakeConnectionString
            |> ConnectionUtils.MakeConnection

        let parameters: Inpx.InpxGenerator.InpxGeneratorParameters =
            { PathToLibrary = pathToLibrary
              PathToInpx = pathToInpx
              ProgressReport = printProgressReport logger
              ErrorReport = printErrorReport logger
              DoInTransactionAsync = ConnectionUtils.DoInTransactionAsync }

        do! ConnectionUtils.WithConnectionAsync(connection, Inpx.InpxGenerator.generateInpxAsync parameters)
    }

let downloadSqlDumps (logger: ILogger) (args: ParseResults<DownloadSqlDumps>) : Task<unit> =
    task {
        let pathToSqlDumps = args.GetResult DownloadSqlDumps.PathToSqlDumps
        let site = args.GetResult DownloadSqlDumps.Site
        let retries = args.GetResult DownloadSqlDumps.Retries

        use httpClient = new HttpClient()

        let parameters: SqlDumpsDownloader.SqlDumpDownloaderParameters =
            { PathToSqlDumps = pathToSqlDumps
              HttpClient = httpClient
              Retries = retries
              ProgressReport = printProgressReport logger }

        match site with
        | Site.Flibusta -> do! SqlDumpsDownloader.downloadSqlDumpsFlibustaAsync parameters
        | Site.Librusec -> do! SqlDumpsDownloader.downloadSqlDumpsLibrusecAsync parameters
        | _ -> raise (InvalidOperationException $"Unknown sql dump source: {site}")

    }

let importSqlDumps (logger: ILogger) (args: ParseResults<ImportSqlDumps>) : Task<unit> =
    task {
        let pathToDb = args.GetResult ImportSqlDumps.PathToDatabase
        let pathToSqlDumps = args.GetResult ImportSqlDumps.PathToSqlDumps
        let site = args.GetResult ImportSqlDumps.Site
        let keepSqlDumps = args.Contains ImportSqlDumps.KeepSqlDumps

        let connection =
            pathToDb
            |> ConnectionUtils.MakeConnectionString
            |> ConnectionUtils.MakeConnection

        let parameters: SqlDumpImporter.SqlDumpImporterParameters =
            { PathToSqlDumps = pathToSqlDumps
              KeepSqlDumps = keepSqlDumps
              ProgressReport = printProgressReport logger
              ErrorReport = printErrorReport logger
              DoInTransactionAsync = ConnectionUtils.DoInTransactionAsync }

        match site with
        | Site.Flibusta ->
            do! ConnectionUtils.WithConnectionAsync(connection, SqlDumpImporter.importSqlDumpsFlibustaAsync parameters)
        | Site.Librusec ->
            do! ConnectionUtils.WithConnectionAsync(connection, SqlDumpImporter.importSqlDumpsLibrusecAsync parameters)
        | _ -> raise (InvalidOperationException $"Unknown sql dump source: {site}")

    }

let runAsync (logger: ILogger) (arguments: ParseResults<CLIArguments>) : Task<unit> =
    task {
        match arguments.GetSubCommand() with
        | ImportSqlDumps args -> do! importSqlDumps logger args
        | DownloadSqlDumps args -> do! downloadSqlDumps logger args
        | GenerateInpx args -> do! generateInpx logger args
        | DownloadBooks args -> do! downloadBooks logger args
        | MergeBooks args -> do! mergeBooks logger args
        | Version _ ->
            printProgressReport logger $"Version: {Reflection.Assembly.GetExecutingAssembly().GetName().Version}"
    }

let withStopwatchAsync (stopwatch: Stopwatch) (actionAsync: unit -> Task<unit>) : Task<Stopwatch> =
    task {
        stopwatch.Start()
        do! actionAsync ()
        stopwatch.Stop()
        return stopwatch
    }

let printElapsedTime (logger: ILogger) (stopwatch: Stopwatch) : unit =
    logger.Information $"Elapsed time: {stopwatch.Elapsed.TotalMinutes |> int:d2}:{stopwatch.Elapsed.Seconds:d2}"

let doWithArgumentsAsync (logger: ILogger) (args: string array) : Task<int> =
    task {
        try
            let arguments = parser.ParseCommandLine args

            let! stopwatch = withStopwatchAsync (new Stopwatch()) (fun _ -> task { do! runAsync logger arguments })
            stopwatch |> printElapsedTime logger

            return 0
        with :? ArguParseException as e ->
            logger.Fatal(e, e.Message)
            return 1
    }

[<EntryPoint>]
let main args =
    use logger = getConsoleAppLogger ()

    (doWithLoggerAsync logger (fun l -> doWithArgumentsAsync l args)).ConfigureAwait(false).GetAwaiter().GetResult()
