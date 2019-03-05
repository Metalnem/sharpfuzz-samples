#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "DateTime.TryParse" "linux-x64" "Linux.x64.Release" "System.DateTime" "System.Globalization.DateTime"
