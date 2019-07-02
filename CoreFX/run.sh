#/bin/sh

sharpfuzz out/"$TARGET_DLL"

if [ -z "$TARGET_DICTIONARY" ]; then
	./afl/afl-fuzz \
		-i testcases/"$TARGET_FUNCTION" \
		-o findings \
		-t 5000 \
		-m 10000 \
		./out/CoreFX.Fuzz "$TARGET_FUNCTION"
else
	./afl/afl-fuzz \
		-i testcases/"$TARGET_FUNCTION" \
		-o findings \
		-t 5000 \
		-m 10000 \
		-x ./afl/dictionaries/"$TARGET_DICTIONARY" \
		./out/CoreFX.Fuzz "$TARGET_FUNCTION"
fi
