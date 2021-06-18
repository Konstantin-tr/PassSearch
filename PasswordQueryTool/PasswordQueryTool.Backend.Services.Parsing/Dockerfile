#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["PasswordQueryTool.Backend.Services.Parsing/PasswordQueryTool.Backend.Services.Parsing.csproj", "PasswordQueryTool.Backend.Services.Parsing/"]
RUN dotnet restore "PasswordQueryTool.Backend.Services.Parsing/PasswordQueryTool.Backend.Services.Parsing.csproj"
COPY . .
WORKDIR "/src/PasswordQueryTool.Backend.Services.Parsing"
RUN dotnet build "PasswordQueryTool.Backend.Services.Parsing.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PasswordQueryTool.Backend.Services.Parsing.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PasswordQueryTool.Backend.Services.Parsing.dll"]