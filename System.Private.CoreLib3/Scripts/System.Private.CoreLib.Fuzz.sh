#/bin/sh
set -eux

cd ../System.Private.CoreLib.Fuzz
mkdir -p ../Binaries
mkdir -p ../Findings

target=$1
platform=$2
coreLibDir=$3

shift 3

/Users/Metalnem/Temp/dotnet-sdk-latest-osx-x64/dotnet publish -r $platform -o ../Binaries/$target
cp ../$coreLibDir/System.Private.CoreLib.dll ../Binaries/$target
sharpfuzz ../Binaries/$target/System.Private.CoreLib.dll $@

cd ..
rm -rf Findings/$target

afl-fuzz \
	-i Testcases/$target \
	-o Findings/$target \
	-t 5000 \
	-m 10000 \
	Binaries/$target/System.Private.CoreLib.Fuzz \
	@@ $target
