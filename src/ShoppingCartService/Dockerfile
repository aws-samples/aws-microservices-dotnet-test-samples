FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ShoppingCartSerrvice/ShoppingCartSerrvice.csproj", "ShoppingCartSerrvice/"]
RUN dotnet restore "ShoppingCartSerrvice/ShoppingCartSerrvice.csproj"
COPY . .
WORKDIR "/src/ShoppingCartSerrvice"
RUN dotnet build "ShoppingCartSerrvice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShoppingCartSerrvice.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShoppingCartSerrvice.dll"]
