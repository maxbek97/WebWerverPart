FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY . .
RUN dotnet restore "WebWerverPart.csproj"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 as final
WORKDIR /app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebWerverPart.dll"]
