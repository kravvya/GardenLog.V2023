#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PlantCatalog.Api/PlantCatalog.Api.csproj", "PlantCatalog.Api/"]
RUN dotnet restore "PlantCatalog.Api/PlantCatalog.Api.csproj"
COPY . .
WORKDIR "/src/PlantCatalog.Api"
RUN dotnet build "PlantCatalog.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlantCatalog.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlantCatalog.Api.dll"]