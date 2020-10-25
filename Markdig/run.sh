#/bin/bash
set -eux

if [ -d bin ]; then rm -rf bin; fi
if [ -d Findings ]; then rm -rf Findings; fi

dotnet publish Markdig.Fuzz/Markdig.Fuzz.csproj -c release -o bin
sharpfuzz bin/Markdig.dll
afl-fuzz -i Testcases/ -o Findings/ -t 10000 -m 10000 dotnet bin/Markdig.Fuzz.dll
