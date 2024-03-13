FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
COPY . .
WORKDIR "/app/src/business-logic/AS.Api"
RUN dotnet publish "AS.Api.csproj" -o /app/build -c Release

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Development
ENV TZ="Asia/Tashkent"
COPY --from=build-env /app/build .
ENTRYPOINT ["dotnet", "AS.Api.dll", "--urls=http://0.0.0.0:10001"]