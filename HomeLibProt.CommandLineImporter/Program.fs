module HomeLibProt.CommandLineImporter.Program

open Argu
open Serilog
open System.Diagnostics
open System.Threading.Tasks

open HomeLibProt.CollectionImporter
open HomeLibProt.CommandLineImporter.Arguments
open HomeLibProt.Common.Logger
open HomeLibProt.Domain.DataAccess

let parser = ArgumentParser.Create<CLIArguments>()

let batchSizeDefault = 100
let maxCountLeafsDefault = 50

let printProgressReport (logger: ILogger) (message: string) : unit = logger.Information message

let importInpxAsync (logger: ILogger) (args: ParseResults<ImportInpxArgs>) : Task<unit> =
    task {
        let pathToInpx = args.GetResult ImportInpxArgs.PathToInpx
        let pathToArchives = args.GetResult ImportInpxArgs.PathToArchives
        let pathToDb = args.GetResult ImportInpxArgs.PathToDatabase
        let fullCreation = args.Contains ImportInpxArgs.FullCreation

        let batchSize =
            args.TryGetResult ImportInpxArgs.BatchSize
            |> Option.defaultValue batchSizeDefault

        let maxCountLeafs =
            args.TryGetResult ImportInpxArgs.MaxCountLeafs
            |> Option.defaultValue maxCountLeafsDefault

        let connection =
            pathToDb
            |> ConnectionUtils.MakeConnectionString
            |> ConnectionUtils.MakeConnection

        let parameters: CollectionImporter.ImportInpxParameters =
            { PathToInpx = pathToInpx
              PathToArchives = pathToArchives
              BatchSize = batchSize
              MaxCountLeafs = maxCountLeafs
              FullCreation = fullCreation
              ProgressReport = printProgressReport logger
              DoInTransactionAsync = ConnectionUtils.DoInTransactionAsync }

        do! ConnectionUtils.WithConnectionAsync(connection, CollectionImporter.importCollectionToDb parameters)
    }

let reimportAHSAsync (logger: ILogger) (args: ParseResults<ReimportAHSArgs>) : Task<unit> =
    task {
        let pathToDb = args.GetResult ReimportAHSArgs.PathToDatabase

        let maxCountLeafs =
            args.TryGetResult ReimportAHSArgs.MaxCountLeafs
            |> Option.defaultValue maxCountLeafsDefault

        let connection =
            pathToDb
            |> ConnectionUtils.MakeConnectionString
            |> ConnectionUtils.MakeConnection

        let parameters: CollectionImporter.ReimportAHSParameters =
            { MaxCountLeafs = maxCountLeafs
              ProgressReport = printProgressReport logger
              DoInTransactionAsync = ConnectionUtils.DoInTransactionAsync }

        do!
            ConnectionUtils.WithConnectionAsync(
                connection,
                CollectionImporter.reimportAuthorHierarchicalSearch parameters
            )
    }

let runAsync (logger: ILogger) (arguments: ParseResults<CLIArguments>) : Task<unit> =
    task {
        match arguments.GetSubCommand() with
        | ImportInpx args -> do! importInpxAsync logger args
        | ReimportAHS args -> do! reimportAHSAsync logger args
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
