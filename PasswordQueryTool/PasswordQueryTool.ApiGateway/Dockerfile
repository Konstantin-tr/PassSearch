#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["PasswordQueryTool.ApiGateway/PasswordQueryTool.ApiGateway.csproj", "PasswordQueryTool.ApiGateway/"]
RUN dotnet restore "PasswordQueryTool.ApiGateway/PasswordQueryTool.ApiGateway.csproj"
COPY . .
WORKDIR "/src/PasswordQueryTool.ApiGateway"
RUN dotnet build "PasswordQueryTool.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PasswordQueryTool.ApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PasswordQueryTool.ApiGateway.dll"]