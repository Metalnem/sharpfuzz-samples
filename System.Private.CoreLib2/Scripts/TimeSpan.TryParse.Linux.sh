#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "TimeSpan.TryParse" "linux-x64" "Linux.x64.Release" "System.TimeSpan" "System.Globalization.TimeSpan"
