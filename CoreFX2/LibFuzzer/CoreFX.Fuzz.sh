#/bin/sh
set -eux

cd ../CoreFX.Fuzz
mkdir -p ../Binaries
mkdir -p ../Findings

dotnet publish -r "$3" -o ../Binaries/"$1"
sharpfuzz ../Binaries/"$1"/"$2"

cd ..
rm -rf Findings/"$1"
cp -r Testcases/"$1"/ Findings/"$1"

clang-7 -fsanitize=fuzzer LibFuzzer/libFuzzer.c -o Binaries/"$1"/fuzzer

Binaries/"$1"/fuzzer \
	-timeout=10 \
	--target_path=Binaries/"$1"/CoreFX.Fuzz \
	--target_arg="$1" \
	Findings/"$1"
