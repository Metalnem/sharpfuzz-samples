#/bin/bash

sharpfuzz out/"$TARGET_DLL"

if [ "${TARGET_ENGINE,,}" == "libfuzzer" ]; then
	if [ -z "$TARGET_DICTIONARY" ]; then
		./libFuzzer/libfuzzer-dotnet \
			-timeout=10 \
			--target_path=out/CoreFX.Fuzz \
			--target_arg="$TARGET_FUNCTION" \
			findings
	else
		./libFuzzer/libfuzzer-dotnet \
			-timeout=10 \
			-dict=dictionaries/"$TARGET_DICTIONARY" \
			--target_path=out/CoreFX.Fuzz \
			--target_arg="$TARGET_FUNCTION" \
			findings
	fi
else
	if [ -z "$TARGET_DICTIONARY" ]; then
		./afl/afl-fuzz \
			-i testcases/"$TARGET_FUNCTION" \
			-o findings \
			-t 5000 \
			-m 10000 \
			out/CoreFX.Fuzz "$TARGET_FUNCTION"
	else
		./afl/afl-fuzz \
			-i testcases/"$TARGET_FUNCTION" \
			-o findings \
			-t 5000 \
			-m 10000 \
			-x dictionaries/"$TARGET_DICTIONARY" \
			out/CoreFX.Fuzz "$TARGET_FUNCTION"
	fi
fi
