# SDK
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /App

COPY *.sln ./
COPY ./Src ./Src
RUN ["dotnet", "restore"]

RUN dotnet publish -c Release -o Outdir "./Src/JWTAuthServer.API/JWTAuthServer.API.csproj"

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App

COPY --from=build /App/Outdir .

ENTRYPOINT ["dotnet", "JWTAuthServer.API.dll"]
