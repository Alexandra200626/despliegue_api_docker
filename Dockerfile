
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["LibreriaDonCesar.API/LibreriaDonCesar.API.csproj", "LibreriaDonCesar.API/"]
COPY ["LibreriaDonCesar.Business/LibreriaDonCesar.Business.csproj", "LibreriaDonCesar.Business/"]
COPY ["LibreríaDonCesar.Core/LibreriaDonCesar.Core.csproj", "LibreríaDonCesar.Core/"]
COPY ["LibreriaDonCesar.DataAccess/LibreriaDonCesar.DataAccess.csproj", "LibreriaDonCesar.DataAccess/"]

RUN dotnet restore "LibreriaDonCesar.API/LibreriaDonCesar.API.csproj"

COPY . .

WORKDIR "/src/LibreriaDonCesar.API"

RUN dotnet publish "LibreriaDonCesar.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "LibreriaDonCesar.API.dll"]
