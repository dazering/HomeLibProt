module HomeLibProt.CollectionManager.Program

open Argu
open System.Diagnostics
open System.Net.Http
open System.Threading.Tasks

open HomeLibProt.CollectionManager.Arguments
open HomeLibProt.CollectionManager.Books
open HomeLibProt.CollectionManager.SqlDumps
open HomeLibProt.Domain.DataAccess

let parser = ArgumentParser.Create<CLIArguments>()

let printProgressReport (message: string) : unit = printfn $"{message}"

let downloadBooks (args: ParseResults<DownloadBooks>) : Task<unit> =
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
              ProgressReport = printProgressReport }

        match site with
        | Site.Flibusta -> do! BooksDownloader.downloadFlibustaArchives parameters
        | Site.Librusec -> failwith $"Unsupported archive source: {site}"
        | _ -> failwith $"Unknown archive source: {site}"
    }

let generateInpx (args: ParseResults<GenerateInpx>) : Task<unit> =
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
              ProgressReport = printProgressReport
              DoInTransactionAsync = ConnectionUtils.DoInTransactionAsync }

        do! ConnectionUtils.WithConnectionAsync(connection, Inpx.InpxGenerator.generateInpxAsync parameters)
    }

let downloadSqlDumps (args: ParseResults<DownloadSqlDumps>) : Task<unit> =
    task {
        let pathToSqlDumps = args.GetResult DownloadSqlDumps.PathToSqlDumps
        let site = args.GetResult DownloadSqlDumps.Site
        let retries = args.GetResult DownloadSqlDumps.Retries

        use httpClient = new HttpClient()

        let parameters: SqlDumpsDownloader.SqlDumpDownloaderParameters =
            { PathToSqlDumps = pathToSqlDumps
              HttpClient = httpClient
              Retries = retries
              ProgressReport = printProgressReport }

        match site with
        | Site.Flibusta -> do! SqlDumpsDownloader.downloadSqlDumpsFlibustaAsync parameters
        | Site.Librusec -> do! SqlDumpsDownloader.downloadSqlDumpsLibrusecAsync parameters
        | _ -> failwith $"Unknown sql dump source: {site}"

    }

let importSqlDumps (args: ParseResults<ImportSqlDumps>) : Task<unit> =
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
              ProgressReport = printProgressReport
              DoInTransactionAsync = ConnectionUtils.DoInTransactionAsync }

        match site with
        | Site.Flibusta ->
            do! ConnectionUtils.WithConnectionAsync(connection, SqlDumpImporter.importSqlDumpsFlibustaAsync parameters)
        | Site.Librusec ->
            do! ConnectionUtils.WithConnectionAsync(connection, SqlDumpImporter.importSqlDumpsLibrusecAsync parameters)
        | _ -> failwith $"Unknown sql dump source: {site}"

    }

let runAsync (arguments: ParseResults<CLIArguments>) : Task<unit> =
    task {
        match arguments.GetSubCommand() with
        | ImportSqlDumps args -> do! importSqlDumps args
        | DownloadSqlDumps args -> do! downloadSqlDumps args
        | GenerateInpx args -> do! generateInpx args
        | DownloadBooks args -> do! downloadBooks args
    }

let withStopwatchAsync (stopwatch: Stopwatch) (actionAsync: unit -> Task<unit>) : Task<Stopwatch> =
    task {
        stopwatch.Start()
        do! actionAsync ()
        stopwatch.Stop()
        return stopwatch
    }

let printElapsedTime (stopwatch: Stopwatch) : unit =
    printfn $"Elapsed time: {stopwatch.Elapsed.Minutes:d2}:{stopwatch.Elapsed.Seconds:d2}"

[<EntryPoint>]
let main args =
    try
        let arguments = parser.ParseCommandLine args

        (withStopwatchAsync (new Stopwatch()) (fun _ -> task { do! runAsync arguments }))
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult()
        |> printElapsedTime

        0

    with :? ArguParseException as e ->
        eprintfn "%s" e.Message
        parser.PrintUsage() |> ignore

        1
