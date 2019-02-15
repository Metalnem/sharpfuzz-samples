#/bin/sh
set -eux

cd ../System.Private.CoreLib.Fuzz
mkdir -p ../Binaries
mkdir -p ../Findings

dotnet publish -r osx-x64 -o ../Binaries/"$1"
cp ../OSX.x64.Debug/System.Private.CoreLib.dll ../Binaries/"$1"
sharpfuzz ../Binaries/"$1"/System.Private.CoreLib.dll

cd ..
rm -rf Findings/"$1"

afl-fuzz \
	-i Testcases/"$1" \
	-o Findings/"$1" \
	-t 5000 \
	-m 10000 \
	Binaries/"$1"/System.Private.CoreLib.Fuzz \
	@@ "$1"
