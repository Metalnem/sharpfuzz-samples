#/bin/sh
set -eux

./CoreFX.FuzzDict.sh XmlReader.Create System.Private.Xml.dll /usr/local/share/afl/dictionaries/xml.dict linux-x64
