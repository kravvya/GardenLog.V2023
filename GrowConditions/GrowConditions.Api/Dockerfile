#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["GrowConditions/GrowConditions.Api/GrowConditions.Api.csproj", "GrowConditions/GrowConditions.Api/"]
COPY ["GardenLog.SharedKernel/GardenLog.SharedKernel.csproj", "GardenLog.SharedKernel/"]
COPY ["UserManagement/UserManagement.Contract/UserManagement.Contract.csproj", "UserManagement/UserManagement.Contract/"]
COPY ["GardenLog.SharedInfrastructure/GardenLog.SharedInfrastructure.csproj", "GardenLog.SharedInfrastructure/"]
COPY ["GrowConditions/GrowConditions.Contract/GrowConditions.Contract.csproj", "GrowConditions/GrowConditions.Contract/"]
RUN dotnet restore "GrowConditions/GrowConditions.Api/GrowConditions.Api.csproj"
COPY . .
WORKDIR "/src/GrowConditions/GrowConditions.Api"
RUN dotnet build "GrowConditions.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GrowConditions.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GrowConditions.Api.dll"]