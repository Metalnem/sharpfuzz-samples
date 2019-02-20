#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "Decimal.Multiply" "osx-x64" "OSX.x64.Release" "System.Decimal" "System.Number"
