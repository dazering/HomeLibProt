module HomeLibProt.CommandLineImporter.Program

open Argu
open System.Diagnostics
open System.Threading.Tasks

open HomeLibProt.CollectionImporter
open HomeLibProt.CommandLineImporter.Arguments
open HomeLibProt.Domain.DataAccess

let parser = ArgumentParser.Create<CLIArguments>()

let batchSizeDefault = 100
let maxCountLeafsDefault = 50

let printProgressReport (message: string) : unit = printfn $"{message}"

let importInpxAsync (args: ParseResults<ImportInpxArgs>) : Task<unit> =
    task {
        let pathToInpx = args.GetResult ImportInpxArgs.PathToInpx
        let pathToDb = args.GetResult ImportInpxArgs.PathToDatabase

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
              BatchSize = batchSize
              MaxCountLeafs = maxCountLeafs
              ProgressReport = printProgressReport
              DoInTransactionAsync = ConnectionUtils.DoInTransactionAsync }

        do! ConnectionUtils.WithConnectionAsync(connection, CollectionImporter.importCollectionToDb parameters)
    }

let reimportAHSAsync (args: ParseResults<ReimportAHSArgs>) : Task<unit> =
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
              ProgressReport = printProgressReport
              DoInTransactionAsync = ConnectionUtils.DoInTransactionAsync }

        do!
            ConnectionUtils.WithConnectionAsync(
                connection,
                CollectionImporter.reimportAuthorHierarchicalSearch parameters
            )
    }

let runAsync (arguments: ParseResults<CLIArguments>) : Task<unit> =
    task {
        match arguments.GetSubCommand() with
        | ImportInpx args -> do! importInpxAsync args
        | ReimportAHS args -> do! reimportAHSAsync args
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
