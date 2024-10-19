#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SomethingFishy.Collabothon2024.API/SomethingFishy.Collabothon2024.API.csproj", "SomethingFishy.Collabothon2024.API/"]
COPY ["SomethingFishy.Collabothon2024.Common/SomethingFishy.Collabothon2024.Common.csproj", "SomethingFishy.Collabothon2024.Common/"]
RUN dotnet restore "./SomethingFishy.Collabothon2024.API/SomethingFishy.Collabothon2024.API.csproj"
COPY . .
WORKDIR "/src/SomethingFishy.Collabothon2024.API"
RUN dotnet build "./SomethingFishy.Collabothon2024.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SomethingFishy.Collabothon2024.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SomethingFishy.Collabothon2024.API.dll"]