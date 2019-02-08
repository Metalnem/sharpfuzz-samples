#/bin/sh
set -eux

./CoreFX.Fuzz.sh BigInteger.TryParse System.Runtime.Numerics.dll "$1"
