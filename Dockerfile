# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj correctly
COPY TravelExpress_MVC/TravelExpress.csproj ./TravelExpress_MVC/
RUN dotnet restore ./TravelExpress_MVC/TravelExpress.csproj

# copy everything else
COPY . .
WORKDIR /src/TravelExpress_MVC
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "TravelExpress.dll"]


