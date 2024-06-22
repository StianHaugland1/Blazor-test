FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BlazorIntAuto/BlazorIntAuto.csproj", "BlazorIntAuto/"]
COPY ["BlazorIntAuto.Client/BlazorIntAuto.Client.csproj", "BlazorIntAuto.Client/"]
RUN dotnet restore "BlazorIntAuto/BlazorIntAuto.csproj"
COPY . .
WORKDIR "/src/BlazorIntAuto"
RUN dotnet build "BlazorIntAuto.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BlazorIntAuto.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorIntAuto.dll"]
