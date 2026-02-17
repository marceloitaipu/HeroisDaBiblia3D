# 🎮 Como Fazer Build WebGL - Manual Rápido

## ❌ Problema com Build Automático

O build automático via linha de comando falhou devido a um conflito do Unity com OneDrive.
Este é um problema conhecido quando o Unity tenta acessar bibliotecas do sistema em pastas sincronizadas.

**Solução:** Fazer build manualmente pelo Unity Editor (método mais confiável).

---

## ✅ Passo a Passo para Build Manual

### 1️⃣ Abrir o Projeto no Unity

1. Abra o **Unity Hub**
2. Localize o projeto "HeroisDaBiblia3D" na lista
   - Se não aparecer, clique em "Add" e navegue até:
   - `C:\Users\marce\OneDrive\Documents\Projetos\HeroisDaBiblia3D`
3. Clique no projeto para abrir

⏱️ **Tempo estimado:** 1-2 minutos

---

### 2️⃣ Configurar Build Settings

1. No Unity, vá em **File → Build Settings**
2. Na lista "Platform", selecione **WebGL**
3. Se não estiver selecionado, clique em **"Switch Platform"**
   - Isso pode demorar 2-3 minutos
4. Clique em **"Add Open Scenes"** se a cena Main.unity não estiver listada

---

### 3️⃣ Configurar Player Settings (Importante!)

1. Na janela Build Settings, clique em **"Player Settings..."**
2. Configure os seguintes itens:

#### **Resolution and Presentation**
- Default Canvas Width: `1080`
- Default Canvas Height: `1920`
- ✅ Run In Background

#### **Publishing Settings**
- Compression Format: **Brotli** (ou Gzip se Brotli não estiver disponível)
- Exception Support: **Explicitly Thrown Exceptions Only**

#### **Other Settings**
- API Compatibility Level: **.NET Standard 2.1**

---

### 4️⃣ Fazer o Build

1. Volte para **File → Build Settings**
2. Clique em **"Build"** (não "Build And Run")
3. Quando pedir para escolher a pasta:
   - Navegue até a pasta do projeto
   - Entre na pasta **docs**
   - Crie uma nova pasta chamada **Build** (se não existir)
   - Selecione a pasta **Build**
4. Clique em **"Selecionar Pasta"**

⏱️ **Tempo estimado:** 10-20 minutos (dependendo do computador)

---

### 5️⃣ Aguardar Conclusão

- Uma barra de progresso aparecerá no canto inferior direito
- **Não feche o Unity durante o build!**
- O Unity pode parecer travado às vezes - é normal
- Acompanhe o progresso pela barra azul

---

### 6️⃣ Verificar Build

Após o build concluir, verifique se os arquivos foram criados:

```powershell
# Execute no PowerShell:
Get-ChildItem docs\Build -Recurse -File | Measure-Object -Property Length -Sum
```

Você deve ver vários arquivos (`.data`, `.wasm`, `.js`, etc.)

---

### 7️⃣ Commit e Push do Build

```powershell
# Adicionar arquivos do build
git add docs/Build/

# Fazer commit
git commit -m "WebGL Build v2.0.0 - Deploy para GitHub Pages"

# Push para GitHub
git push origin main
```

---

### 8️⃣ Verificar GitHub Pages

1. Acesse: https://github.com/marceloitaipu/HeroisDaBiblia3D/settings/pages
2. Verifique se está configurado:
   - Source: **Deploy from a branch**
   - Branch: **main**
   - Folder: **/docs**
3. Aguarde 2-5 minutos para o deploy
4. Acesse: https://marceloitaipu.github.io/HeroisDaBiblia3D/

---

## 🚨 Problemas Comuns

### "Switch Platform" demorando muito
- Normal na primeira vez (2-5 minutos)
- O Unity está reimportando todos os assets

### Build falha com erro de memória
- Feche outros programas
- Tente novamente

### Build falha com erro de compilação
- Verifique se há erros no Console (Window → General → Console)
- Se houver erros nos scripts, corrija-os primeiro

### Arquivos não aparecem em docs/Build
- Verifique se você selecionou a pasta correta
- O build deve estar em: `docs/Build/Build/` (pasta Build dentro de Build)

---

## 📊 Tamanho Esperado do Build

- **Brotli:** ~15-25 MB
- **Gzip:** ~20-30 MB
- **Uncompressed:** ~40-60 MB

Recomendado usar **Brotli** para melhor performance.

---

## ✅ Checklist Final

- [ ] Projeto aberto no Unity
- [ ] Platform WebGL selecionado
- [ ] Player Settings configurados
- [ ] Build concluído com sucesso
- [ ] Arquivos em docs/Build/ verificados
- [ ] Commit e push realizados
- [ ] GitHub Pages ativado
- [ ] Jogo acessível online

---

**Boa sorte com o build! 🚀**

Se precisar de ajuda, veja também:
- [GUIA_BUILD_UNITY.md](GUIA_BUILD_UNITY.md)
- [GUIA_GITHUB_PAGES.md](GUIA_GITHUB_PAGES.md)
