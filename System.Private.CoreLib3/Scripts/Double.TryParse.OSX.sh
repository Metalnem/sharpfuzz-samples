#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "Double.TryParse" "osx-x64" "OSX.x64.Release" "System.Number"
