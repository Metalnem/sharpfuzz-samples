#/bin/sh
set -eux

./System.Private.CoreLib.Fuzz.sh "Convert.ToInt32" "osx-x64" "OSX.x64.Release" "System.Convert" "System.ParseNumbers"
