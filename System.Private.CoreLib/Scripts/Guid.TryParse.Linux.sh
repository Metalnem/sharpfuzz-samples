#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "Guid.TryParse" "linux-x64" "Linux.x64.Release" "System.Guid"
