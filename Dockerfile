FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["LibreriaDonCesar.APIVI/LibreriaDonCesar.API/LibreriaDonCesar.API.csproj", "LibreriaDonCesar.APIVI/LibreriaDonCesar.API/"]
COPY ["LibreriaDonCesar.APIVI/LibreriaDonCesar.API.Business/LibreriaDonCesar.API.Business.csproj", "LibreriaDonCesar.APIVI/LibreriaDonCesar.API.Business/"]
COPY ["LibreriaDonCesar.APIVI/LibreriaDonCesar.API.Core/LibreriaDonCesar.API.Core.csproj", "LibreriaDonCesar.APIVI/LibreriaDonCesar.API.Core/"]
COPY ["LibreriaDonCesar.APIVI/LibreriaDonCesar.API.DataAccess/LibreriaDonCesar.API.DataAccess.csproj", "LibreriaDonCesar.APIVI/LibreriaDonCesar.API.DataAccess/"]

RUN dotnet restore "LibreriaDonCesar.APIVI/LibreriaDonCesar.API/LibreriaDonCesar.API.csproj"

COPY . .

WORKDIR "/src/LibreriaDonCesar.APIVI/LibreriaDonCesar.API"

RUN dotnet publish "LibreriaDonCesar.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "LibreriaDonCesar.API.dll"]