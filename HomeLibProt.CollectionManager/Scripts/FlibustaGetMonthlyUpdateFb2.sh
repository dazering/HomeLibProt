#!/bin/sh

if [ -z "$1" ]; then
    echo "Usage: $0 <InpxPath> <LibraryPath>"
    exit 1
fi

if [ -z "$2" ]; then
    echo "Usage: $0 $1 <LibraryPath>"
    exit 1
fi

InpxPath=$1
LibraryPath=$2

RootPath=$(dirname "$(realpath "$0")")
ToolPath=$RootPath/../

SqlDumpsPath=$ToolPath/flibusta_sql_dumps/

mkdir -p $SqlDumpsPath

SiteKey="flibusta"
InpxName="flibusta_all_local.inpx"
DatePrefix=$(date -d "14 day ago" "+%y-%m")

$ToolPath/HomeLibProt.CollectionManager mergebooks -i $LibraryPath -o $LibraryPath -s 10000 -p "f.fb2-" -f "$DatePrefix-f.fb2-*.zip"

if [ $? -ne 0 ]; then
    echo "Merge monthly fb2 archives failed"
    exit 1 
fi

$ToolPath/HomeLibProt.CollectionManager downloadsqldumps -i $SqlDumpsPath -s $SiteKey -r 10
if [ $? -ne 0 ]; then
    echo "Downloading sql dumps failed"
    exit 1 
fi

$ToolPath/HomeLibProt.CollectionManager importsqldumps -i $SqlDumpsPath -d "$SqlDumpsPath/sql_dump.db" -s $SiteKey

if [ $? -ne 0 ]; then
    echo "Importing sql dumps failed"
    exit 1 
fi

$ToolPath/HomeLibProt.CollectionManager generateinpx -i $LibraryPath -d "$SqlDumpsPath/sql_dump.db" -o "$SqlDumpsPath/$InpxName"

if [ $? -ne 0 ]; then
    echo "Generating inpx failed"
    exit 1 
fi

cp -f "$SqlDumpsPath/$InpxName" $InpxPath