FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# copy and publish app and libraries
COPY . ./
RUN dotnet publish -c Debug -o .

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app .
EXPOSE 5001
ENTRYPOINT ["dotnet", "webapp.dll", "--urls=http://127.0.0.1:5001/"]