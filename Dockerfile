FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ApiCadastro.csproj", "."]
RUN dotnet restore "./ApiCadastro.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ApiCadastro.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiCadastro.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/ ./src/
ENTRYPOINT ["dotnet", "ApiCadastro.dll"]