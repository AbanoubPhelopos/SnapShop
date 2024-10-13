FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SnapShop.Core/SnapShop.Core.csproj", "SnapShop.Core/"]
COPY ["SnapShop.Application/SnapShop.Application.csproj", "SnapShop.Application/"]
COPY ["SnapShop.Utility/SnapShop.Utility.csproj", "SnapShop.Utility/"]
RUN dotnet restore "SnapShop.Core/SnapShop.Core.csproj"
COPY . .
WORKDIR "/src/SnapShop.Core"
RUN dotnet build "SnapShop.Core.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SnapShop.Core.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SnapShop.Core.dll"]
