#/bin/sh
set -eux

./CoreFX.Fuzz.sh HttpUtility.UrlEncode System.Web.HttpUtility.dll linux-x64
