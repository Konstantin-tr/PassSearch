#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["PasswordQueryTool.ParseWebApp/Server/PasswordQueryTool.ParseWebApp.Server.csproj", "PasswordQueryTool.ParseWebApp/Server/"]
COPY ["PasswordQueryTool.ParseWebApp/Client/PasswordQueryTool.ParseWebApp.Client.csproj", "PasswordQueryTool.ParseWebApp/Client/"]
COPY ["PasswordQueryTool.ImportModels/PasswordQueryTool.Parsing.Common.csproj", "PasswordQueryTool.ImportModels/"]
RUN dotnet restore "PasswordQueryTool.ParseWebApp/Server/PasswordQueryTool.ParseWebApp.Server.csproj"
COPY . .
WORKDIR "/src/PasswordQueryTool.ParseWebApp/Server"
RUN dotnet build "PasswordQueryTool.ParseWebApp.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PasswordQueryTool.ParseWebApp.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PasswordQueryTool.ParseWebApp.Server.dll"]
