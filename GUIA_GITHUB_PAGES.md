# 🌐 GUIA RÁPIDO - Ativar GitHub Pages

## ⚠️ IMPORTANTE
**Só faça isto DEPOIS do build do Unity e do push!**

---

## Passo 1: Acessar Configurações

A página já foi aberta automaticamente para você:
https://github.com/marceloitaipu/HeroisDaBiblia3D/settings/pages

Ou manualmente:
1. Acesse: https://github.com/marceloitaipu/HeroisDaBiblia3D
2. Clique na aba **"Settings"** (⚙️)
3. No menu lateral esquerdo, clique em **"Pages"**

## Passo 2: Configurar Source

Na seção **"Build and deployment"**:

1. **Source:** 
   - Certifique-se que está selecionado: **"Deploy from a branch"**

2. **Branch:**
   - Primeiro dropdown: Selecione **"main"**
   - Segundo dropdown: Selecione **"/docs"**
   - Clique no botão **"Save"** ao lado

## Passo 3: Aguardar Deploy

Após salvar:
- Uma mensagem azul aparece: "GitHub Pages source saved"
- A página recarrega automaticamente
- Você verá: **"Your site is live at https://marceloitaipu.github.io/HeroisDaBiblia3D/"**

⏱️ **Aguarde 2-5 minutos** para o site ficar disponível

## Passo 4: Verificar

Recarregue a página de Settings → Pages após alguns minutos:
- Se aparecer um **✅ verde**: Site está no ar!
- Se aparecer ⏳ amarelo: Ainda processando, aguarde mais
- Se aparecer ❌ vermelho: Houve erro (veja seção de problemas)

## Passo 5: Testar

Acesse o link do seu jogo:
**https://marceloitaipu.github.io/HeroisDaBiblia3D/**

O jogo deve carregar e funcionar!

---

## 🎨 OPCIONAL: Custom Domain

Se você tiver um domínio próprio (ex: meujogo.com):

1. Na mesma página (Settings → Pages)
2. Seção **"Custom domain"**
3. Digite seu domínio
4. Clique **"Save"**
5. Configure o DNS do domínio para apontar para GitHub Pages

---

## ⚠️ Problemas Comuns

### "404 - File not found"
**Causa:** Pasta docs/ vazia ou sem index.html

**Solução:**
1. Verifique se o build do Unity foi feito na pasta docs/
2. Verifique se você fez git push dos arquivos
3. Aguarde 5 minutos após o push

### "Site not building"
**Causa:** Branch ou folder incorretos

**Solução:**
1. Volte em Settings → Pages
2. Certifique-se: Branch = **main**, Folder = **/docs**
3. Clique Save novamente

### "Showing old version of the site"
**Causa:** Cache do navegador ou GitHub

**Solução:**
1. Force refresh: **Ctrl + Shift + R** (ou **Cmd + Shift + R** no Mac)
2. Aguarde 2 minutos e tente novamente
3. Abra em aba anônima/privada

### "White screen / Unity not loading"
**Causa:** Build com problemas ou compressão incorreta

**Solução:**
1. Verifique se escolheu compressão **Brotli** no Unity
2. Verifique console do navegador (F12 → Console)
3. Tente fazer novo build

---

## ✅ Checklist

- [ ] Build do Unity completo
- [ ] Git push feito dos arquivos em docs/
- [ ] Acessei Settings → Pages
- [ ] Configurado: main branch, /docs folder
- [ ] Clicado em Save
- [ ] Aguardado 3-5 minutos
- [ ] Acessado o link e testado
- [ ] Jogo carregando corretamente

---

## 📱 PRÓXIMO PASSO: Instalar no Celular

Depois que o site estiver funcionando:

**Android (Chrome):**
1. Abra o link no Chrome
2. Menu (⋮) → "Instalar app"
3. Confirme

**iOS (Safari):**
1. Abra o link no Safari
2. Botão Compartilhar (□↑)
3. "Adicionar à Tela de Início"

---

## 🎉 PARABÉNS!

Seu jogo está oficialmente publicado na web!

**Link para compartilhar:**
https://marceloitaipu.github.io/HeroisDaBiblia3D/

---

**💡 DICA:** Compartilhe nas redes sociais, grupos, amigos para receber feedback!
