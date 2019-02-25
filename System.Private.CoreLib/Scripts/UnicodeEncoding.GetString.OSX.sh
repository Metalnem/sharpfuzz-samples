#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "UnicodeEncoding.GetString" "osx-x64" "OSX.x64.Release" "System.Text"
