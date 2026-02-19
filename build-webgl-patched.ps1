###############################################################################
# build-webgl-patched.ps1
#
# Build WebGL com workaround para o bug do CIL Linker no Unity 6000.3.9f1
#
# O bug: EditorToUnityLinkerData.json contém tipos managed-only (GameFlowManager)
# com "moduleName":"" (vazio), causando ArgumentNullException no linker.
#
# Solução: Este script roda um patcher em loop agressivo (5ms) em um job separado
# que intercepta e corrige o JSON assim que o Bee o regenera, ANTES do linker ler.
###############################################################################

$ErrorActionPreference = 'Stop'

$projectPath   = $PSScriptRoot
$unity         = "C:\Program Files\Unity\Hub\Editor\6000.3.9f1\Editor\Unity.exe"
$logFile       = Join-Path $projectPath "build-automated.log"
$jsonFile      = Join-Path $projectPath "Library\Bee\artifacts\UnityLinkerInputs\EditorToUnityLinkerData.json"

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "  Build WebGL - HeroisDaBiblia3D" -ForegroundColor Cyan
Write-Host "  Unity 6000.3.9f1 + CIL Linker Patch" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# ── Verifica Unity ──
if (-not (Test-Path $unity)) {
    Write-Host "ERRO: Unity não encontrado em: $unity" -ForegroundColor Red
    exit 1
}

# ── Para qualquer Unity rodando ──
$running = Get-Process Unity -ErrorAction SilentlyContinue
if ($running) {
    Write-Host "Parando Unity em execução (PID: $($running.Id))..." -ForegroundColor Yellow
    $running | Stop-Process -Force
    Start-Sleep -Seconds 3
}

# ── Limpar Library\Bee para forçar rebuild completo ──
# Bee mantém estado de build (TundraBuildState.state, tundra.digestcache, *.dag_derived)
# que pode causar pulos do IL2CPP_CodeGen quando os arquivos de saída foram removidos
$beePath = Join-Path $projectPath "Library\Bee"
if (Test-Path $beePath) {
    Write-Host "Limpando Library\Bee para forçar rebuild completo..." -ForegroundColor Yellow
    Remove-Item $beePath -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "Library\Bee limpo com sucesso." -ForegroundColor Green
} else {
    Write-Host "Library\Bee não existe, build será limpo." -ForegroundColor DarkGray
}
Write-Host ""

# ── Pre-patch do JSON se já existir ──
# Module names not in WebGL modules.asset that crash the CIL Linker
$invalidModules = @('Editor','BuildReportingEditor','LocalizationEditor','SketchUpEditor','SubstanceEditor','TextRenderingEditor','VFXEditor','VideoEditor','ShaderFoundry','undefined')

function Patch-LinkerJson {
    param([string]$Path)
    if (-not (Test-Path $Path)) { return $false }
    try {
        $content = [System.IO.File]::ReadAllText($Path)
        $patched = $content
        $changed = $false

        # FIX 1 (ROOT CAUSE): Empty forceIncludeModules - PhysicsBackendPhysX doesn't exist in WebGL
        $before = $patched
        $patched = [regex]::Replace($patched, '"forceIncludeModules"\s*:\s*\[[^\]]*\]', '"forceIncludeModules":[]')
        if ($patched -ne $before) { $changed = $true }

        # FIX 2: Remove typesInScenes entries with empty nativeClass (managed-only types)
        $before = $patched
        $patched = [regex]::Replace($patched, '\{[^{}]*"nativeClass"\s*:\s*""[^{}]*\}\s*,?\s*', '')
        if ($patched -ne $before) { $changed = $true }

        # FIX 3: Replace invalid module names with "Core" in allNativeTypes
        foreach ($mod in $script:invalidModules) {
            $before = $patched
            $escaped = [regex]::Escape($mod)
            $patched = [regex]::Replace($patched, '"module"\s*:\s*"' + $escaped + '"', '"module":"Core"')
            $patched = [regex]::Replace($patched, '"baseModule"\s*:\s*"' + $escaped + '"', '"baseModule":"Core"')
            if ($patched -ne $before) { $changed = $true }
        }

        # Clean trailing commas
        $patched = [regex]::Replace($patched, ',\s*\]', ']')

        if ($changed) {
            [System.IO.File]::WriteAllText($Path, $patched)
            return $true
        }
    } catch {}
    return $false
}

