#/bin/sh
set -eux

./CoreFX.Fuzz.sh BigInteger.ModPow System.Runtime.Numerics.dll "$1"
