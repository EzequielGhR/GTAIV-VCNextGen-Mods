#!/bin/bash

CMD="";
if command -v csc &> /dev/null; then
    echo "'csc' command installed"
    CMD="csc"
elif command -v msc &> /dev/null; then
    echo "'msc' command installed"
    CMD="msc"
else
    echo "'csc' or 'msc' command needed for compilation. Have you installed 'mono'?"
    exit 1
fi

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
CURRENT_DIR=$(pwd)

cd $SCRIPT_DIR && {
    $CMD -target:library \
        -r:"../../lib/ScriptHookDotNet.dll" \
        -r:"../../lib/NAudioAdvanced-IV.dll" \
        -out:"bin/MateScript.net.dll" \
        "MateScript.cs"
} || {
    echo "There was an issue compiling the script"
    cd $CURRENT_DIR
    exit 1
}

echo "Script compiled to bin/MateScript.net.dll"
cd $CURRENT_DIR
