#/bin/sh
set -eux

./CoreFX.Fuzz.sh Utf8Parser.TryParseDouble System.Memory.dll linux-x64
