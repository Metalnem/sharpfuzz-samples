#/bin/sh
set -eux

cd ../CoreFX.Fuzz
mkdir -p ../Binaries
mkdir -p ../Findings

dotnet publish -r osx-x64 -o ../Binaries/XmlReader.Create
sharpfuzz ../Binaries/XmlReader.Create/System.Private.Xml.dll

cd ..
rm -rf Findings/XmlReader.Create

afl-fuzz \
	-i Testcases/XmlReader.Create \
	-o Findings/XmlReader.Create \
	-t 5000 \
	-m 10000 \
	Binaries/XmlReader.Create/CoreFX.Fuzz \
	@@ XmlReader.Create
