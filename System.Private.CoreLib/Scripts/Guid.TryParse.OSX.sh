#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "Guid.TryParse" "osx-x64" "OSX.x64.Release" "System.Guid"
