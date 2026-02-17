# Script para fazer build WebGL automatizado
# Executa o Unity em batch mode com o WebGLBuilder.cs

$ErrorActionPreference = "Continue"

$unityPath = "C:\Program Files\Unity\Hub\Editor\6000.3.8f1\Editor\Unity.exe"
$projectPath = $PWD.Path
$logFile = Join-Path $projectPath "build-automated.log"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  BUILD WEBGL - Herois da Biblia 3D" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Unity Path: $unityPath" -ForegroundColor Gray
Write-Host "Project: $projectPath" -ForegroundColor Gray
Write-Host "Log: $logFile" -ForegroundColor Gray
Write-Host ""

# Verificar se Unity existe
if (-not (Test-Path $unityPath)) {
    Write-Host "ERRO: Unity nao encontrado!" -ForegroundColor Red
    exit 1
}

# Limpar pasta docs/Build se existir
$buildPath = Join-Path $projectPath "docs\Build"
if (Test-Path $buildPath) {
    Write-Host "Limpando build anterior..." -ForegroundColor Yellow
    Remove-Item -Path $buildPath -Recurse -Force -ErrorAction SilentlyContinue
}

Write-Host ""
Write-Host "Iniciando build WebGL..." -ForegroundColor Green
Write-Host "ATENCAO: Isso pode demorar 10-20 minutos!" -ForegroundColor Yellow
Write-Host "Acompanhe o progresso em: $logFile" -ForegroundColor Yellow
Write-Host ""

# Executar Unity em batch mode
$arguments = @(
    "-quit",
    "-batchmode",
    "-projectPath", "`"$projectPath`"",
    "-buildTarget", "WebGL",
    "-executeMethod", "WebGLBuilder.Build",
    "-logFile", "`"$logFile`""
)

Write-Host "Executando Unity..." -ForegroundColor Cyan
Write-Host ""

$process = Start-Process -FilePath $unityPath -ArgumentList $arguments -PassThru -NoNewWindow

Write-Host "Processo Unity iniciado (PID: $($process.Id))" -ForegroundColor Green
Write-Host ""
Write-Host "Aguardando conclusao do build..." -ForegroundColor Yellow
Write-Host "(Este processo rodara em primeiro plano e pode demorar bastante)" -ForegroundColor Gray
Write-Host ""

# Aguardar conclusão
$process.WaitForExit()

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

if ($process.ExitCode -eq 0) {
    Write-Host "BUILD CONCLUIDO COM SUCESSO!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Arquivos gerados em: docs/Build/" -ForegroundColor Green
    
    # Verificar se os arquivos foram criados
    if (Test-Path "$buildPath\Build") {
        $files = Get-ChildItem "$buildPath\Build" -Recurse | Measure-Object -Property Length -Sum
        Write-Host "Total de arquivos: $($files.Count)" -ForegroundColor Gray
        Write-Host "Tamanho total: $([math]::Round($files.Sum / 1MB, 2)) MB" -ForegroundColor Gray
    }
} else {
    Write-Host "ERRO NO BUILD! (Exit Code: $($process.ExitCode))" -ForegroundColor Red
    Write-Host ""
    Write-Host "Verifique o log: $logFile" -ForegroundColor Yellow
}

Write-Host "========================================" -ForegroundColor Cyan
