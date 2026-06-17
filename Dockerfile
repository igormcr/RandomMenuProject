FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy just the project file first (for better caching)
COPY RandomMenuProject/RandomMenuProject.csproj RandomMenuProject/
RUN dotnet restore RandomMenuProject/RandomMenuProject.csproj

# Copy everything else
COPY . .

# Build and publish
WORKDIR /app/RandomMenuProject
RUN dotnet publish -c Release -o /out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Railway sets PORT environment variable
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "RandomMenuProject.dll"]