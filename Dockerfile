FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release  -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

COPY --from=build /app .

RUN echo "ASPNETCORE_URLS=http://0.0.0.0:\$PORT\nDOTNET_RUNNING_IN_CONTAINER=true" > /app/setup_heroku_env.sh && chmod +x /app/setup_heroku_env.sh

CMD /bin/bash -c "source /app/setup_heroku_env.sh && dotnet NEWSHORE.API.dll"
