#/bin/sh
set -eux

./CoreFX.Fuzz.sh Utf8Parser.TryParseDateTime System.Memory.dll "$1"
