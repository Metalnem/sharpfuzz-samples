#/bin/bash
set -eux

if [ -d bin ]; then rm -rf bin; fi
if [ -d Findings ]; then rm -rf Findings; fi

dotnet publish Bond.Fuzz/Bond.Fuzz.csproj -c release -o bin
sharpfuzz bin/Bond.dll
sharpfuzz bin/Bond.IO.dll
sharpfuzz bin/Bond.Reflection.dll
afl-fuzz -i Testcases/ -o Findings/ -t 10000 -m 10000 dotnet bin/Bond.Fuzz.dll
