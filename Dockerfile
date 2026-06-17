# Use .NET 8 SDK instead of 9
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and project files
COPY ["RandomMenuProject.sln", "./"]
COPY ["RandomMenuProject/RandomMenuProject.csproj", "RandomMenuProject/"]

# Restore dependencies
RUN dotnet restore "RandomMenuProject/RandomMenuProject.csproj"

# Copy all source code
COPY . .

# Build and publish the project
WORKDIR "/src/RandomMenuProject"
RUN dotnet build "RandomMenuProject.csproj" -c Release -o /app/build
RUN dotnet publish "RandomMenuProject.csproj" -c Release -o /app/publish

# Use .NET 8 runtime instead of 9
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy the published output
COPY --from=build /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "RandomMenuProject.dll"]