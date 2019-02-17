#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "$1" "osx-x64" "OSX.x64.Release"
