# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and project files
COPY QuickPass.sln ./
COPY src/QuickPass.Domain/QuickPass.Domain.csproj src/QuickPass.Domain/
COPY src/QuickPass.Application/QuickPass.Application.csproj src/QuickPass.Application/
COPY src/QuickPass.Infrastructure/QuickPass.Infrastructure.csproj src/QuickPass.Infrastructure/
COPY src/QuickPass.API/QuickPass.API.csproj src/QuickPass.API/
COPY test/QuickPass.UnitTests/QuickPass.UnitTests.csproj test/QuickPass.UnitTests/

# Restore dependencies
RUN dotnet restore

# Copy all source files
COPY src/ src/
COPY test/ test/

# Build and run unit tests
RUN dotnet test test/QuickPass.UnitTests/QuickPass.UnitTests.csproj --configuration Release
RUN dotnet publish src/QuickPass.API/QuickPass.API.csproj -c Release -o /out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "QuickPass.API.dll"]
