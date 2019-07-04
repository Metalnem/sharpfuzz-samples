#/bin/bash

sharpfuzz out/System.Private.CoreLib.dll "$TARGET_PREFIX"

if [ "${TARGET_ENGINE,,}" == "libfuzzer" ]; then
	./libFuzzer/libfuzzer-dotnet \
		-timeout=10 \
		--target_path=out/System.Private.CoreLib.Fuzz \
		--target_arg="$TARGET_FUNCTION" \
		findings
else
	./afl/afl-fuzz \
		-i testcases/"$TARGET_FUNCTION" \
		-o findings \
		-t 5000 \
		-m 10000 \
		out/System.Private.CoreLib.Fuzz "$TARGET_FUNCTION"
fi
