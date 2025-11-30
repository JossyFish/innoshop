FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["backend/Auth.Core/Auth.Core.csproj", "backend/Auth.Core/"]
COPY ["backend/UserService/UserService.API/UserService.API.csproj", "backend/UserService/UserService.API/"]
COPY ["backend/UserService/UserService.Application/UserService.Application.csproj", "backend/UserService/UserService.Application/"]
COPY ["backend/UserService/UserService.Domain/UserService.Domain.csproj", "backend/UserService/UserService.Domain/"]
COPY ["backend/UserService/UserService.Infrastructure/UserService.Infrastructure.csproj", "backend/UserService/UserService.Infrastructure/"]

RUN dotnet restore "backend/UserService/UserService.API/UserService.API.csproj"

COPY backend/ ./backend/

COPY ["backend/Auth.Core/config.json", "backend/Auth.Core/"]

WORKDIR "/src/backend/UserService/UserService.API"
RUN dotnet build "UserService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UserService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "UserService.API.dll"]