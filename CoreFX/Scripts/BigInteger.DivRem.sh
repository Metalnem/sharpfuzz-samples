#/bin/sh
set -eux

./CoreFX.Fuzz.sh BigInteger.DivRem System.Runtime.Numerics.dll "$1"
