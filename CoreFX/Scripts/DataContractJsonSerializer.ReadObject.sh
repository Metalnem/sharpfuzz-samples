#/bin/sh
set -eux

./CoreFX.FuzzDict.sh DataContractJsonSerializer.ReadObject System.Private.DataContractSerialization.dll /usr/local/share/afl/dictionaries/json.dict "$1"
