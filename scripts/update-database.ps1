# Script para aplicar migrações ao banco de dados
# Uso: .\scripts\update-database.ps1

Write-Host "Aplicando migrações ao banco de dados..." -ForegroundColor Green

dotnet ef database update `
    --project src/EnergyESG.Infrastructure `
    --startup-project src/EnergyESG.Api

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migrações aplicadas com sucesso!" -ForegroundColor Green
} else {
    Write-Host "Erro ao aplicar migrações!" -ForegroundColor Red
}


