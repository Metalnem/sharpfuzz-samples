#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "IdnMapping.GetAscii" "osx-x64" "OSX.x64.Release" "System.Globalization.IdnMapping"
