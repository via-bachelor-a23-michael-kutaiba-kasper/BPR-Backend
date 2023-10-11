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

RUN echo ${SERVICE_ACCOUNT_KEY} > /app/service_account_key.json

EXPOSE 80
ENV GOOGLE_APPLICATION_CREDENTIALS=/app/service_acount_key.json

ENTRYPOINT ["dotnet"]
CMD ["EventManagementService.API.dll"]