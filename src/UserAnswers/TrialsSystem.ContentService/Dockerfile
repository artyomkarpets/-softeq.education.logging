FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build


ARG TOKEN
WORKDIR /src

COPY ["UserAnswers/TrialsSystem.ContentService/TrialsSystem.ContentService.Api/TrialsSystem.ContentService.Api.csproj", "UserAnswers/TrialsSystem.ContentService/TrialsSystem.ContentService.Api/"]
COPY ["UserAnswers/TrialsSystem.ContentService/TrialsSystem.ContentService.sln", "UserAnswers/TrialsSystem.ContentService/"]
COPY ["UserAnswers/TrialsSystem.ContentService/nuget.config", "UserAnswers/TrialsSystem.ContentService/"]

RUN dotnet restore "UserAnswers/TrialsSystem.ContentService/TrialsSystem.ContentService.sln"
COPY . .

WORKDIR UserAnswers/TrialsSystem.ContentService/TrialsSystem.ContentService.Api
RUN dotnet publish --no-restore -c Release -o /app/publish

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "TrialsSystem.ContentService.Api.dll"]