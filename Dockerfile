## ------------------------------------------------------------
## Stage 1: build + publish
## Usa a imagem SDK apenas para compilar/publicar a aplicação.
## ------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia solução e arquivos de projeto primeiro para melhorar cache do restore.
COPY EnergyESG.sln .
COPY src/EnergyESG.Domain/EnergyESG.Domain.csproj src/EnergyESG.Domain/
COPY src/EnergyESG.Application/EnergyESG.Application.csproj src/EnergyESG.Application/
COPY src/EnergyESG.Infrastructure/EnergyESG.Infrastructure.csproj src/EnergyESG.Infrastructure/
COPY src/EnergyESG.Api/EnergyESG.Api.csproj src/EnergyESG.Api/
COPY tests/EnergyESG.Tests/EnergyESG.Tests.csproj tests/EnergyESG.Tests/

# Restaura dependências da solução.
RUN dotnet restore EnergyESG.sln

# Copia o restante do código após o restore.
COPY . .

# Publica apenas o projeto principal da API em modo Release.
# UseAppHost=false evita binário de host nativo e reduz o tamanho do output.
RUN dotnet publish src/EnergyESG.Api/EnergyESG.Api.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

## ------------------------------------------------------------
## Stage 2: runtime
## Imagem final leve, contendo somente runtime + artefatos publicados.
## ------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Variáveis padrão de execução.
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Porta HTTP exposta pelo container.
EXPOSE 8080

# Copia apenas o resultado publicado da etapa de build.
COPY --from=build /app/publish .

# Inicialização da API.
ENTRYPOINT ["dotnet", "EnergyESG.Api.dll"]


