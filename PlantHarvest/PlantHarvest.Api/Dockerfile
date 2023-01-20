#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PlantHarvest/PlantHarvest.Api/PlantHarvest.Api.csproj", "PlantHarvest/PlantHarvest.Api/"]
COPY ["GardenLog.SharedInfrastructure/GardenLog.SharedInfrastructure.csproj", "GardenLog.SharedInfrastructure/"]
COPY ["GardenLog.SharedKernel/GardenLog.SharedKernel.csproj", "GardenLog.SharedKernel/"]
COPY ["PlantHarvest/PlantHarvest.Contract/PlantHarvest.Contract.csproj", "PlantHarvest/PlantHarvest.Contract/"]
COPY ["PlantHarvest/PlantHarvest.Domain/PlantHarvest.Domain.csproj", "PlantHarvest/PlantHarvest.Domain/"]
COPY ["PlantHarvest/PlantHarvest.Infrastructure/PlantHarvest.Infrastructure.csproj", "PlantHarvest/PlantHarvest.Infrastructure/"]
RUN dotnet restore "PlantHarvest/PlantHarvest.Api/PlantHarvest.Api.csproj"
COPY . .
WORKDIR "/src/PlantHarvest/PlantHarvest.Api"
RUN dotnet build "PlantHarvest.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlantHarvest.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlantHarvest.Api.dll"]