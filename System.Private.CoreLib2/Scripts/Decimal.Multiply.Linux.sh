#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "Decimal.Multiply" "linux-x64" "Linux.x64.Release" "System.Decimal" "System.Number"
