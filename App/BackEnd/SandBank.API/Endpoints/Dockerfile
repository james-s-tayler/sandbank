FROM mcr.microsoft.com/dotnet/core/aspnet:2.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build
WORKDIR /app
COPY . .
WORKDIR /app/App/BackEnd/SandBank.API/Endpoints
RUN dotnet restore -nowarn:msb3202,nu1503
RUN dotnet build --no-restore -c Release -o output

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o output

FROM base AS final
WORKDIR /app
COPY --from=publish /app/App/BackEnd/SandBank.API/Endpoints/output .
ENTRYPOINT ["dotnet", "Endpoints.dll"]
