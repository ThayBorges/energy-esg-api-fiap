# Script para criar migrações do Entity Framework
# Uso: .\scripts\create-migration.ps1 -MigrationName "NomeDaMigracao"

param(
    [Parameter(Mandatory=$true)]
    [string]$MigrationName
)

Write-Host "Criando migração: $MigrationName" -ForegroundColor Green

dotnet ef migrations add $MigrationName `
    --project src/EnergyESG.Infrastructure `
    --startup-project src/EnergyESG.Api

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migração criada com sucesso!" -ForegroundColor Green
} else {
    Write-Host "Erro ao criar migração!" -ForegroundColor Red
}


