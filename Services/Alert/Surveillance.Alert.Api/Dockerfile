# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Alert/Surveillance.Alert.Api/Surveillance.Alert.Api.csproj", "Services/Alert/Surveillance.Alert.Api/"]
COPY ["Services/Alert/Surveillance.Alert.Application/Surveillance.Alert.Application.csproj", "Services/Alert/Surveillance.Alert.Application/"]
COPY ["BuildingBlocks/Surveillance.EventBus/Surveillance.EventBus.csproj", "BuildingBlocks/Surveillance.EventBus/"]
COPY ["BuildingBlocks/Surveillance.Saga/Surveillance.Saga.csproj", "BuildingBlocks/Surveillance.Saga/"]
COPY ["Services/Alert/Surveillance.Alert.Domain/Surveillance.Alert.Domain.csproj", "Services/Alert/Surveillance.Alert.Domain/"]
COPY ["BuildingBlocks/Surveillance.SharedKernel/Surveillance.SharedKernel.csproj", "BuildingBlocks/Surveillance.SharedKernel/"]
COPY ["Services/Alert/Surveillance.Alert.Infrastructure/Surveillance.Alert.Infrastructure.csproj", "Services/Alert/Surveillance.Alert.Infrastructure/"]
COPY ["BuildingBlocks/Surveillance.Logging/Surveillance.Logging.csproj", "BuildingBlocks/Surveillance.Logging/"]
RUN dotnet restore "./Services/Alert/Surveillance.Alert.Api/Surveillance.Alert.Api.csproj"
COPY . .
WORKDIR "/src/Services/Alert/Surveillance.Alert.Api"
RUN dotnet build "./Surveillance.Alert.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Surveillance.Alert.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Surveillance.Alert.Api.dll"]