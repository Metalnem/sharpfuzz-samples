#/bin/bash

SHARPFUZZ_PRINT_INSTRUMENTED_TYPES=true sharpfuzz out/System.Private.CoreLib.dll "$TARGET_PREFIXES"

if [ "${TARGET_ENGINE,,}" == "libfuzzer" ]; then
	./libFuzzer/libfuzzer-dotnet \
		-timeout=10 \
		--target_path=out/CoreCLR.Fuzz \
		--target_arg="$TARGET_FUNCTION" \
		findings
else
	./afl/afl-fuzz \
		-i testcases/"$TARGET_FUNCTION" \
		-o findings \
		-t 5000 \
		-m 10000 \
		out/CoreCLR.Fuzz "$TARGET_FUNCTION"
fi
