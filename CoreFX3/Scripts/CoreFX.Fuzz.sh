#/bin/sh
set -eux

cd ../CoreFX.Fuzz
mkdir -p ../Binaries
mkdir -p ../Findings

/Users/Metalnem/Temp/dotnet-sdk-3.0.100-preview3/dotnet publish -r "$3" -o ../Binaries/"$1"
sharpfuzz ../Binaries/"$1"/"$2"

cd ..
rm -rf Findings/"$1"

afl-fuzz \
	-i Testcases/"$1" \
	-o Findings/"$1" \
	-t 5000 \
	-m 10000 \
	Binaries/"$1"/CoreFX.Fuzz "$1"
