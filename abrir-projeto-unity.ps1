# Script para abrir o projeto no Unity 2022.3 (versão estável)

$unityPath = "C:\Program Files\Unity\Hub\Editor\2022.3.72f1\Editor\Unity.exe"
$projectPath = $PWD.Path

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  ABRINDO PROJETO NO UNITY 2022.3" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Unity: $unityPath" -ForegroundColor Gray
Write-Host "Projeto: $projectPath" -ForegroundColor Gray
Write-Host ""

if (-not (Test-Path $unityPath)) {
    Write-Host "ERRO: Unity 2022.3.72f1 nao encontrado!" -ForegroundColor Red
    Write-Host "Instale via Unity Hub ou use Unity 6" -ForegroundColor Yellow
    exit 1
}

Write-Host "Abrindo Unity..." -ForegroundColor Green
Write-Host ""
Write-Host "IMPORTANTE:" -ForegroundColor Yellow
Write-Host "1. Aguarde o Unity carregar completamente" -ForegroundColor White
Write-Host "2. Depois va em: File > Build Settings" -ForegroundColor White
Write-Host "3. Selecione WebGL e clique em Switch Platform" -ForegroundColor White
Write-Host "4. Depois clique em Build e escolha a pasta docs/Build" -ForegroundColor White
Write-Host ""

# Abrir Unity
Start-Process -FilePath $unityPath -ArgumentList "-projectPath", "`"$projectPath`""

Write-Host "Unity iniciado!" -ForegroundColor Green
Write-Host "Aguarde a janela do Unity abrir..." -ForegroundColor Gray
