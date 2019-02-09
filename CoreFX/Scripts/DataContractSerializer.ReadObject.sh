#/bin/sh
set -eux

./CoreFX.FuzzDict.sh DataContractSerializer.ReadObject System.Private.DataContractSerialization.dll /usr/local/share/afl/dictionaries/xml.dict "$1"
