module HomeLibProt.Common.Tests.TestFb2Reader

open NUnit.Framework
open System.IO

open HomeLibProt.Common.Fb2Reader
open HomeLibProt.Common.Tests.Utils

let getBookPath (testCaseName: string) =
    Path.Combine(TestCaseUtils.getTestCasesBasePath "TestFb2Reader", testCaseName, "Book.fb2")

let getFb2AttributesTestCases =
    [| TestCaseData(
           "GetAttributes/Simple",
           Some
               { Annotation = Some "<p>Annotation</p>"
                 Coverpage = Some "Base64" }
       )
       TestCaseData(
           "GetAttributes/OnlyReference",
           Some
               { Annotation = None
                 Coverpage = Some "Base64" }
       )
       TestCaseData("GetAttributes/NoAttributes", Some { Annotation = None; Coverpage = None }) |]

let getFb2InfoTestCases =
    [| TestCaseData(
           "GetFb2Info/Simple",
           Some
               { Authors =
                   [| { First = "Aa"
                        Middle = "Bb"
                        Last = "Cc" }
                      { First = "Dd"
                        Middle = "Ee"
                        Last = "Ff" } |]
                 Genres = [| "genre1"; "genre2" |]
                 Title = "Title 1"
                 Language = "en"
                 Keywords = [| "Keyword1"; "Keyword2"; "Keyword3" |]
                 Series = [| { Name = "Series 1"; Number = "1" }; { Name = "Series 2"; Number = "1" } |] }
       )
       TestCaseData(
           "GetFb2Info/MultilineTitle",
           Some
               { Authors =
                   [| { First = "Aa"
                        Middle = "Bb"
                        Last = "Cc" }
                      { First = "Dd"
                        Middle = "Ee"
                        Last = "Ff" } |]
                 Genres = [| "genre1"; "genre2" |]
                 Title = "Title 1"
                 Language = "en"
                 Keywords = [| "Keyword1"; "Keyword2"; "Keyword3" |]
                 Series = [| { Name = "Series 1"; Number = "1" }; { Name = "Series 2"; Number = "1" } |] }
       )
       TestCaseData(
           "GetFb2Info/NoAttributes",
           Some
               { Authors = [||]
                 Genres = [||]
                 Title = ""
                 Language = ""
                 Keywords = [||]
                 Series = [||] }
       ) |]

[<Test>]
[<TestCaseSource(nameof getFb2AttributesTestCases)>]
let TestGetFb2Attributes (testCaseName: string, expected: Fb2Attributes option) =
    use fb2 = getBookPath testCaseName |> File.OpenRead

    let actual = getFb2Attributes fb2

    Assert.That(actual, Is.EqualTo expected)

[<Test>]
[<TestCaseSource(nameof getFb2InfoTestCases)>]
let TestGetFb2Info (testCaseName: string, expected: Fb2Info option) =
    use fb2 = getBookPath testCaseName |> File.OpenRead

    let actual = getFb2Info fb2

    Assert.That(actual, Is.EqualTo expected)