if (Test-Path $jsonFile) {
    $result = Patch-LinkerJson -Path $jsonFile
    if ($result) {
        Write-Host "[PRE-PATCH] Corrigido moduleName vazio no JSON existente" -ForegroundColor Green
    } else {
        Write-Host "[PRE-PATCH] JSON já está correto ou não precisa de patch" -ForegroundColor DarkGray
    }
}

# ── Iniciar Job de monitoramento agressivo ──
Write-Host ""
Write-Host "Iniciando patcher de background (polling 5ms)..." -ForegroundColor Yellow

$patcherJob = Start-Job -ArgumentList $jsonFile -ScriptBlock {
    param($targetFile)
    $patchCount = 0
    $lastWrite  = [DateTime]::MinValue
    
    $badModules = @('Editor','BuildReportingEditor','LocalizationEditor','SketchUpEditor','SubstanceEditor','TextRenderingEditor','VFXEditor','VideoEditor','ShaderFoundry','undefined')
    while ($true) {
        try {
            if ([System.IO.File]::Exists($targetFile)) {
                $info = [System.IO.FileInfo]::new($targetFile)
                if ($info.LastWriteTimeUtc -gt $lastWrite) {
                    $content = [System.IO.File]::ReadAllText($targetFile)
                    $patched = $content
                    $changed = $false

                    # FIX 1 (ROOT CAUSE): Empty forceIncludeModules
                    $before = $patched
                    $patched = [regex]::Replace($patched, '"forceIncludeModules"\s*:\s*\[[^\]]*\]', '"forceIncludeModules":[]')
                    if ($patched -ne $before) { $changed = $true }

                    # FIX 2: Remove typesInScenes with empty nativeClass
                    $before = $patched
                    $patched = [regex]::Replace($patched, '\{[^{}]*"nativeClass"\s*:\s*""[^{}]*\}\s*,?\s*', '')
                    if ($patched -ne $before) { $changed = $true }

                    # FIX 3: Replace invalid module names with Core
                    foreach ($mod in $badModules) {
                        $escaped = [regex]::Escape($mod)
                        $before = $patched
                        $patched = [regex]::Replace($patched, '"module"\s*:\s*"' + $escaped + '"', '"module":"Core"')
                        $patched = [regex]::Replace($patched, '"baseModule"\s*:\s*"' + $escaped + '"', '"baseModule":"Core"')
                        if ($patched -ne $before) { $changed = $true }
                    }

                    # Clean trailing commas
                    $patched = [regex]::Replace($patched, ',\s*\]', ']')

                    if ($changed) {
                        [System.IO.File]::WriteAllText($targetFile, $patched)
                        $patchCount++
                        $lastWrite = [DateTime]::UtcNow
                        Write-Output "PATCH #$patchCount applied at $((Get-Date).ToString('HH_mm_ss.fff'))"
                    } else {
                        $lastWrite = $info.LastWriteTimeUtc
                    }
                }
            }
        } catch {
            # Ignorar erros de acesso concorrente
        }
        [System.Threading.Thread]::Sleep(5)  # 5ms = muito agressivo
    }
}

Write-Host "Patcher rodando (Job ID: $($patcherJob.Id))" -ForegroundColor Green

# ── Limpar log anterior ──
if (Test-Path $logFile) { Remove-Item $logFile -Force }

# ── Iniciar Unity Build ──
Write-Host ""
Write-Host "Iniciando Unity build..." -ForegroundColor Cyan
Write-Host "Log: $logFile" -ForegroundColor DarkGray
Write-Host ""

$startTime = Get-Date

$unityProcess = Start-Process -FilePath $unity -ArgumentList @(
    "-batchmode",
    "-quit",
    "-projectPath", "`"$projectPath`"",
    "-executeMethod", "WebGLBuilder.Build",
    "-logFile", "`"$logFile`""
) -PassThru

