# command to run from Backend Dir - docker build -t gamestore-api:1.0 .
# build stage - pruces published directory by publishing (compiling + packaging with dotnet sdk 10)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build         
WORKDIR /build
COPY . . 
RUN dotnet publish ./src/GameStore.Api -o published /p:UseAppHost=false

# run stage - making runnable docker image with aspnet 10
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /build/published .
ENTRYPOINT [ "dotnet", "GameStore.Api.dll" ]