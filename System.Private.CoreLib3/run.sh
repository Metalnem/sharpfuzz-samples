#/bin/bash

sharpfuzz out/System.Private.CoreLib.dll System.Number
out/System.Private.CoreLib.Fuzz Double.TryParse
