module HomeLibProt.CollectionManager.SqlDumps.SqlDumpParser

open System.IO
open System.Text
open System.Text.RegularExpressions

let private appendChar (char: char) (sb: StringBuilder) = char |> sb.Append |> ignore

let private readSqlDumpEntry (sqlDumpStream: StreamReader) : seq<string> =
    seq {
        let mutable stringBuilder = StringBuilder()

        let mutable capture = false
        let mutable ignoreClosingParentheses = false
        let mutable escapeNextChar = false

        while sqlDumpStream.EndOfStream |> not do
            let char = sqlDumpStream.Read() |> char

            match char, capture, ignoreClosingParentheses, escapeNextChar with
            | ''', true, _, false ->
                ignoreClosingParentheses <- ignoreClosingParentheses |> not
                (char, stringBuilder) ||> appendChar
            | '(', false, false, false ->
                capture <- true
                (char, stringBuilder) ||> appendChar
            | ')', true, false, false ->
                (char, stringBuilder) ||> appendChar
                capture <- false
                yield stringBuilder.ToString()
                stringBuilder <- StringBuilder()
            | '\\', true, _, false ->
                escapeNextChar <- true
                (char, stringBuilder) ||> appendChar
            | _, true, _, true ->
                escapeNextChar <- false
                (char, stringBuilder) ||> appendChar
            | _, true, _, false -> (char, stringBuilder) ||> appendChar
            | _ -> ()
    }

let parseSqlDump<'T>
    (getResult: (GroupCollection -> 'T))
    (regExPattern: string)
    (sqlDumpStream: StreamReader)
    : seq<'T> =
    seq {
        let regEx = Regex(regExPattern, RegexOptions.Compiled)

        yield!
            sqlDumpStream
            |> readSqlDumpEntry
            |> Seq.map regEx.Match
            |> Seq.filter (fun m -> m.Success)
            |> Seq.map (fun m -> m.Groups |> getResult)
    }