Write-Host "Unity PID: $($unityProcess.Id)" -ForegroundColor DarkGray
Write-Host ""
Write-Host "Aguardando build (isso pode demorar vários minutos)..." -ForegroundColor Yellow
Write-Host "Pressione Ctrl+C para cancelar." -ForegroundColor DarkGray
Write-Host ""

# ── Monitorar progresso ──
$lastLogSize = 0
$dots = 0

while (-not $unityProcess.HasExited) {
    Start-Sleep -Seconds 3
    $dots++
    
    # Verificar patches do job
    $jobOutput = Receive-Job $patcherJob -ErrorAction SilentlyContinue
    if ($jobOutput) {
        foreach ($line in $jobOutput) {
            Write-Host "  [PATCHER] $line" -ForegroundColor Magenta
        }
    }
    
    # Mostrar progresso do log
    if (Test-Path $logFile) {
        $currentSize = (Get-Item $logFile).Length
        if ($currentSize -ne $lastLogSize) {
            $tail = Get-Content $logFile -Tail 1 -ErrorAction SilentlyContinue
            if ($tail -and $tail.Length -gt 0) {
                $short = if ($tail.Length -gt 100) { $tail.Substring(0, 100) + "..." } else { $tail }
                Write-Host "  [LOG] $short" -ForegroundColor DarkGray
            }
            $lastLogSize = $currentSize
        } else {
            if ($dots % 10 -eq 0) {
                $elapsed = ((Get-Date) - $startTime).ToString("mm\:ss")
                Write-Host "  [$elapsed] Build em andamento..." -ForegroundColor DarkGray
            }
        }
    }
}

$endTime = Get-Date
$duration = ($endTime - $startTime).ToString("mm\:ss")

# ── Parar patcher ──
Stop-Job $patcherJob -ErrorAction SilentlyContinue
$finalPatches = Receive-Job $patcherJob -ErrorAction SilentlyContinue
Remove-Job $patcherJob -Force -ErrorAction SilentlyContinue

if ($finalPatches) {
    foreach ($line in $finalPatches) {
        Write-Host "  [PATCHER] $line" -ForegroundColor Magenta
    }
}

# ── Verificar resultado ──
Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "  BUILD CONCLUÍDO em $duration" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

$exitCode = $unityProcess.ExitCode
Write-Host "Unity Exit Code: $exitCode"

if ($exitCode -eq 0) {
    Write-Host ""
    Write-Host "  ✅ BUILD SUCESSO!" -ForegroundColor Green
    Write-Host ""
    
    # Verificar arquivos de saída
    $docsPath = Join-Path $projectPath "docs"
    if (Test-Path (Join-Path $docsPath "Build")) {
        $buildFiles = Get-ChildItem (Join-Path $docsPath "Build") -Recurse
        $totalSize = ($buildFiles | Measure-Object -Property Length -Sum).Sum
        Write-Host "  Arquivos em docs/Build:" -ForegroundColor Green
        foreach ($f in $buildFiles) {
            $sizeMB = [math]::Round($f.Length / 1MB, 2)
            Write-Host "    $($f.Name) ($sizeMB MB)" -ForegroundColor White
        }
        $totalMB = [math]::Round($totalSize / 1MB, 2)
        Write-Host ""
        Write-Host "  Tamanho total: $totalMB MB" -ForegroundColor White
    }
} else {
    Write-Host ""
    Write-Host "  ❌ BUILD FALHOU!" -ForegroundColor Red
    Write-Host ""
    
    # Mostrar últimas linhas relevantes do log
    if (Test-Path $logFile) {
        Write-Host "  Últimas mensagens de erro:" -ForegroundColor Yellow
        $errors = Select-String -Path $logFile -Pattern "Fatal|error|failed|FALHOU" -CaseSensitive:$false |
            Select-Object -Last 10
        foreach ($e in $errors) {
            $line = $e.Line.Trim()
            if ($line.Length -gt 150) { $line = $line.Substring(0, 150) + "..." }
            Write-Host "    $line" -ForegroundColor Red
        }
    }
}

Write-Host ""
Write-Host "Log completo: $logFile" -ForegroundColor DarkGray
exit $exitCode
