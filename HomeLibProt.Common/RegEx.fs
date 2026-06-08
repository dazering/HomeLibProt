module HomeLibProt.Common.RegEx

open System.Text.RegularExpressions

let extractGroupValue (group: Group) : string = group.Value

let tryToExtractGroupValue (group: Group) : string option =
    match group.Success with
    | true -> group |> extractGroupValue |> Some
    | _ -> None
