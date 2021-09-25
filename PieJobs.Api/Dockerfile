FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["../BuzzCi.Api/BuzzCi.Api/BuzzCi.Api.csproj", "BuzzCi.Api/"]
RUN dotnet restore "../BuzzCi.Api/BuzzCi.Api/BuzzCi.Api.csproj"
COPY . .
WORKDIR "/src/BuzzCi.Api"
RUN dotnet build "BuzzCi.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BuzzCi.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BuzzCi.Api.dll"]
