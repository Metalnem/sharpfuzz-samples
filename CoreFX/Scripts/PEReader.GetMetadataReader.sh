#/bin/sh
set -eux

cd ../CoreFX.Fuzz
mkdir -p ../Binaries
mkdir -p ../Findings

dotnet publish -r "$1" -o ../Binaries/PEReader.GetMetadataReader
sharpfuzz ../Binaries/PEReader.GetMetadataReader/System.Reflection.Metadata.dll

cd ..
rm -rf Findings/PEReader.GetMetadataReader

afl-fuzz \
	-i Testcases/PEReader.GetMetadataReader \
	-o Findings/PEReader.GetMetadataReader \
	-t 5000 \
	-m 10000 \
	Binaries/PEReader.GetMetadataReader/CoreFX.Fuzz \
	@@ PEReader.GetMetadataReader
