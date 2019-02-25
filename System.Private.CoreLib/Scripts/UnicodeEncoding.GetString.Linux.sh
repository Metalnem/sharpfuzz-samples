#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "UnicodeEncoding.GetString" "linux-x64" "Linux.x64.Release" "System.Text"
