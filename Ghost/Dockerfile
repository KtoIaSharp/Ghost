FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем и восстанавливаем пакеты
COPY *.csproj ./
RUN dotnet restore

# Копируем остальной код
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime этап
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .

# Render использует порт 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Ghost.dll"]