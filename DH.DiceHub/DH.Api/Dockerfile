# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DH.Api/DH.Api.csproj", "DH.Api/"]
COPY ["DH.Adapter.Authentication/DH.Adapter.Authentication.csproj", "DH.Adapter.Authentication/"]
COPY ["DH.Domain/DH.Domain.csproj", "DH.Domain/"]
COPY ["DH.OperationResult/DH.OperationResultCore.csproj", "DH.OperationResult/"]
COPY ["DH.Adapter.ChallengesOrchestrator/DH.Adapter.ChallengesOrchestrator.csproj", "DH.Adapter.ChallengesOrchestrator/"]
COPY ["DH.Adapter.ChatHub/DH.Adapter.ChatHub.csproj", "DH.Adapter.ChatHub/"]
COPY ["DH.Adapter.ChallengeHub/DH.Adapter.ChallengeHub.csproj", "DH.Adapter.ChallengeHub/"]
COPY ["DH.Adapter.Localization/DH.Adapter.Localization.csproj", "DH.Adapter.Localization/"]
COPY ["DH.Adapter.Data/DH.Adapter.Data.csproj", "DH.Adapter.Data/"]
COPY ["DH.Adapter.Email/DH.Adapter.Email.csproj", "DH.Adapter.Email/"]
COPY ["DH.Adapter.GameSession/DH.Adapter.GameSession.csproj", "DH.Adapter.GameSession/"]
COPY ["DH.Adapter.PushNotifications/DH.Adapter.PushNotifications.csproj", "DH.Adapter.PushNotifications/"]
COPY ["DH.Adapter.QRManager/DH.Adapter.QRManager.csproj", "DH.Adapter.QRManager/"]
COPY ["DH.Adapter.Reservations/DH.Adapter.Reservations.csproj", "DH.Adapter.Reservations/"]
COPY ["DH.Adapter.Scheduling/DH.Adapter.Scheduling.csproj", "DH.Adapter.Scheduling/"]
COPY ["DH.Adapter.Statistics/DH.Adapter.Statistics.csproj", "DH.Adapter.Statistics/"]
COPY ["DH.Application/DH.Application.csproj", "DH.Application/"]
RUN dotnet restore "./DH.Api/DH.Api.csproj"
COPY . .
WORKDIR "/src/DH.Api"
RUN dotnet build "./DH.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DH.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DH.Api.dll"]