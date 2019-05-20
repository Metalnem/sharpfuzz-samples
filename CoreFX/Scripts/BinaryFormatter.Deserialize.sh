#/bin/sh
set -eux

./CoreFX.Fuzz.sh BinaryFormatter.Deserialize System.Runtime.Serialization.Formatters.dll "$1"
