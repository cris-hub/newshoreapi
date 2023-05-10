#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NEWSHORE.API/NEWSHORE.API.csproj", "NEWSHORE.API/"]
COPY ["NEWSHORE.IoC/NEWSHORE.IoC.csproj", "NEWSHORE.IoC/"]
COPY ["NEWSHORE.RepositoryEF/NEWSHORE.RepositoryEF.csproj", "NEWSHORE.RepositoryEF/"]
COPY ["NEWSHORE.DTOs/NEWSHORE.DTOs.csproj", "NEWSHORE.DTOs/"]
COPY ["NEWSHORE.Entities/NEWSHORE.Entities.csproj", "NEWSHORE.Entities/"]
COPY ["NEWSHORE.Presenter/NEWSHORE.Presenter.csproj", "NEWSHORE.Presenter/"]
COPY ["NEWSHORE.UseCasesPorts/NEWSHORE.UseCasesPorts.csproj", "NEWSHORE.UseCasesPorts/"]
COPY ["NEWSHORE.Controllers/NEWSHORE.Controllers.csproj", "NEWSHORE.Controllers/"]
COPY ["NEWSHORE.UseCases/NEWSHORE.UseCases.csproj", "NEWSHORE.UseCases/"]
RUN dotnet restore "NEWSHORE.API/NEWSHORE.API.csproj"
COPY . .
WORKDIR "/src/NEWSHORE.API"
RUN dotnet build "NEWSHORE.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NEWSHORE.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NEWSHORE.API.dll"]