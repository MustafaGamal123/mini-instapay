# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["RepotingService/RepotingService.csproj", "RepotingService/"]
COPY ["BusinessLayer/BusinessLayer.csproj", "BusinessLayer/"]
COPY ["DbAccessLayer/DbAccessLayer.csproj", "DbAccessLayer/"]

# Restore dependencies
RUN dotnet restore "RepotingService/RepotingService.csproj"

# Copy the rest of the source code
COPY . .

# Set working directory to the main project and build it
WORKDIR "/src/RepotingService"
RUN dotnet build "RepotingService.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "RepotingService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RepotingService.dll"]
