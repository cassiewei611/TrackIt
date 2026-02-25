FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/TrackIt.API/TrackIt.API.csproj", "src/TrackIt.API/"]
COPY ["src/TrackIt.Application/TrackIt.Application.csproj", "src/TrackIt.Application/"]
COPY ["src/TrackIt.Infrastructure/TrackIt.Infrastructure.csproj", "src/TrackIt.Infrastructure/"]
COPY ["src/TrackIt.Domain/TrackIt.Domain.csproj", "src/TrackIt.Domain/"]
RUN dotnet restore "src/TrackIt.API/TrackIt.API.csproj"
COPY . .
WORKDIR "/src/src/TrackIt.API"
RUN dotnet build "TrackIt.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrackIt.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrackIt.API.dll"]
