FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["backend/Auth.Core/Auth.Core.csproj", "backend/Auth.Core/"]
COPY ["backend/ProductService/ProductService.API/ProductService.API.csproj", "backend/ProductService/ProductService.API/"]
COPY ["backend/ProductService/ProductService.Application/ProductService.Application.csproj", "backend/ProductService/ProductService.Application/"]
COPY ["backend/ProductService/ProductService.Domain/ProductService.Domain.csproj", "backend/ProductService/ProductService.Domain/"]
COPY ["backend/ProductService/ProductService.Infrastructure/ProductService.Infrastructure.csproj", "backend/ProductService/ProductService.Infrastructure/"]

RUN dotnet restore "backend/ProductService/ProductService.API/ProductService.API.csproj"

COPY backend/ ./backend/

COPY ["backend/Auth.Core/config.json", "backend/Auth.Core/"]

WORKDIR "/src/backend/ProductService/ProductService.API"
RUN dotnet build "ProductService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProductService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ProductService.API.dll"]