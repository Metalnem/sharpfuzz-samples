#/bin/sh
set -eux

./CoreFX.Fuzz.sh BigInteger.Multiply System.Runtime.Numerics.dll "$1"
