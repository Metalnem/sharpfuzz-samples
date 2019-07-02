#/bin/sh

sharpfuzz out/"$TARGET_DLL"
mkdir findings

if [ -z "$TARGET_DICTIONARY" ]; then
	./libfuzzer-dotnet \
		-timeout=10 \
		--target_path=out/CoreFX.Fuzz \
		--target_arg="$TARGET_FUNCTION" \
		findings
else
	./libfuzzer-dotnet \
		-timeout=10 \
		-dict=dictionaries/"$TARGET_DICTIONARY" \
		--target_path=out/CoreFX.Fuzz \
		--target_arg="$TARGET_FUNCTION" \
		findings
fi
