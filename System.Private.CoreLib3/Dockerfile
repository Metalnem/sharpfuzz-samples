FROM mcr.microsoft.com/dotnet-buildtools/prereqs:ubuntu-16.04-c103199-20180628134544 AS build-env
WORKDIR /coreclr

RUN git clone --progress --verbose https://github.com/dotnet/coreclr.git .
RUN ./build.sh skiptests skipcrossgen skipnative release

WORKDIR /app
RUN wget http://lcamtuf.coredump.cx/afl/releases/afl-latest.tgz
RUN tar -xvf afl-latest.tgz

RUN wget https://github.com/Metalnem/libfuzzer-dotnet/releases/latest/download/libfuzzer-dotnet.tar.gz
RUN tar -xvf libfuzzer-dotnet.tar.gz

WORKDIR afl-2.52b
RUN make

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-bionic
WORKDIR /app

ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global SharpFuzz.CommandLine

COPY System.Private.CoreLib.Fuzz/System.Private.CoreLib.Fuzz.csproj ./
RUN dotnet restore -r linux-x64

COPY System.Private.CoreLib.Fuzz/Program.cs ./
RUN dotnet publish -r linux-x64 --no-restore -c release -o out

COPY --from=build-env /coreclr/bin/Product/Linux.x64.Release/IL/System.Private.CoreLib.dll ./out/
COPY --from=build-env /app/afl-2.52b ./afl/
COPY --from=build-env /app/libfuzzer-dotnet ./libFuzzer/

COPY run.sh ./
COPY Testcases ./testcases/

VOLUME /app/findings
ENV AFL_SKIP_BIN_CHECK=true
CMD /bin/bash ./run.sh
