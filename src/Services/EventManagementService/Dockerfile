FROM mcr.microsoft.com/dotnet/sdk:7.0 as build-env
WORKDIR /src
COPY . .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime
WORKDIR /app
COPY --from=build-env /publish .
WORKDIR /publish
EXPOSE 80

CMD ["dotnet", "EventManagementService.DbUp.dll", "&&", "dotnet", "EventManagementService.API.dll"]
