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
COPY ["Services/Identity/Surveillance.Identity.Api/Surveillance.Identity.Api.csproj", "Services/Identity/Surveillance.Identity.Api/"]
COPY ["Services/Identity/Surveillance.Identity.Application/Surveillance.Identity.Application.csproj", "Services/Identity/Surveillance.Identity.Application/"]
COPY ["Services/Identity/Surveillance.Identity.Domain/Surveillance.Identity.Domain.csproj", "Services/Identity/Surveillance.Identity.Domain/"]
COPY ["Services/Identity/Surveillance.Identity.Infrastructure/Surveillance.Identity.Infrastructure.csproj", "Services/Identity/Surveillance.Identity.Infrastructure/"]
RUN dotnet restore "./Services/Identity/Surveillance.Identity.Api/Surveillance.Identity.Api.csproj"
COPY . .
WORKDIR "/src/Services/Identity/Surveillance.Identity.Api"
RUN dotnet build "./Surveillance.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Surveillance.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Surveillance.Identity.Api.dll"]