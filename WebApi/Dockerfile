FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

EXPOSE 80
EXPOSE 443

WORKDIR /src

# copy csproj and restore as distinct layers
COPY ["*.sln", "./"]
COPY ["Domain/*.csproj", "Domain/"]
COPY ["Application/*.csproj", "Application/"]
COPY ["Infrastructure/*.csproj", "Infrastructure/"]
COPY ["WebApi/*.csproj", "WebApi/"]
RUN dotnet restore

# copy everything else and build app
COPY ["Domain/*", "Domain/"]
COPY ["Application/*", "Application/"]
COPY ["Infrastructure/*", "Infrastructure/"]
COPY ["WebApi/*", "WebApi/"]

WORKDIR "/src/WebApi"

FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]
