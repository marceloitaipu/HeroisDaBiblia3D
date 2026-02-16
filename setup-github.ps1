# Script de Setup Rápido do GitHub
# Este script ajuda a configurar o repositório remoto

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  SETUP GITHUB - Heróis da Bíblia" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se git está instalado
$gitInstalled = Get-Command git -ErrorAction SilentlyContinue
if (-not $gitInstalled) {
    Write-Host "❌ Git não está instalado!" -ForegroundColor Red
    Write-Host "Baixe em: https://git-scm.com/download/win" -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ Git encontrado!" -ForegroundColor Green

# Verificar se já tem remote
$hasRemote = git remote -v 2>$null | Select-String "origin"
if ($hasRemote) {
    Write-Host "✅ Remote 'origin' já configurado:" -ForegroundColor Green
    git remote -v
    Write-Host ""
    $continue = Read-Host "Deseja reconfigurar? (s/N)"
    if ($continue -ne "s") {
        exit 0
    }
    git remote remove origin
}

# Solicitar usuário do GitHub
Write-Host ""
Write-Host "Digite seu usuário do GitHub:" -ForegroundColor Yellow
$username = Read-Host

if ([string]::IsNullOrWhiteSpace($username)) {
    Write-Host "❌ Usuário não pode ser vazio!" -ForegroundColor Red
    exit 1
}

# Montar URL
$repoUrl = "https://github.com/$username/HeroisDaBiblia3D.git"

Write-Host ""
Write-Host "🔗 Conectando ao repositório:" -ForegroundColor Cyan
Write-Host $repoUrl -ForegroundColor White

# Adicionar remote
try {
    git remote add origin $repoUrl
    Write-Host "✅ Remote adicionado com sucesso!" -ForegroundColor Green
} catch {
    Write-Host "❌ Erro ao adicionar remote!" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}

# Verificar branch
$currentBranch = git branch --show-current
if ($currentBranch -ne "main") {
    Write-Host "📝 Renomeando branch para 'main'..." -ForegroundColor Cyan
    git branch -M main
}

Write-Host ""
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  ✅ CONFIGURAÇÃO COMPLETA!" -ForegroundColor Green
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Instruções finais
Write-Host "PRÓXIMOS PASSOS:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Crie o repositório no GitHub:" -ForegroundColor White
Write-Host "   https://github.com/new" -ForegroundColor Cyan
Write-Host "   Nome: HeroisDaBiblia3D (exatamente assim)" -ForegroundColor Gray
Write-Host "   Público: ✅" -ForegroundColor Gray
Write-Host ""

Write-Host "2. Faça o push inicial:" -ForegroundColor White
Write-Host "   git push -u origin main" -ForegroundColor Cyan
Write-Host ""

Write-Host "3. Configure o GitHub Pages:" -ForegroundColor White
Write-Host "   Repositório → Settings → Pages" -ForegroundColor Cyan
Write-Host "   Branch: main → Folder: /docs" -ForegroundColor Gray
Write-Host ""

Write-Host "4. Faça o build WebGL no Unity para pasta docs/" -ForegroundColor White
Write-Host ""

Write-Host "Deseja fazer o push agora? (s/N)" -ForegroundColor Yellow
$doPush = Read-Host

if ($doPush -eq "s") {
    Write-Host ""
    Write-Host "📤 Fazendo push..." -ForegroundColor Cyan
    Write-Host "⚠️ Você precisará fornecer suas credenciais" -ForegroundColor Yellow
    Write-Host "   Usuário: $username" -ForegroundColor Gray
    Write-Host "   Senha: Use Personal Access Token" -ForegroundColor Gray
    Write-Host ""
    
    git push -u origin main
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "🎉 PUSH BEM-SUCEDIDO!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Seu código está em:" -ForegroundColor White
        Write-Host "https://github.com/$username/HeroisDaBiblia3D" -ForegroundColor Cyan
    } else {
        Write-Host ""
        Write-Host "❌ Push falhou!" -ForegroundColor Red
        Write-Host "Verifique suas credenciais e tente novamente:" -ForegroundColor Yellow
        Write-Host "git push -u origin main" -ForegroundColor Cyan
    }
} else {
    Write-Host ""
    Write-Host "OK! Execute quando estiver pronto:" -ForegroundColor White
    Write-Host "git push -u origin main" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "📚 Veja START_HERE.md para mais detalhes" -ForegroundColor Gray
Write-Host ""
