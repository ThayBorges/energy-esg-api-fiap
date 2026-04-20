# Projeto - Cidades ESG Inteligentes

Este repositório apresenta a **Energy ESG API**, uma API REST desenvolvida em **.NET 8** com foco em eficiência energética e boas práticas ESG.  
O objetivo acadêmico é demonstrar um ciclo DevOps completo, incluindo build, testes automatizados, containerização e deploy em dois ambientes simulados.

O projeto foi estruturado para atender aos requisitos da atividade avaliativa de DevOps, com:

- pipeline de CI/CD com GitHub Actions;
- execução de testes xUnit no processo de integração contínua;
- empacotamento da aplicação via Docker;
- orquestração da API e do SQL Server com Docker Compose;
- simulação de ambientes de **staging** e **produção**.

## Como executar localmente com Docker

### Pré-requisitos

- Docker Desktop instalado e em execução;
- portas livres: `8081`, `8082`, `14331` e `14332` (ou ajuste via variáveis);
- arquivo `.env` (opcional) com variáveis de ambiente do projeto.

### 1) Build da imagem da API

Na raiz do projeto, execute:

```bash
docker build -t energy-esg-api:latest .
```

### 2) Subir ambiente de staging (simulado)

```bash
docker compose --profile staging up -d --build
```

Validação:

```bash
docker compose --profile staging ps
```

Endpoint esperado da API (staging):

- `http://localhost:8081`

### 3) Subir ambiente de produção (simulado)

```bash
docker compose --profile production up -d
```

Validação:

```bash
docker compose --profile production ps
```

Endpoint esperado da API (produção simulada):

- `http://localhost:8082`

### 4) Encerrar os ambientes

```bash
docker compose --profile staging down -v
docker compose --profile production down -v
```

## Pipeline CI/CD

O pipeline foi implementado com **GitHub Actions** no arquivo:

- `.github/workflows/ci-cd.yml`

### Estratégia adotada

O workflow é acionado em `push` na branch `main` e executa as etapas abaixo de forma encadeada:

1. **Restore** de dependências da solução .NET;
2. **Build** da aplicação em modo Release;
3. **Testes automatizados** com xUnit;
4. **Build da imagem Docker** da API;
5. **Deploy em staging** (simulado via Docker Compose);
6. **Deploy em produção** (simulado via Docker Compose, após sucesso do staging).

### Fluxo técnico resumido

- Job `ci`: valida o código com `dotnet restore`, `dotnet build` e `dotnet test`.
- Job `docker_image`: gera a imagem da API com tag baseada no commit e publica como artefato do pipeline.
- Job `deploy_staging`: carrega a imagem, injeta variáveis/secrets de staging e sobe containers com `docker-compose.staging.yml`.
- Job `deploy_production`: promove a mesma imagem validada e executa deploy com `docker-compose.production.yml`.

### Segregação de ambientes

Foram definidos dois ambientes no GitHub Actions:

- `staging`
- `production`

Cada um pode usar secrets independentes, como:

- `SQL_SA_PASSWORD_STAGING`, `JWT_KEY_STAGING`
- `SQL_SA_PASSWORD_PRODUCTION`, `JWT_KEY_PRODUCTION`

Isso garante separação de configuração e aproxima o comportamento de um cenário real de entrega contínua.

## Containerização

A aplicação utiliza um **Dockerfile multi-stage** para otimizar o processo de build e reduzir o tamanho da imagem final.

### Estratégia do Dockerfile

- Stage de build com `mcr.microsoft.com/dotnet/sdk:8.0`;
- restore, compilação e publish da API;
- stage final com `mcr.microsoft.com/dotnet/aspnet:8.0` (mais leve);
- exposição da porta `8080`;
- inicialização com `dotnet EnergyESG.Api.dll`.

### Orquestração com Docker Compose

Foram definidos serviços para API e SQL Server em ambientes separados:

- `docker-compose.yml` com profiles (`staging` e `production`);
- `docker-compose.staging.yml`;
- `docker-compose.production.yml`.

Elementos de infraestrutura aplicados:

- **volumes** para persistência dos dados SQL Server;
- **variáveis de ambiente** para connection string e JWT;
- **redes internas** isoladas por ambiente.

## Prints do funcionamento (indicar onde inserir)

Inserir nesta seção evidências visuais da execução do projeto. Sugestão de organização:

1. **Pipeline CI/CD rodando no GitHub Actions**
   - print da execução com jobs `ci`, `docker_image`, `deploy_staging`, `deploy_production`.
2. **Build e testes automatizados**
   - print do log com `dotnet restore`, `dotnet build` e `dotnet test` finalizados com sucesso.
3. **Deploy em staging**
   - print do `docker compose ps` e logs do ambiente staging.
4. **Deploy em produção (simulado)**
   - print do `docker compose ps` e logs do ambiente production.
5. **API em funcionamento**
   - print do Swagger ou chamada em endpoint HTTP retornando sucesso.

Modelo de legenda recomendado:

- *Figura 1 - Execução do pipeline CI/CD na branch main.*
- *Figura 2 - Testes automatizados (xUnit) no job de CI.*
- *Figura 3 - Ambiente de staging em execução.*
- *Figura 4 - Ambiente de produção simulado em execução.*

## Tecnologias utilizadas

- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server 2022**
- **xUnit**
- **Docker**
- **Docker Compose**
- **GitHub Actions**
- **JWT Authentication**
- **Swagger/OpenAPI**


