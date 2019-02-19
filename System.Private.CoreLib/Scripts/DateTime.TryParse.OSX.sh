#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "DateTime.TryParse" "osx-x64" "OSX.x64.Release" "System.DateTime" "System.Globalization.DateTime"
