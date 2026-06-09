param (
    [Parameter(Mandatory)]
    [string] $InpxPath,
    [Parameter(Mandatory)]
    [string] $LibraryPath,
    [Parameter(Mandatory)]
    [string] $DatabasePath
)

$toolPath = "$PSScriptRoot/../"
$importedDatabasePath = "$toolPath/Inpx.db"

if (Test-Path -Path $importedDatabasePath -PathType Leaf) {
    & $toolPath/HomeLibProt.CommandLineImporter.exe importinpx -i $InpxPath -a $LibraryPath -d $importedDatabasePath
}
else {
    & $toolPath/HomeLibProt.CommandLineImporter.exe importinpx -i $InpxPath -a $LibraryPath -d $importedDatabasePath -f
}

if (!$?) {
    throw "Import inpx failed"
}

Copy-Item -Path $importedDatabasePath -Destination $DatabasePath -Force