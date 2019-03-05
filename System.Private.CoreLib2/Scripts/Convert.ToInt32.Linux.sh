#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "Convert.ToInt32" "linux-x64" "Linux.x64.Release" "System.Convert" "System.ParseNumbers"
