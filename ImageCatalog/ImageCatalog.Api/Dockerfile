#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ImageCatalog/ImageCatalog.Api/ImageCatalog.Api.csproj", "ImageCatalog/ImageCatalog.Api/"]
COPY ["GardenLog.SharedInfrastructure/GardenLog.SharedInfrastructure.csproj", "GardenLog.SharedInfrastructure/"]
COPY ["GardenLog.SharedKernel/GardenLog.SharedKernel.csproj", "GardenLog.SharedKernel/"]
COPY ["ImageCatalog/ImageCatalog.Contract/ImageCatalog.Contract.csproj", "ImageCatalog/ImageCatalog.Contract/"]
RUN dotnet restore "ImageCatalog/ImageCatalog.Api/ImageCatalog.Api.csproj"
COPY . .
WORKDIR "/src/ImageCatalog/ImageCatalog.Api"
RUN dotnet build "ImageCatalog.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ImageCatalog.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ImageCatalog.Api.dll"]