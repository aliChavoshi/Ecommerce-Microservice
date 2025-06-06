# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
#Expose 443

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/discount/Discount.API/Discount.API.csproj", "Services/discount/Discount.API/"]
COPY ["Services/discount/Discount.Application/Discount.Application.csproj", "Services/discount/Discount.Application/"]
COPY ["Services/discount/Discount.Core/Discount.Core.csproj", "Services/discount/Discount.Core/"]
COPY ["Services/discount/Discount.Infrastructure/Discount.Infrastructure.csproj", "Services/discount/Discount.Infrastructure/"]
COPY ["Infrastructure/Common.Logging/Common.Logging.csproj","Infrastructure/Common.Logging/"]
RUN dotnet restore "./Services/discount/Discount.API/Discount.API.csproj"
COPY . .
WORKDIR "/src/Services/discount/Discount.API"
RUN dotnet build "./Discount.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Discount.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Discount.API.dll"]