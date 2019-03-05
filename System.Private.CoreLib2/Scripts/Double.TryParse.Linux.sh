#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "Double.TryParse" "linux-x64" "Linux.x64.Release" "System.Number"
