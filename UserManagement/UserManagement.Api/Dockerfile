#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["UserManagement/UserManagement.Api/UserManagement.Api.csproj", "UserManagement/UserManagement.Api/"]
COPY ["GardenLog.SharedInfrastructure/GardenLog.SharedInfrastructure.csproj", "GardenLog.SharedInfrastructure/"]
COPY ["GardenLog.SharedKernel/GardenLog.SharedKernel.csproj", "GardenLog.SharedKernel/"]
COPY ["UserManagement/UserManagement.Contract/UserManagement.Contract.csproj", "UserManagement/UserManagement.Contract/"]
RUN dotnet restore "UserManagement/UserManagement.Api/UserManagement.Api.csproj"
COPY . .
WORKDIR "/src/UserManagement/UserManagement.Api"
RUN dotnet build "UserManagement.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserManagement.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserManagement.Api.dll"]