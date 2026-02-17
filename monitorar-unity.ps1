# Monitorar progresso do Unity

Write-Host "Monitorando Unity..." -ForegroundColor Cyan
Write-Host "Pressione Ctrl+C para parar" -ForegroundColor Gray
Write-Host ""

while ($true) {
    $unity = Get-Process Unity -ErrorAction SilentlyContinue
    
    if ($unity) {
        $memMB = [math]::Round($unity.WorkingSet64/1MB, 0)
        $cpuPercent = [math]::Round(($unity.CPU / [Environment]::ProcessorCount), 1)
        
        Write-Host "$(Get-Date -Format 'HH:mm:ss') - Unity rodando | Memoria: $memMB MB | PID: $($unity.Id)" -ForegroundColor Green
        
        # Verificar se build começou (pasta docs/Build existe)
        if (Test-Path "docs\Build") {
            $buildFiles = Get-ChildItem "docs\Build" -Recurse -File -ErrorAction SilentlyContinue
            if ($buildFiles) {
                $totalMB = [math]::Round(($buildFiles | Measure-Object -Property Length -Sum).Sum / 1MB, 1)
                Write-Host "  -> Build em andamento! Arquivos: $($buildFiles.Count) | Tamanho: $totalMB MB" -ForegroundColor Yellow
            }
        }
    } else {
        Write-Host "$(Get-Date -Format 'HH:mm:ss') - Unity fechou" -ForegroundColor Red
        break
    }
    
    Start-Sleep -Seconds 10
}

Write-Host ""
Write-Host "Unity foi fechado." -ForegroundColor Gray
