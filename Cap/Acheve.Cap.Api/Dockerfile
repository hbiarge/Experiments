#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Acheve.Cap.Api/Acheve.Cap.Api.csproj", "Acheve.Cap.Api/"]
RUN dotnet restore "Acheve.Cap.Api/Acheve.Cap.Api.csproj"
COPY . .
WORKDIR "/src/Acheve.Cap.Api"
RUN dotnet build "Acheve.Cap.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Acheve.Cap.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Acheve.Cap.Api.dll"]