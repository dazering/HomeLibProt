#!/bin/sh

if [ -z "$1" ]; then
    echo "Usage: $0 <InpxPath> <LibraryPath> <DatabasePath>"
    exit 1
fi

if [ -z "$2" ]; then
    echo "Usage: $0 $1 <LibraryPath> <DatabasePath>"
    exit 1
fi

if [ -z "$3" ]; then
    echo "Usage: $0 $1 $2 <DatabasePath>"
    exit 1
fi

InpxPath=$1
LibraryPath=$2
DatabasePath=$3

RootPath=$(dirname "$(realpath "$0")")
ToolPath=$RootPath/../

ImportedDatabasePath=$ToolPath/Inpx.db

if [ -f $ImportedDatabasePath ]; then
    $ToolPath/HomeLibProt.CommandLineImporter importinpx -i $InpxPath -a $LibraryPath -d $ImportedDatabasePath
else
    $ToolPath/HomeLibProt.CommandLineImporter importinpx -i $InpxPath -a $LibraryPath -d $ImportedDatabasePath -f
fi

if [ $? -ne 0 ]; then
    echo "Import inpx failed"
    exit 1 
fi

cp -f $ImportedDatabasePath $DatabasePath