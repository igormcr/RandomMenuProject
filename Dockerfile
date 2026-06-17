FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the project file from the subfolder
COPY RandomMenuProject/*.csproj ./RandomMenuProject/
RUN dotnet restore ./RandomMenuProject/RandomMenuProject.csproj

# Copy everything and build
COPY . ./
RUN dotnet publish ./RandomMenuProject/RandomMenuProject.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "RandomMenuProject.dll"]