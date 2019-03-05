#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "TimeSpan.TryParse" "osx-x64" "OSX.x64.Release" "System.TimeSpan" "System.Globalization.TimeSpan"
