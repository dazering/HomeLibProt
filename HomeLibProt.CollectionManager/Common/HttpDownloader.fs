module HomeLibProt.CollectionManager.Common.HttpDownloader

open System
open System.IO
open System.Net.Http
open System.Threading.Tasks
open System.Net.Http.Headers

let private sendRequest
    (httpClient: HttpClient)
    (processResponseAsync: HttpResponseMessage -> Task<'T>)
    (request: HttpRequestMessage)
    : Task<'T option> =
    task {
        try
            use! response = httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)

            response.EnsureSuccessStatusCode() |> ignore

            let! result = processResponseAsync response

            return Some result
        with :? HttpRequestException as _ ->
            return None
    }

let private isAcceptRangeSupported (response: HttpResponseMessage) : Task<bool> =
    task {
        match response.Headers.AcceptRanges |> Seq.toArray with
        | [| "bytes" |] -> return true
        | _ -> return false
    }

let private saveResponse (fs: FileStream) (response: HttpResponseMessage) : Task<unit> =
    task { do! response.Content.CopyToAsync fs }

let private getStringBody (response: HttpResponseMessage) : Task<string> =
    task { return! response.Content.ReadAsStringAsync() }

let private headRequestMessage (uri: Uri) : HttpRequestMessage =
    new HttpRequestMessage(HttpMethod.Head, uri)

let private getRequestMessage (uri: Uri) : HttpRequestMessage =
    new HttpRequestMessage(HttpMethod.Get, uri)

let private addRangeHeaderToRequestMessage (start: int64) (requestMessage: HttpRequestMessage) : HttpRequestMessage =
    let range = RangeHeaderValue(start, Nullable())

    requestMessage.Headers.Range <- range

    requestMessage

let rec private doWithRetry
    (makeReportMessage: uint -> string)
    (reportProgress: string -> unit)
    (makeRequest: unit -> Task<'T option>)
    (retries: uint)
    : Task<'T> =
    task {
        if retries <= 0u then
            raise (InvalidOperationException $"Count of retries is exhausted")

        match! makeRequest () with
        | Some r -> return r
        | None ->
            retries - 1u |> makeReportMessage |> reportProgress

            do! Task.Delay(10_000)

            return! doWithRetry makeReportMessage reportProgress makeRequest (retries - 1u)
    }

let private makeReportMessageForDownloadingFile (fileName: string) (retries: uint) : string =
    $"Attempt to download {fileName} failed. Remaing retries {retries}"

let private makeReportMessageForCheckingRange (retries: uint) : string =
    $"Attempt to check Accept-Range header failed. Remaing retries {retries}"

let private downloadRangeFileWithRetryAsync
    (httpClient: HttpClient)
    (reportProgress: string -> unit)
    (fs: FileStream)
    (uri: Uri)
    (fileName: string)
    (retries: uint)
    : Task<unit> =
    task {
        let makeRequest () =
            task {
                use requestMessage =
                    uri |> getRequestMessage |> addRangeHeaderToRequestMessage fs.Length

                return! sendRequest httpClient (saveResponse fs) requestMessage
            }

        do! doWithRetry (makeReportMessageForDownloadingFile fileName) reportProgress makeRequest retries
    }

let private downloadFileWithRetryAsync
    (pathToSqlDumps: string)
    (httpClient: HttpClient)
    (reportProgress: string -> unit)
    (uri: Uri)
    (fileName: string)
    (retries: uint)
    : Task<unit> =
    task {
        let makeRequest () =
            task {
                use fs = File.Create(Path.Combine(pathToSqlDumps, fileName))

                use requestMessage = uri |> getRequestMessage

                return! sendRequest httpClient (saveResponse fs) requestMessage
            }

        do! doWithRetry (makeReportMessageForDownloadingFile fileName) reportProgress makeRequest retries
    }

let private checkIfRequestSupportsRangeWithRetryAsync
    (httpClient: HttpClient)
    (reportProgress: string -> unit)
    (uri: Uri)
    (retries: uint)
    : Task<bool> =
    task {
        let makeRequest () =
            task {
                use requestMessage = uri |> headRequestMessage

                return! sendRequest httpClient isAcceptRangeSupported requestMessage
            }

        return! doWithRetry makeReportMessageForCheckingRange reportProgress makeRequest retries
    }

let private downloadStringWithRetryAsync
    (httpClient: HttpClient)
    (reportProgress: string -> unit)
    (uri: Uri)
    (retries: uint)
    : Task<string> =
    task {
        let makeRequest () =
            task {
                use requestMessage = uri |> getRequestMessage

                return! sendRequest httpClient getStringBody requestMessage
            }

        return! doWithRetry makeReportMessageForCheckingRange reportProgress makeRequest retries
    }

let downloadFileAsync
    (pathToSqlDumps: string)
    (httpClient: HttpClient)
    (reportProgress: string -> unit)
    (uri: Uri)
    (fileName: string)
    (retries: uint)
    : Task =
    task {
        let! supportRange = checkIfRequestSupportsRangeWithRetryAsync httpClient reportProgress uri retries

        if supportRange then
            use fileStrean = File.Create(Path.Combine(pathToSqlDumps, fileName))

            do! downloadRangeFileWithRetryAsync httpClient reportProgress fileStrean uri fileName retries
        else
            do! downloadFileWithRetryAsync pathToSqlDumps httpClient reportProgress uri fileName retries
    }

let downloadStringAsync
    (httpClient: HttpClient)
    (reportProgress: string -> unit)
    (uri: Uri)
    (retries: uint)
    : Task<string> =
    task { return! downloadStringWithRetryAsync httpClient reportProgress uri retries }
