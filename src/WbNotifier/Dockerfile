FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WbNotifier.csproj", "WbNotifier/"]
RUN dotnet restore "WbNotifier/WbNotifier.csproj"
WORKDIR "/src/WbNotifier"
COPY . .
RUN dotnet build "WbNotifier.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WbNotifier.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WbNotifier.dll"]