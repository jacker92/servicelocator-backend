FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY *.sln .
COPY ServiceLocatorBackend.Tests/*.csproj ServiceLocatorBackend.Tests/
COPY ServiceLocatorBackend/*.csproj ServiceLocatorBackend/
RUN dotnet restore
COPY . .

# Tests
FROM build AS testing
WORKDIR /src/ServiceLocatorBackend
RUN dotnet build
WORKDIR /src/ServiceLocatorBackend.Tests
RUN dotnet test || exit 1

FROM build AS publish
WORKDIR /src/ServiceLocatorBackend
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet ServiceLocatorBackend.dll