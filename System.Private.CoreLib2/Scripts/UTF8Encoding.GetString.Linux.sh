#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "UTF8Encoding.GetString" "linux-x64" "Linux.x64.Release" "System.Text"
