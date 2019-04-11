#/bin/sh
set -eux

rm -rf Binaries
rm -rf Findings

dotnet publish -r "$1" -o Binaries
sharpfuzz Binaries/Microsoft.AspNetCore.Server.Kestrel.Core.dll

afl-fuzz -i Testcases -o Findings -t 5000 -m 1000 -x Http.dict Binaries/AspNetCore.Fuzz
