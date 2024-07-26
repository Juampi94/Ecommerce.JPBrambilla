 #Esto setea que imagen se va a usar, y un nombre de variable
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src
COPY . .
WORKDIR /src/Pacagoup.Ecommerce.Services.WebApi
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build-env /src/Pacagoup.Ecommerce.Services.WebApi/out ./

ENTRYPOINT ["dotnet", "Pacagoup.Ecommerce.Services.WebApi.dll"]