#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "IdnMapping.GetAscii" "linux-x64" "Linux.x64.Release" "System.Globalization.IdnMapping"
