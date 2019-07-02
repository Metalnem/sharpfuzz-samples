FROM ubuntu AS afl-env
WORKDIR /app

RUN apt-get update && apt-get install -y gcc make wget
RUN wget http://lcamtuf.coredump.cx/afl/releases/afl-latest.tgz
RUN tar -xvf afl-latest.tgz

WORKDIR afl-2.52b
RUN make

FROM mcr.microsoft.com/dotnet/core/sdk:3.0
WORKDIR /app

ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global SharpFuzz.CommandLine

COPY CoreFX.Fuzz/CoreFX.Fuzz.csproj CoreFX.Fuzz/NuGet.Config ./
RUN dotnet restore -r linux-x64

COPY CoreFX.Fuzz/Program.cs ./
RUN dotnet publish -r linux-x64 --no-restore -c release -o out

COPY --from=afl-env /app/afl-2.52b ./afl/
COPY Testcases ./testcases/

ENV AFL_SKIP_BIN_CHECK=true

CMD sharpfuzz out/"$TARGET_DLL" \
	&& ./afl/afl-fuzz \
	-i testcases/"$TARGET_FUNCTION" \
	-o findings \
	-t 5000 \
	-m 10000 \
	./out/CoreFX.Fuzz "$TARGET_FUNCTION"
