#/bin/sh
set -eux

cd ../System.Private.CoreLib.Fuzz
mkdir -p ../Binaries
mkdir -p ../Findings

target=$1
platform=$2
coreLibDir=$3

shift 3

dotnet publish -r $platform -o ../Binaries/$target
cp ../$coreLibDir/System.Private.CoreLib.dll ../Binaries/$target
sharpfuzz ../Binaries/$target/System.Private.CoreLib.dll $@

cd ..
rm -rf Findings/$target
cp -r Testcases/$target/ Findings/$target

clang-7 -fsanitize=fuzzer LibFuzzer/libFuzzer.c -o Binaries/$target/fuzzer

Binaries/$target/fuzzer \
	-timeout=10 \
	--target_path=Binaries/$target/System.Private.CoreLib.Fuzz \
	--target_arg=$target \
	Findings/$target
