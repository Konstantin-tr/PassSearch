#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["../PasswordQueryTool.WebApp/Server/PasswordQueryTool.WebApp.Server.csproj", "../PasswordQueryTool.WebApp/Server/"]
COPY ["../PasswordQueryTool.WebApp/Client/PasswordQueryTool.WebApp.Client.csproj", "../PasswordQueryTool.WebApp/Client/"]
COPY ["PasswordQueryTool.Model/PasswordQueryTool.Model.csproj", "PasswordQueryTool.Model/"]
COPY ["../PasswordQueryTool.WebApp/Shared/PasswordQueryTool.WebApp.Shared.csproj", "../PasswordQueryTool.WebApp/Shared/"]
RUN dotnet restore "../PasswordQueryTool.WebApp/Server/PasswordQueryTool.WebApp.Server.csproj"
COPY . .
WORKDIR "/src/../PasswordQueryTool.WebApp/Server"
RUN dotnet build "PasswordQueryTool.WebApp.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PasswordQueryTool.WebApp.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PasswordQueryTool.WebApp.Server.dll"]