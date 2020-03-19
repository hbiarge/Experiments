FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
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
