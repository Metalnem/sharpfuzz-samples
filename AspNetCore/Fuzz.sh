#/bin/sh
set -eux

rm -rf Binaries
rm -rf Findings

dotnet publish -r osx-x64 -o Binaries
sharpfuzz Binaries/Microsoft.AspNetCore.Server.Kestrel.Core.dll

afl-fuzz -i Testcases -o Findings -t 5000 -x Http.dict Binaries/AspNetCore.Fuzz
