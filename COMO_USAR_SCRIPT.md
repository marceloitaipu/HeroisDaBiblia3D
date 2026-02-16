# 🚀 COMO RODAR O SCRIPT DE SETUP

## Opção 1: Executar Diretamente

1. Abra PowerShell na pasta do projeto
2. Execute:
```powershell
.\setup-github.ps1
```

Se der erro de política de execução:
```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\setup-github.ps1
```

## Opção 2: Manual (Alternativa)

Se preferir fazer manualmente sem o script:

1. Substitua `SEU-USUARIO` pelo seu usuário do GitHub:
```powershell
git remote add origin https://github.com/SEU-USUARIO/HeroisDaBiblia3D.git
git branch -M main
```

2. Crie o repositório no GitHub: https://github.com/new

3. Faça push:
```powershell
git push -u origin main
```

## O que o script faz:

✅ Verifica se Git está instalado
✅ Configura o remote 'origin'
✅ Renomeia branch para 'main'
✅ Oferece fazer push automaticamente
✅ Mostra próximos passos

---

**Dica:** Use o script para economizar tempo! 🚀
