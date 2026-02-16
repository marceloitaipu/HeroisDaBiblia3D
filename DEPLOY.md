# 🚀 Como Publicar no GitHub e Deploy na Web

## Passo 1: Criar Repositório no GitHub

### Via Web (Mais Fácil)
1. Acesse [github.com](https://github.com) e faça login
2. Clique no **+** (canto superior direito) → **New repository**
3. Preencha:
   - **Repository name:** `HeroisDaBiblia3D`
   - **Description:** `Jogo educativo 3D com 5 mundos baseados em histórias bíblicas`
   - **Public** ✅ (para GitHub Pages funcionar grátis)
   - **NÃO** marque "Initialize with README" (já temos)
4. Clique **Create repository**

## Passo 2: Conectar Projeto Local ao GitHub

Abra o PowerShell na pasta do projeto e execute:

```powershell
# Inicializar Git (se ainda não estiver)
git init

# Adicionar todos os arquivos
git add .

# Primeiro commit
git commit -m "Initial commit: Projeto completo com 5 mundos e sistemas avançados"

# Conectar ao repositório remoto (SUBSTITUA SEU-USUARIO)
git remote add origin https://github.com/SEU-USUARIO/HeroisDaBiblia3D.git

# Renomear branch para main
git branch -M main

# Fazer push inicial
git push -u origin main
```

### 🔐 Autenticação
Se o GitHub pedir login:
- **Opção 1 (Recomendada):** Use GitHub Desktop
- **Opção 2:** Personal Access Token
  1. GitHub → Settings → Developer settings → Personal access tokens → Tokens (classic)
  2. Generate new token → Marque `repo`
  3. Use o token como senha ao fazer push

## Passo 3: Fazer Build WebGL no Unity

### Configuração do Build

1. **Abra o Unity** Editor
2. **File → Build Settings**
3. **Selecione WebGL** na lista de plataformas
4. **Switch Platform** (se necessário)

5. **Player Settings:**
   - Resolution and Presentation:
     - Default Canvas Width: **1080**
     - Default Canvas Height: **1920**
   - Publishing Settings:
     - Compression Format: **Brotli**
     - Enable Exceptions: **Explicitly Thrown Exceptions Only**
   - Other Settings:
     - Color Space: **Linear** (melhor qualidade visual)

6. **Build:**
   - Clique em **Build**
   - Selecione a pasta `docs/` (IMPORTANTE!)
   - Aguarde (pode demorar 5-15 minutos)

### ⚠️ Importante
O build irá substituir o `docs/index.html` atual. Isso é esperado!

## Passo 4: Commit e Push do Build

```powershell
# Adicionar arquivos do build
git add docs/

# Commit
git commit -m "WebGL Build - Deploy para GitHub Pages"

# Push
git push origin main
```

## Passo 5: Ativar GitHub Pages

1. Vá para o repositório no GitHub
2. **Settings** → **Pages** (menu lateral esquerdo)
3. **Source:** Deploy from a branch
4. **Branch:** `main` → Folder: `/docs` → **Save**
5. Aguarde 2-5 minutos

### 🎉 Seu jogo estará disponível em:
```
https://SEU-USUARIO.github.io/HeroisDaBiblia3D/
```

## Passo 6: Testar e Instalar no Celular

### No Celular (Android)
1. Abra o link no **Chrome** ou **Edge**
2. Menu (⋮) → **Instalar app** ou **Adicionar à tela inicial**
3. O jogo aparecerá como app nativo! 📱

### No iPhone (iOS)
1. Abra o link no **Safari**
2. Botão **Compartilhar** (quadrado com seta)
3. **Adicionar à Tela de Início**
4. Confirme

## 📋 Checklist Completo

- [ ] Repositório criado no GitHub
- [ ] Git inicializado localmente
- [ ] Commit inicial feito
- [ ] Remote origin configurado
- [ ] Push inicial bem-sucedido
- [ ] Build WebGL compilado
- [ ] Build commitado e pushed
- [ ] GitHub Pages ativado
- [ ] Site acessível pelo link
- [ ] Testado no celular
- [ ] App instalado com sucesso

## 🔄 Workflow Futuro (Para Atualizações)

```powershell
# 1. Faça suas mudanças no Unity
# 2. Build WebGL novamente para docs/
# 3. Commit e push:

git add .
git commit -m "Descrição das mudanças"
git push origin main

# 4. GitHub Pages atualiza automaticamente em ~2 minutos
```

## 🛠️ Troubleshooting

### Erro: "repository not found"
- Verifique se o repositório foi criado no GitHub
- Confirme o nome do usuário no URL remoto

### Build muito grande (>100MB)
- Use compressão Brotli
- Reduza texturas (Edit → Project Settings → Quality)
- Remova assets não usados

### GitHub Pages não atualiza
- Aguarde 5 minutos
- Force refresh no navegador (Ctrl+Shift+R)
- Verifique se a branch e folder estão corretos

### "Não consigo instalar no celular"
- **Android:** Use Chrome ou Edge (não Firefox)
- **iOS:** Use Safari (não Chrome)
- Certifique-se que o site está em HTTPS (GitHub Pages usa)

### Build falha no Unity
- Verifique se WebGL module está instalado (Unity Hub)
- Feche programas pesados (libera RAM)
- Tente Switch Platform antes de Build

## 📊 Analytics (Opcional)

Para ver quantas pessoas acessam seu jogo:

### Google Analytics
1. Crie conta em [analytics.google.com](https://analytics.google.com)
2. Adicione o tracking code no `docs/index.html` (após o build)
3. Faça commit novamente

### Simples (sem código):
Use [GitHub Insights](https://github.com/SEU-USUARIO/HeroisDaBiblia3D/graphs/traffic)
- Mostra visitantes únicos
- Views por dia
- Sites que linkaram para você

## 🌐 Domínio Customizado (Opcional)

Se quiser usar `seujogo.com` ao invés de `usuario.github.io`:

1. Compre domínio (Registro.br, Namecheap, etc)
2. Adicione arquivo `CNAME` na pasta docs/ com seu domínio
3. Configure DNS do domínio para apontar para GitHub Pages
4. Settings → Pages → Custom domain

## 📱 Próximos Passos

- [ ] Compartilhar link nas redes sociais
- [ ] Criar página no itch.io (alternativa)
- [ ] Criar vídeo de gameplay para YouTube
- [ ] Pedir feedback de amigos/comunidade
- [ ] Iterar baseado no feedback

## 🤝 Open Source

Seu projeto está público! Outras pessoas podem:
- ⭐ Dar estrela (star)
- 🍴 Fazer fork
- 🐛 Reportar bugs (Issues)
- 🔧 Contribuir (Pull Requests)

Adicione um badge no README.md:
```markdown
![GitHub stars](https://img.shields.io/github/stars/SEU-USUARIO/HeroisDaBiblia3D)
![GitHub forks](https://img.shields.io/github/forks/SEU-USUARIO/HeroisDaBiblia3D)
```

---

**🎮 Boa sorte com o lançamento do seu jogo!**

Qualquer dúvida, abra uma Issue no repositório ou me pergunte!
