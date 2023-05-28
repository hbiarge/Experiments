#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0.5-bullseye-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk: 7.0.302-bullseye-slim AS build
WORKDIR /src
COPY ["Notifications/Notifications.csproj", "Notifications/"]
RUN dotnet restore "Notifications/Notifications.csproj"
COPY . .
WORKDIR "/src/Notifications"
RUN dotnet build "Notifications.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Notifications.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notifications.dll"]