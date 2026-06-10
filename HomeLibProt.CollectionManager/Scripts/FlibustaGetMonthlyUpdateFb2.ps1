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
$sqlDumpsPath = "$toolPath/flibusta_sql_dumps"

CreateDirectoryIfNeeded $sqlDumpsPath

$datePrefix = (Get-Date).AddDays(-14).ToString("yy-MM")
$siteKey = "flibusta"
$inpxName = "flibusta_fb2_local.inpx"

& $toolPath/HomeLibProt.CollectionManager.exe mergebooks -i $LibraryPath -o $LibraryPath -s 10000 -p "f.fb2-" -f "$datePrefix-f.fb2-*.zip"

if (!$?) {
    throw "Merge monthly fb2 archives failed"
}

& $toolPath/HomeLibProt.CollectionManager.exe downloadsqldumps -i $sqlDumpsPath -s $siteKey -r 10

if (!$?) {
    throw "Downloading sql dumps failed"
}

& $toolPath/HomeLibProt.CollectionManager.exe importsqldumps -i $sqlDumpsPath -d "$sqlDumpsPath/sql_dump.db" -s $siteKey

if (!$?) {
    throw "Importing sql dumps failed"
}

& $toolPath/HomeLibProt.CollectionManager.exe generateinpx -i $LibraryPath -d "$sqlDumpsPath/sql_dump.db" -o "$sqlDumpsPath/$inpxName"

if (!$?) {
    throw "Generating inpx failed"
}

Copy-Item -Path "$sqlDumpsPath/$inpxName" -Destination $InpxPath -Force