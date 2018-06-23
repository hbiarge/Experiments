FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Sender/Sender.csproj Sender/
RUN dotnet restore Sender/Sender.csproj
COPY . .
WORKDIR /src/Sender
RUN dotnet build Sender.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Sender.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Sender.dll"]
