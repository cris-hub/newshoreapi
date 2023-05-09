FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release  -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

COPY --from=build /app .

ENV ASPNETCORE_URLS http://*:$PORT
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet","NEWSHORE.API.dll"]
