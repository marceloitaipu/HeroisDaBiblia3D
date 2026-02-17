# 🚀 SOLUÇÃO: Como Fazer o Build (3 Opções Fáceis)

## ❌ Problema Identificado

O projeto está configurado para Unity 6, mas pode haver conflitos. **JÁ CORRIGI O PROJETO** para usar Unity 2022.3 que você tem instalado!

---

## ✅ OPÇÃO 1: Abrir Unity Manualmente pelo Script (MAIS FÁCIL)

Execute este comando no PowerShell:

```powershell
.\abrir-projeto-unity.ps1
```

**O que vai acontecer:**
1. O Unity 2022.3 abrirá automaticamente com o projeto
2. Aguarde carregar (1-2 minutos)
3. Quando carregar, siga os passos no console PowerShell

**Depois que o Unity abrir:**
1. Vá em **File → Build Settings**
2. Selecione **WebGL**
3. Clique em **Switch Platform** (se aparecer)
4. Clique em **Build**
5. Escolha a pasta: `docs/Build`
6. Aguarde 15-20 minutos

---

## ✅ OPÇÃO 2: Abrir Unity Hub Manualmente

Se o script não funcionar:

1. Abra o **Unity Hub**
2. Se o projeto não aparecer na lista:
   - Clique em **"Add"**
   - Selecione a pasta: `C:\Users\marce\OneDrive\Documents\Projetos\HeroisDaBiblia3D`
3. **IMPORTANTE:** Antes de abrir, verifique:
   - Ao lado do nome do projeto deve mostrar **"2022.3.72f1"**
   - Se mostrar "6000.3.8f1" ou outra versão, clique com botão direito → **"Open with** → Escolha **2022.3.72f1**
4. Clique para abrir
5. Siga os mesmos passos da Opção 1 (Build Settings → WebGL → Build)

---

## ✅ OPÇÃO 3: Build Automático via GitHub Actions (SEM UNITY!)

**Esta opção faz o build na nuvem, você NÃO precisa ter Unity!**

### Passo 1: Ativar GitHub Actions

```powershell
# Fazer push do workflow que criei
git add .github/workflows/build-webgl.yml
git add ProjectSettings/ProjectVersion.txt
git add abrir-projeto-unity.ps1
git commit -m "Adiciona build automático via GitHub Actions"
git push origin main
```

### Passo 2: Configurar Licença Unity (Uma vez só)

1. Acesse: https://github.com/marceloitaipu/HeroisDaBiblia3D/settings/secrets/actions
2. Clique em **"New repository secret"**
3. Crie 3 secrets (use uma Unity License gratuita):

**Para obter a licença:**
- Acesse: https://id.unity.com/
- Faça login
- Personal → Licenses → Add license → Personal → Get a free personal license

Depois de ativar a licença, você precisa codificá-la. Execute:

```powershell
# No Unity Editor (se conseguir abrir):
# Menu → Edit → Preferences → Licenses → Manage Licenses
# Pegue o arquivo .ulf e converta para base64
```

**OU use este método mais simples:**
- Use o serviço: https://unity-ci.com/docs/github/activation

### Passo 3: Rodar o Build

Depois de configurar os secrets:

1. Vá em: https://github.com/marceloitaipu/HeroisDaBiblia3D/actions
2. Clique no workflow **"Build and Deploy WebGL"**
3. Clique em **"Run workflow"**
4. Aguarde 20-30 minutos
5. O build será feito automaticamente e deployado!

---

## 🎯 QUAL OPÇÃO ESCOLHER?

| Opção | Dificuldade | Tempo | Requer Unity Local |
|-------|-------------|-------|-------------------|
| Opção 1 (Script) | ⭐ Fácil | 20 min | ✅ Sim |
| Opção 2 (Manual) | ⭐⭐ Média | 25 min | ✅ Sim |
| Opção 3 (GitHub Actions) | ⭐⭐⭐ Complexa | 40 min (setup) + 30 min (build) | ❌ Não |

**RECOMENDAÇÃO:** Tente a **Opção 1** primeiro. Se não funcionar, me avise qual erro apareceu!

---

## 🐛 Se o Unity Ainda Não Abrir

### Problema: OneDrive Bloqueando Arquivos

O OneDrive pode estar bloqueando arquivos do Unity. Execute:

```powershell
# Pausar sincronização temporariamente
# 1. Clique no ícone do OneDrive na bandeja do sistema
# 2. Configurações → Pausar sincronização → 2 horas
```

Depois tente abrir o Unity novamente.

---

### Problema: Módulo WebGL Não Instalado

Se o Unity abrir mas não mostrar WebGL nas opções:

1. Abra o **Unity Hub**
2. Vá em **"Installs"**
3. Clique nos 3 pontinhos ao lado de **"2022.3.72f1"**
4. Clique em **"Add Modules"**
5. Marque **"WebGL Build Support"**
6. Clique em **"Install"**
7. Aguarde a instalação (2-5 minutos)

---

## 📞 Ainda Não Funciona?

Me diga qual erro específico está aparecendo:

```powershell
# Ver logs do Unity
Get-Content "$env:LOCALAPPDATA\Unity\Editor\Editor.log" -Tail 50
```

Copie e cole o erro aqui que eu te ajudo!

---

## 🎮 Alternativa Final: Publish na Unity Play

Se nada funcionar, você pode publicar direto pela Unity:

1. Crie conta em: https://play.unity.com/
2. No Unity Editor: File → Publish to Unity Play
3. O jogo fica hospedado gratuitamente!
