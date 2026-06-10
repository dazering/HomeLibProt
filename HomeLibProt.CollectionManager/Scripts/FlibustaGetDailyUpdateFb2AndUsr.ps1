param (
    [Parameter(Mandatory)]
    [string] $InpxPath,
    [Parameter(Mandatory)]
    [string] $LibraryPath
)

function CreateDirectoryIfNeeded {
    param (
        [string] $path
        )
        
        if (-not (Test-Path -Path $path -PathType Container)) {
            mkdir $path
        }
    }

$toolPath = "$PSScriptRoot/../"
$fb2UpdatesPath = "$LibraryPath/Updates/Fb2"
$usrUpdatesPath = "$LibraryPath/Updates/Binary"
$sqlDumpsPath = "$toolPath/flibusta_sql_dumps"

CreateDirectoryIfNeeded $fb2UpdatesPath
CreateDirectoryIfNeeded $usrUpdatesPath
CreateDirectoryIfNeeded $sqlDumpsPath

$datePrefix = Get-Date -Format "yy-MM"
$siteKey = "flibusta"
$inpxName = "flibusta_all_local.inpx"

& $toolPath/HomeLibProt.CollectionManager.exe downloadbooks -i $LibraryPath -o $fb2UpdatesPath -s $siteKey -r 10 -a fb2

if (!$?) {
    throw "Downloading fb2 archives failed"
}

& $toolPath/HomeLibProt.CollectionManager.exe downloadbooks -i $LibraryPath -o $usrUpdatesPath -s $siteKey -r 10 -a binary

if (!$?) {
    throw "Downloading usr archives failed"
}

& $toolPath/HomeLibProt.CollectionManager.exe mergebooks -i $fb2UpdatesPath -o $LibraryPath -s 10000 -p "$datePrefix-f.fb2-" -f "*.zip"

if (!$?) {
    throw "Merge daily fb2 archives failed"
}

& $toolPath/HomeLibProt.CollectionManager.exe mergebooks -i $usrUpdatesPath -o $LibraryPath -s 10000 -p "$datePrefix-f.usr-" -f "*.zip"

if (!$?) {
    throw "Merge daily usr archives failed"
}

& $toolPath/HomeLibProt.CollectionManager.exe downloadsqldumps -i $sqlDumpsPath -s $siteKey -r 10

if (!$?) {
    throw "Downloading sql dumps failed"
}

& $toolPath/HomeLibProt.CollectionManager.exe importsqldumps -i $sqlDumpsPath -d "$sqlDumpsPath/sql_dump.db" -s $siteKey

if (!$?) {
    throw "Importing sql dumps failed"
}

& $toolPath/HomeLibProt.CollectionManager.exe generateinpx -i $LibraryPath -d "$sqlDumpsPath/sql_dump.db" -o "$sqlDumpsPath/$inpxName" -s $siteKey -l all

if (!$?) {
    throw "Generating inpx failed"
}

Copy-Item -Path "$sqlDumpsPath/$inpxName" -Destination $InpxPath -Force