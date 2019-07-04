#/bin/bash

sharpfuzz out/System.Private.CoreLib.dll System.Number

./afl/afl-fuzz \
	-i testcases/Double.TryParse \
	-o findings \
	-t 5000 \
	-m 10000 \
	out/System.Private.CoreLib.Fuzz Double.TryParse
