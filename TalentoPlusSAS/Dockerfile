FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


COPY ["TalentoPlusSAS.Api/TalentoPlusSAS.Api.csproj", "TalentoPlusSAS.Api/"]
COPY ["TalentoPlusSAS.Application/TalentoPlusSAS.Application.csproj", "TalentoPlusSAS.Application/"]
COPY ["TalentoPlusSAS.Domain/TalentoPlusSAS.Domain.csproj", "TalentoPlusSAS.Domain/"]
COPY ["TalentoPlusSAS.Infrastructure/TalentoPlusSAS.Infrastructure.csproj", "TalentoPlusSAS.Infrastructure/"]

RUN dotnet restore "TalentoPlusSAS.Api/TalentoPlusSAS.Api.csproj"

COPY . .
WORKDIR "/src/TalentoPlusSAS.Api"

RUN dotnet build "TalentoPlusSAS.Api.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "TalentoPlusSAS.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y libfontconfig1 fontconfig

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TalentoPlusSAS.Api.dll"]