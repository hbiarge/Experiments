#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk: 7.0.302-bullseye-slim AS build
WORKDIR /src
COPY ["ProcessManager/ProcessManager.csproj", "ProcessManager/"]
RUN dotnet restore "ProcessManager/ProcessManager.csproj"
COPY . .
WORKDIR "/src/ProcessManager"
RUN dotnet build "ProcessManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProcessManager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProcessManager.dll"]