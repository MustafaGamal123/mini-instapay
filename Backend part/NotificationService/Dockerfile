FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NotifcationAPI/NotifcationAPI.csproj", "NotifcationAPI/"]
COPY ["BusinessLayer/BusinessLayer.csproj", "BusinessLayer/"]
COPY ["DbAccessLayer/DbAccessLayer.csproj", "DbAccessLayer/"]
RUN dotnet restore "NotifcationAPI/NotifcationAPI.csproj"
COPY . .
WORKDIR "/src/NotifcationAPI"
RUN dotnet build "NotifcationAPI.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "NotifcationAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotifcationAPI.dll"]
