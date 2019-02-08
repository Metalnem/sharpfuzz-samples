#/bin/sh
set -eux

./CoreFX.Fuzz.sh PEReader.GetMetadataReader System.Reflection.Metadata.dll "$1"
