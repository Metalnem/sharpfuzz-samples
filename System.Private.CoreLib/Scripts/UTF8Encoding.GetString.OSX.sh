#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "UTF8Encoding.GetString" "osx-x64" "OSX.x64.Release" "System.Text"
