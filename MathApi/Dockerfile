# ── Build ─────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-self-contained

# ── Runtime ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Render injects PORT; fall back to 5000 locally
ENV PORT=5000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE $PORT
ENTRYPOINT ["dotnet", "MathApi.dll"]
