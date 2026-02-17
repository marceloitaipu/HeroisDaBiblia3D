# 🎮 GUIA RÁPIDO - Build Unity WebGL

## Passo 1: Abrir Unity

1. Abra o **Unity Hub**
2. Clique no projeto **HeroisDaBiblia3D**
3. Aguarde o Unity abrir

## Passo 2: Verificar Plataforma WebGL

1. No Unity Editor, vá em **File → Build Settings**
2. Na lista de plataformas, clique em **WebGL**
3. Se o botão mostrar **"Switch Platform"**, clique nele e aguarde
4. Se já estiver selecionado, continue

## Passo 3: Configurar Player Settings

1. Ainda na janela Build Settings, clique em **"Player Settings..."** (canto inferior esquerdo)
2. Na janela Inspector que abre à direita:

### Resolution and Presentation
- Encontre **"Resolution and Presentation"** (clique para expandir)
- **Default Canvas Width:** Digite **1080**
- **Default Canvas Height:** Digite **1920**
- **Run In Background:** ✅ Marque (checkbox)

### Publishing Settings
- Role até **"Publishing Settings"** (clique para expandir)
- **Compression Format:** Selecione **Brotli** (dropdown)
- **Enable Exceptions:** Selecione **"Explicitly Thrown Exceptions Only"**

### Other Settings (Opcional, mas recomendado)
- Role até **"Other Settings"**
- **Color Space:** **Linear** (melhor qualidade visual)
- **Auto Graphics API:** ✅ Deve estar marcado

## Passo 4: Fazer Build

1. **Feche** a janela Player Settings (X no canto)
2. Volte para **Build Settings**
3. Clique no botão grande **"Build"** (não "Build and Run")

## Passo 5: Selecionar Pasta

Uma janela de seleção de pasta vai abrir:

1. Navegue até a pasta do projeto
2. **IMPORTANTE:** Selecione a pasta **docs/**
3. O caminho completo deve ser:
   ```
   C:\Users\marce\OneDrive\Documents\Projetos\HeroisDaBiblia3D\docs
   ```
4. Clique em **"Select Folder"** ou **"Selecionar Pasta"**

## Passo 6: Aguardar Build

⏱️ **O build vai demorar 5-15 minutos dependendo do seu PC**

Você verá:
- Barra de progresso no canto inferior direito
- Mensagens no Console
- O Unity pode parecer travado - **é normal!**

**NÃO feche o Unity durante o build!**

## Passo 7: Build Completo

Quando terminar:
- A barra de progresso desaparece
- Uma mensagem "Build completed" aparece
- A pasta `docs/` agora tem vários arquivos novos

## Passo 8: Verificar Arquivos

Na pasta `docs/`, você deve ver:
- `index.html` (substituído)
- `Build/` (pasta nova)
- `TemplateData/` (pasta nova)
- `StreamingAssets/` (possível)

## Passo 9: Fazer Push (NO POWERSHELL)

Abra o PowerShell na pasta do projeto e execute:

```powershell
git add docs/
git commit -m "WebGL Build - Deploy inicial"
git push origin main
```

---

## ⚠️ Problemas Comuns

### "WebGL module not installed"
→ Unity Hub → Installs → Clique nos 3 pontos da sua versão → Add Modules → Marque WebGL Build Support

### "Out of memory during build"
→ Feche outros programas
→ Build Settings → Player Settings → Publishing → Memory Size: Reduza para 256MB

### "Build failed with errors"
→ Veja o Console (Window → General → Console)
→ Corrija os erros mostrados

### Build muito lento
→ Normal! Aguarde pacientemente
→ Primeira build sempre demora mais

---

## ✅ Checklist

- [ ] Unity aberto
- [ ] Plataforma WebGL selecionada
- [ ] Resolution: 1080x1920
- [ ] Compression: Brotli
- [ ] Build para pasta docs/
- [ ] Build completado (sem erros)
- [ ] Arquivos verificados em docs/
- [ ] git add, commit, push executados

---

**💡 DICA:** Marque esta página nos favoritos para consultar depois!

**⏱️ Tempo total estimado:** 20-30 minutos (sendo 15 min só esperando o build)
