FROM mcr.microsoft.com/dotnet/sdk:7.0 as build-env
WORKDIR /src
COPY . .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime
WORKDIR /app
COPY --from=build-env /publish .

ARG SERVICE_ACCOUNT_KEY=NOTHING

EXPOSE 80
ENV SERVICE_ACCOUNT_KEY_JSON=${SERVICE_ACCOUNT_KEY}

ENTRYPOINT ["dotnet"]
CMD ["EventManagementService.API.dll"]