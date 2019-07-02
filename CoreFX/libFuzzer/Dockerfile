FROM mcr.microsoft.com/dotnet/core/sdk:3.0
WORKDIR /app

ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global SharpFuzz.CommandLine

RUN wget https://github.com/Metalnem/libfuzzer-dotnet/releases/latest/download/libfuzzer-dotnet.tar.gz
RUN tar -xvf libfuzzer-dotnet.tar.gz

COPY CoreFX.Fuzz/CoreFX.Fuzz.csproj CoreFX.Fuzz/NuGet.Config ./
RUN dotnet restore -r linux-x64

COPY CoreFX.Fuzz/Program.cs ./
RUN dotnet publish -r linux-x64 --no-restore -c release -o out

COPY LibFuzzer/run.sh ./
COPY Dictionaries ./dictionaries/
COPY Testcases ./testcases/

CMD ./run.sh
