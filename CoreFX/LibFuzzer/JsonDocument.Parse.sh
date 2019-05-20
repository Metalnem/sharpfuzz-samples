#/bin/sh
set -eux

./CoreFX.FuzzDict.sh JsonDocument.Parse System.Text.Json.dll /usr/local/share/afl/dictionaries/json.dict linux-x64
