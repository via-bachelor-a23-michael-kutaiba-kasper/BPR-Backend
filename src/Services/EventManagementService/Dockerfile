FROM mcr.microsoft.com/dotnet/sdk:7.0 as build-env
WORKDIR /src
COPY . .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /publish
RUN mv /src/Scripts/entrypoint.sh /publish/entrypoint.sh

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime
WORKDIR /app
COPY --from=build-env /publish /app/publish
WORKDIR /app/publish
EXPOSE 80

CMD [ "sh", "entrypoint.sh" ]
