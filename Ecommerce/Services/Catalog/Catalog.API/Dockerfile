# Base image for runtime (used by final stage)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER root
RUN apt-get update && apt-get install -y curl
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# Stage: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# کپی پروژه‌ها
COPY ["Services/Catalog/Catalog.API/Catalog.API.csproj", "Services/Catalog/Catalog.API/"]
COPY ["Services/Catalog/Catalog.Application/Catalog.Application.csproj", "Services/Catalog/Catalog.Application/"]
COPY ["Services/Catalog/Catalog.Core/Catalog.Core.csproj", "Services/Catalog/Catalog.Core/"]
COPY ["Services/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj", "Services/Catalog/Catalog.Infrastructure/"]
COPY ["Infrastructure/Common.Logging/Common.Logging.csproj", "Infrastructure/Common.Logging/"]

# اضافه‌کردن گواهی برای مرحله build nginx
COPY id-local.eshopping.com.crt /usr/local/share/ca-certificates/
RUN update-ca-certificates

# بازیابی وابستگی‌ها
RUN dotnet restore "./Services/Catalog/Catalog.API/Catalog.API.csproj"

# کپی کامل سورس
COPY . .

# ساخت برنامه
WORKDIR "/src/Services/Catalog/Catalog.API"
RUN dotnet build "./Catalog.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage: publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Catalog.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage: final (برای اجرای واقعی برنامه)
FROM base AS final
WORKDIR /app

# اضافه‌کردن گواهی برای مرحله نهایی nginx
COPY id-local.eshopping.com.crt /usr/local/share/ca-certificates/
RUN update-ca-certificates

# کپی خروجی منتشر شده
COPY --from=publish /app/publish .

# اجرای برنامه
ENTRYPOINT ["dotnet", "Catalog.API.dll"]
