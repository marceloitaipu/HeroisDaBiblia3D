# 🎨 Como Criar Ícones Personalizados (Opcional)

Os ícones atuais são placeholders em SVG. Para criar ícones personalizados:

## Opção 1: Online (Mais Fácil)

### Usar Canva (Grátis)
1. Acesse [canva.com](https://www.canva.com)
2. Crie design customizado: **512x512px**
3. Design sugerido:
   - Fundo com gradiente (roxo → azul)
   - Logo/símbolo do jogo no centro
   - Bordas arredondadas (radius: 100px)
4. Baixe como PNG
5. Renomeie para `icon-512.png`

### Gerar múltiplos tamanhos
1. Use [realfavicongenerator.net](https://realfavicongenerator.net)
2. Upload do icon-512.png
3. Gera todos os tamanhos automaticamente
4. Baixe e substitua na pasta `docs/`

## Opção 2: Unity (Usar Assets do Jogo)

### Capturar Screenshot
1. No Unity Editor, ajuste câmera para mostrar herói
2. `Window → Analysis → Frame Debugger`
3. Capture frame com transparência
4. Edite no Photoshop/GIMP:
   - Tamanho: 512x512px
   - Adicione fundo colorido
   - Adicione texto "Heróis da Bíblia"
5. Exporte como PNG

## Opção 3: Ferramenta Automática

### PWA Asset Generator

```bash
# Instalar (requer Node.js)
npm install -g pwa-asset-generator

# Gerar todos os ícones de uma imagem
pwa-asset-generator logo.png docs/ --favicon --index docs/index.html
```

## Opção 4: Converter SVG para PNG

Os SVGs atuais (icon-192.png.svg e icon-512.png.svg) podem ser convertidos:

### No Windows
1. Abra no navegador (Chrome/Edge)
2. F12 → Console
3. Cole:
```javascript
// Baixar SVG como PNG
const svg = document.querySelector('svg');
const canvas = document.createElement('canvas');
canvas.width = 512;
canvas.height = 512;
const ctx = canvas.getContext('2d');
const img = new Image();
img.onload = () => {
  ctx.drawImage(img, 0, 0);
  canvas.toBlob(blob => {
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'icon-512.png';
    a.click();
  });
};
img.src = 'data:image/svg+xml;base64,' + btoa(svg.outerHTML);
```

### Online
- [cloudconvert.com/svg-to-png](https://cloudconvert.com/svg-to-png)
- [svgtopng.com](https://svgtopng.com)

## Tamanhos Necessários

- **192x192px** - Android, Chrome
- **512x512px** - Android, Chrome (maskable)
- **180x180px** - iOS (apple-touch-icon)

## Dicas de Design

### ✅ Boas Práticas
- Fundo sólido ou gradiente (não transparente)
- Ícone centralizado
- Alto contraste
- Sem texto pequeno (ilegível)
- Cores vibrantes
- Bordas arredondadas

### ❌ Evitar
- Fundo branco (some em home screen clara)
- Detalhes muito finos
- Texto longo
- Muitas cores
- Ícone muito complexo

## Exemplo de Design Sugerido

```
┌──────────────────┐
│  Gradiente       │
│  Roxo → Azul     │
│                  │
│     ⚔️  🛡️       │
│                  │
│   HERÓIS DA      │
│     BÍBLIA       │
│                  │
└──────────────────┘
```

## Testar Ícones

### Chrome DevTools
1. F12 → Application → Manifest
2. Verifica se ícones estão carregando
3. Mostra preview

### Real Device Lighthouse
1. Chrome DevTools → Lighthouse
2. Roda PWA audit
3. Verifica qualidade dos ícones

---

## 🎨 Recursos Gratuitos

### Icons
- [flaticon.com](https://www.flaticon.com) - milhões de ícones
- [fontawesome.com](https://fontawesome.com) - ícones vetoriais
- [icons8.com](https://icons8.com) - ícones modernos

### Gradientes
- [cssgradient.io](https://cssgradient.io)
- [uigradients.com](https://uigradients.com)

### Cores
- [coolors.co](https://coolors.co) - gerador de paletas
- [materialpalette.com](https://materialpalette.com)

---

**💡 Dica:** Os ícones atuais já funcionam! Você pode personalizá-los depois.
