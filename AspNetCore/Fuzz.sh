#/bin/sh
set -eux

rm -rf Binaries
rm -rf Findings

dotnet publish -r "$1" -o Binaries
sharpfuzz Binaries/Microsoft.AspNetCore.Server.Kestrel.Core.dll

./libfuzzer-proto-dotnet \
	-timeout=5 \
	-dict=Http.dict \
	--target_path=Binaries/AspNetCore.Fuzz \
	Testcases
