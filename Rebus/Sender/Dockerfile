FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
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
