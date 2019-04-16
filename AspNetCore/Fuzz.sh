#/bin/sh
set -eux

rm -rf Binaries
rm -rf Findings

dotnet publish -r "$1" -o Binaries
sharpfuzz Binaries/Microsoft.AspNetCore.Server.Kestrel.Core.dll

wget -q https://github.com/Metalnem/sharpfuzz/releases/download/libfuzzer-proto-dotnet-0.0.1/libfuzzer-proto-dotnet-0.0.1.zip
unzip -o libfuzzer-proto-dotnet-0.0.1.zip

./libfuzzer-proto-dotnet \
	-timeout=5 \
	-dict=Http.dict \
	--target_path=Binaries/AspNetCore.Fuzz \
	Testcases
