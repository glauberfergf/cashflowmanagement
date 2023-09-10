#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CashFlowManagement.API/CashFlowManagement.API.csproj", "CashFlowManagement.API/"]
RUN dotnet restore "CashFlowManagement.API/CashFlowManagement.API.csproj"
COPY . .
WORKDIR "/src/CashFlowManagement.API"
RUN dotnet build "CashFlowManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CashFlowManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CashFlowManagement.API.dll"]