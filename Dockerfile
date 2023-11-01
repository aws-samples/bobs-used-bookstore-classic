FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS build
WORKDIR /app

COPY . ./

RUN nuget restore

RUN msbuild app/Bookstore.Web/Bookstore.Web.csproj /p:DeployOnBuild=true /p:PublishProfile=FolderProfile.pubxml

FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019 AS runtime

WORKDIR /LogMonitor
RUN Invoke-WebRequest -Uri "https://github.com/microsoft/windows-container-tools/releases/download/v2.0.2/LogMonitor.exe" -OutFile "LogMonitor.exe"
COPY LogMonitorConfig.json .

WORKDIR /inetpub/wwwroot

COPY --from=build /app/app/Bookstore.Web/obj/Docker/publish/ .

ENTRYPOINT ["C:\\LogMonitor\\LogMonitor.exe", "C:\\ServiceMonitor.exe", "w3svc"]