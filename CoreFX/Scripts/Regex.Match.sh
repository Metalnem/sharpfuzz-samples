#/bin/sh
set -eux

./CoreFX.Fuzz.sh Regex.Match System.Text.RegularExpressions.dll "$1"
