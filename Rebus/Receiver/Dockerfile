FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Receiver/Receiver.csproj Receiver/
RUN dotnet restore Receiver/Receiver.csproj
COPY . .
WORKDIR /src/Receiver
RUN dotnet build Receiver.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Receiver.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Receiver.dll"]
