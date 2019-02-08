#/bin/sh
set -eux

./CoreFX.Fuzz.sh ZipArchive.Entries System.IO.Compression.dll "$1"
