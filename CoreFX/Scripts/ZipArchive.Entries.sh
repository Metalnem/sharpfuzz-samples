#/bin/sh
set -eux

cd ../CoreFX.Fuzz
mkdir -p ../Binaries
mkdir -p ../Findings

dotnet publish -r "$1" -o ../Binaries/ZipArchive.Entries
sharpfuzz ../Binaries/ZipArchive.Entries/System.IO.Compression.dll

cd ..
rm -rf Findings/ZipArchive.Entries

afl-fuzz \
	-i Testcases/ZipArchive.Entries \
	-o Findings/ZipArchive.Entries \
	-t 5000 \
	-m 10000 \
	Binaries/ZipArchive.Entries/CoreFX.Fuzz \
	@@ ZipArchive.Entries
