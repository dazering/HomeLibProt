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

[<Test>]
[<TestCaseSource(nameof getFb2AttributesTestCases)>]
let TestGetFb2Attributes (testCaseName: string, expected: Fb2Attributes option) =
    use fb2 = getBookPath testCaseName |> File.OpenRead

    let actual = getFb2Attributes fb2

    Assert.That(actual, Is.EqualTo expected)
