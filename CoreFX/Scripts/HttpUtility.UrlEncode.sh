#/bin/sh
set -eux

cd ../CoreFX.Fuzz
mkdir -p ../Binaries
mkdir -p ../Findings

dotnet publish -r "$1" -o ../Binaries/HttpUtility.UrlEncode
sharpfuzz ../Binaries/HttpUtility.UrlEncode/System.Web.HttpUtility.dll

cd ..
rm -rf Findings/HttpUtility.UrlEncode

afl-fuzz \
	-i Testcases/HttpUtility.UrlEncode \
	-o Findings/HttpUtility.UrlEncode \
	-t 5000 \
	-m 10000 \
	Binaries/HttpUtility.UrlEncode/CoreFX.Fuzz \
	@@ HttpUtility.UrlEncode
