# Build: restore first (cached while only code changes), then publish the API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY global.json Directory.Build.props Directory.Packages.props ./
COPY SwimSchool.Domain/SwimSchool.Domain.csproj SwimSchool.Domain/
COPY SwimSchool.Application/SwimSchool.Application.csproj SwimSchool.Application/
COPY SwimSchool.Infrastructure/SwimSchool.Infrastructure.csproj SwimSchool.Infrastructure/
COPY SwimSchool.API/SwimSchool.API.csproj SwimSchool.API/
RUN dotnet restore SwimSchool.API/SwimSchool.API.csproj
COPY . .
RUN dotnet publish SwimSchool.API/SwimSchool.API.csproj -c Release -o /app --no-restore

# Run: Debian x64 (not Alpine) so QuestPDF's native library works
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "SwimSchool.API.dll"]
