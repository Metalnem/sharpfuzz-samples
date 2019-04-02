#/bin/sh
set -eux

./CoreFX.Fuzz.sh Utf8Parser.TryParseTimeSpan System.Memory.dll linux-x64
