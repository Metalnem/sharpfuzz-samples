#/bin/sh
set -eux

cd ../CoreFX.Fuzz
mkdir -p ../Binaries
mkdir -p ../Findings

dotnet publish -r "$1" -o ../Binaries/XmlReader.Create
sharpfuzz ../Binaries/XmlReader.Create/System.Private.Xml.dll

cd ..
rm -rf Findings/XmlReader.Create

afl-fuzz \
	-i Testcases/XmlReader.Create \
	-o Findings/XmlReader.Create \
	-t 5000 \
	-m 10000 \
	-x /usr/local/share/afl/dictionaries/xml.dict \
	Binaries/XmlReader.Create/CoreFX.Fuzz \
	@@ XmlReader.Create
