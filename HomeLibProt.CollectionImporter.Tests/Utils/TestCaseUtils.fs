module HomeLibProt.CollectionImporter.Tests.Utils.TestCaseUtils

open System.IO
open System.Reflection

let getTestCasesBasePath (testName: string) =
    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestCases", testName)
