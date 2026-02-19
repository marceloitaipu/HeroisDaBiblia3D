# Script para iniciar build WebGL em background
# Inicia o Unity e retorna imediatamente

$unityPath = "C:\Program Files\Unity\Hub\Editor\6000.3.9f1\Editor\Unity.exe"
$projectPath = $PWD.Path
$logFile = Join-Path $projectPath "build-automated.log"

Write-Host "Iniciando build WebGL em background..." -ForegroundColor Cyan
Write-Host "Log: $logFile" -ForegroundColor Gray

# Limpar pasta docs/Build se existir
$buildPath = Join-Path $projectPath "docs\Build"
if (Test-Path $buildPath) {
    Write-Host "Limpando build anterior..." -ForegroundColor Yellow
    Remove-Item -Path $buildPath -Recurse -Force -ErrorAction SilentlyContinue
}

# Executar Unity em batch mode (background)
$arguments = @(
    "-quit",
    "-batchmode",
    "-projectPath", "`"$projectPath`"",
    "-buildTarget", "WebGL",
    "-executeMethod", "WebGLBuilder.Build",
    "-logFile", "`"$logFile`""
)

Start-Process -FilePath $unityPath -ArgumentList $arguments -WindowStyle Hidden

Write-Host ""
Write-Host "Build iniciado em background!" -ForegroundColor Green
Write-Host "Tempo estimado: 10-20 minutos" -ForegroundColor Yellow
Write-Host ""
Write-Host "Para acompanhar o progresso:" -ForegroundColor Gray
Write-Host "  Get-Content build-automated.log -Tail 20 -Wait" -ForegroundColor Gray
Write-Host ""
Write-Host "Para verificar se terminou:" -ForegroundColor Gray
Write-Host "  Get-Process Unity -ErrorAction SilentlyContinue" -ForegroundColor Gray
