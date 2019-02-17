#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "$1" "linux-x64" "Linux.x64.Release"
