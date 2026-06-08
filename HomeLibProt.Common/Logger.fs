module HomeLibProt.Common.Logger

open Serilog
open Serilog.Core
open System
open System.IO
open System.Threading.Tasks

let private getAssemblyDirectory () : string =
    Path.GetDirectoryName Environment.ProcessPath

let private getLogPath (fileName: string) =
    Path.Combine(getAssemblyDirectory (), "logs", fileName)

let private setMinimumLevelVerbose (loggerConfiguration: LoggerConfiguration) : LoggerConfiguration =
    loggerConfiguration.MinimumLevel.Verbose()

let private writeFatalToFile (loggerConfiguration: LoggerConfiguration) : LoggerConfiguration =
    loggerConfiguration.WriteTo.Logger(fun l ->
        l.Filter
            .ByIncludingOnly(fun e -> e.Level = Events.LogEventLevel.Fatal)
            .WriteTo.File(path = getLogPath "fatal-.txt", rollingInterval = RollingInterval.Day)
        |> ignore)

let private writeFatalAndInformationToConsoleAndFile (loggerConfiguration: LoggerConfiguration) : LoggerConfiguration =
    loggerConfiguration.WriteTo.Logger(fun l ->
        l.Filter
            .ByIncludingOnly(fun e ->
                e.Level = Events.LogEventLevel.Fatal
                || e.Level = Events.LogEventLevel.Information)
            .WriteTo.File(path = getLogPath "information-.txt", rollingInterval = RollingInterval.Day)
            .WriteTo.Console()
        |> ignore)

let private writeErrorsToConsoleAndFile (loggerConfiguration: LoggerConfiguration) : LoggerConfiguration =
    loggerConfiguration.WriteTo.Logger(fun l ->
        l.Filter
            .ByIncludingOnly(fun e -> e.Level = Events.LogEventLevel.Error)
            .WriteTo.File(path = getLogPath "errors-.txt", rollingInterval = RollingInterval.Day)
            .WriteTo.Console()
        |> ignore)

let getConsoleAppLogger () : Logger =
    (new LoggerConfiguration()
     |> setMinimumLevelVerbose
     |> writeFatalToFile
     |> writeFatalAndInformationToConsoleAndFile
     |> writeErrorsToConsoleAndFile)
        .CreateLogger()

let doWithLoggerAsync (logger: ILogger) (action: ILogger -> Task) : Task<int> =
    task {
        try
            do! action logger
            return 0
        with ex ->
            logger.Fatal(ex, "Fatal error")
            return 1
    }
