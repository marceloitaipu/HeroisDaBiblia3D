# 🎨 GUIA VISUAL - Como Fazer Build (COM PRINTS)

## 🖥️ **Tela do Unity - O que você verá:**

```
┌─────────────────────────────────────────────────────┐
│ File  Edit  Assets  GameObject  Component  Window  │ ← MENU DO TOPO
├─────────────────────────────────────────────────────┤
│                                                      │
│         [  Cena 3D do Jogo  ]                       │
│         (Aqui você vê o jogador,                    │
│          chão, câmera, etc.)                        │
│                                                      │
├─────────────────────────────────────────────────────┤
│ Console  | Project  | Hierarchy                     │
└─────────────────────────────────────────────────────┘
```

---

## 📖 **PASSO 1: Abrir Build Settings**

1. Olhe para o **TOPO DA TELA**
2. Você verá: **File  Edit  Assets...**
3. Clique em **File**
4. Um menu abrirá com várias opções
5. Procure por **"Build Settings..."** (quase no final da lista)
6. Clique nele

**Atalho:** Pressione **Ctrl+Shift+B**

---

## 📖 **PASSO 2: Janela Build Settings**

Uma nova janela abrirá assim:

```
┌───────────────────────────────────────────┐
│         Build Settings                    │
├───────────────────────────────────────────┤
│                                           │
│  Scenes In Build:                         │
│  ✓ Assets/Scenes/Main.unity              │
│                                           │
│  Platform:          ┌──────────────────┐ │
│  ┌ PC               │                  │ │
│  ┌ Mac              │   [Preview da    │ │
│  ┌ Linux            │    plataforma]   │ │
│  ● WebGL  ←────────┘                   │ │
│  ┌ iOS                                  │ │
│  ┌ Android                              │ │
│                                           │
│           [Switch Platform]               │
│                                           │
│    [Player Settings...]  [Build]  [X]     │
└───────────────────────────────────────────┘
```

---

## 📖 **PASSO 3: Selecionar WebGL**

1. Na lista **"Platform"** (lado esquerdo)
2. Procure por **"WebGL"**
3. **Clique UMA VEZ** em WebGL
4. Observe o ícone do Unity ao lado:
   - **Se estiver COLORIDO:** Já está selecionado! Pule para Passo 4
   - **Se estiver CINZA:** Continue para o próximo item

---

## 📖 **PASSO 3.1: Switch Platform (SE NECESSÁRIO)**

1. Clique no botão **"Switch Platform"** (embaixo da lista)
2. Aguarde **2-5 minutos**
3. Uma barra de progresso aparecerá:
   ```
   Importing Assets... 23%
   [████████░░░░░░░░░░░░]
   ```
4. Aguarde até 100%

---

## 📖 **PASSO 4: Player Settings (OPCIONAL mas RECOMENDADO)**

1. Clique em **"Player Settings..."** (botão inferior esquerdo)
2. No painel Inspector (direita), procure por:
   - **Resolution and Presentation**
     - Default Canvas Width: `1080`
     - Default Canvas Height: `1920`
   - **Publishing Settings**
     - Compression Format: `Brotli` ou `Gzip`

---

## 📖 **PASSO 5: FAZER O BUILD!**

1. Na janela Build Settings, clique no botão **"Build"**
   - **NÃO** clique em "Build And Run"
   - Apenas **"Build"**

2. Uma janela de **escolha de pasta** abrirá

3. **ATENÇÃO!** Navegue até a pasta correta:
   ```
   C:\Users\marce\OneDrive\Documents\Projetos\HeroisDaBiblia3D
   └── docs
       └── Build  ← ESCOLHA ESTA PASTA!
   ```

4. **Se a pasta Build não existir:**
   - Clique em "Nova pasta"
   - Digite: `Build`
   - Entre na pasta Build
   - Clique em "Selecionar Pasta"

5. **Se a pasta Build já existe:**
   - Entre na pasta (clique duas vezes)
   - Clique em "Selecionar Pasta"

---

## 📖 **PASSO 6: AGUARDAR BUILD**

Você verá uma janela de progresso:

```
┌─────────────────────────────────────┐
│     Building Player                 │
├─────────────────────────────────────┤
│                                     │
│  Building scripts...                │
│  [████████████████░░░░]  76%        │
│                                     │
│  Please wait...                     │
│                                     │
│            [Cancel]                 │
└─────────────────────────────────────┘
```

**Etapas que você verá:**
1. "Preparing build..."
2. "Building scripts..."
3. "Compiling shaders..." (mais demorado!)
4. "Building WebGL player..."
5. "Compressing files..."

⏱️ **Tempo total: 15-25 minutos**

☕ **Dica:** Aproveite para descansar!

**⚠️ NÃO:**
- ❌ Feche o Unity
- ❌ Clique em Cancel
- ❌ Desligue o computador
- ❌ Force close no Task Manager

---

## 📖 **PASSO 7: BUILD CONCLUÍDO!**

Quando terminar, a janela de progresso desaparecerá.

**Verificar se deu certo:**

Execute no PowerShell:

```powershell
Get-ChildItem docs\Build -Recurse | Select-Object Name | Select-Object -First 15
```

**Você deve ver arquivos como:**
- `index.html`
- `Build/` (pasta)
- `TemplateData/` (pasta)
- Vários arquivos `.js`, `.wasm`, `.data`

---

## 📖 **PASSO 8: COMMIT E PUSH**

No PowerShell:

```powershell
git add docs/Build/
git commit -m "WebGL Build v2.0 - Jogo pronto para deploy"
git push origin main
```

---

## 📖 **PASSO 9: AGUARDAR GITHUB PAGES (2-5 min)**

1. Acesse: https://github.com/marceloitaipu/HeroisDaBiblia3D
2. Veja o ícone ao lado do último commit:
   - 🟠 Laranja: Processando...
   - ✅ Verde: Pronto!
3. Acesse: **https://marceloitaipu.github.io/HeroisDaBiblia3D/**

---

## 🎉 **PRONTO! JOGO ONLINE!**

Compartilhe o link:
```
https://marceloitaipu.github.io/HeroisDaBiblia3D/
```

---

## 🆘 **PROBLEMAS?**

### Unity não abre
```powershell
# Tente forçar abertura
Start-Process "C:\Program Files\Unity\Hub\Editor\2022.3.72f1\Editor\Unity.exe" -ArgumentList "-projectPath", "`"$PWD`""
```

### WebGL não aparece na lista
- Vá em Unity Hub → Installs → 2022.3.72f1 → Add Modules → WebGL Build Support

### Build falha
- Veja o Console do Unity (Window → General → Console)
- Copie o erro e me envie

### Memória insuficiente
- Feche outros programas
- Tente novamente
