# Heróis da Bíblia 3D — Turbo (Portrait)

![Unity](https://img.shields.io/badge/Unity-6.0-black?logo=unity)
![C#](https://img.shields.io/badge/C%23-10.0-blue?logo=csharp)
![License](https://img.shields.io/badge/License-MIT-green)

## 📖 Descrição

Jogo educativo 3D para mobile (modo retrato) com 5 mundos baseados em histórias bíblicas. Desenvolvido em Unity com foco em performance e experiência mobile otimizada.

### 🎮 Mundos Disponíveis

1. **Noé** - Runner: Corra pela arca coletando pergaminhos
2. **Davi x Golias** - Boss: Derrote Golias com timing perfeito
3. **Jonas** - Puzzle: Ordene os eventos da história
4. **Moisés** - Puzzle: Recrie a sequência do milagre do Mar Vermelho
5. **Jesus** - Coleta: Colete corações espalhados pelo caminho

## ✨ Características

### 🎯 Gameplay
- 5 modos de jogo únicos
- Sistema de progressão por mundos
- 3 heróis jogáveis (Theo, Lia, Nina)
- 3 skins customizáveis
- Sistema de moedas e virtudes
- Mini-desafios educativos

### 🏆 Sistemas Avançados
- **Conquistas**: 20+ achievements com recompensas
- **Object Pooling**: Performance otimizada
- **Quality Manager**: Ajuste automático de gráficos
- **Audio System**: Música e SFX com fade e controle de volume
- **Localização**: Suporte para PT, EN, ES
- **Effect Manager**: Sistema de partículas pooled

### 📱 Mobile-First
- Interface otimizada para portrait
- Virtual joystick responsivo
- Detecção automática de qualidade
- FPS adaptativo
- Baixo consumo de bateria

## 🚀 Como Abrir o Projeto

### Requisitos
- Unity 6 (6000.3.9f1) ou superior
- 2GB+ RAM disponível
- Suporte a C# 9.0+

### Passos
1. Clone o repositório
```bash
git clone https://github.com/seu-usuario/herois-biblia-3d.git
```

2. Abra no Unity Hub
   - Unity Hub → Add → Selecione a pasta do projeto
   - Aguarde a importação dos assets

3. Abra a cena principal
   - Navegue para `Assets/Scenes/Main.unity`
   - Clique em Play na Unity Editor

## 🎮 Controles

### PC (Editor/Teste)
- **WASD** ou **Setas**: Movimento lateral
- **Espaço**: Pular / Ação
- **Shift**: Deslizar (Runner)

### Mobile
- **Virtual Joystick**: Movimento
- **Botões na tela**: Ações contextuais por modo

## 🏗️ Arquitetura do Código

### Padrões Utilizados
- **Singleton**: Gerenciadores globais (AudioManager, QualityManager, etc)
- **Object Pooling**: Reutilização eficiente de objetos
- **ScriptableObjects**: Configurações data-driven
- **Observer Pattern**: Sistema de eventos (Achievements)
- **State Machine**: Controle de fluxo do jogo

### Estrutura de Pastas
```
Assets/
├── Scenes/          # Cenas do Unity
├── Scripts/         # Código C#
│   ├── Managers/    # Singletons globais
│   ├── Gameplay/    # Modos de jogo
│   ├── UI/          # Sistema de interface
│   └── Utils/       # Utilitários
├── Resources/       # Assets carregados em runtime
└── Settings/        # ScriptableObjects de configuração
```

### Principais Scripts

#### 🎮 Gameplay
- `GameFlowManager.cs` - Controle central do fluxo do jogo
- `RunnerMode.cs` - Modo corrida (Mundo 1)
- `BossMode.cs` - Modo boss (Mundo 2)
- `JonasPuzzleMode.cs` - Puzzle 3 etapas (Mundo 3)
- `MoisesPuzzleMode.cs` - Puzzle 4 etapas (Mundo 4)
- `JesusCollectMode.cs` - Modo coleta (Mundo 5)

#### 🔧 Sistemas
- `ObjectPool.cs` - Sistema de object pooling genérico
- `AdvancedAudioManager.cs` - Gerenciamento de áudio completo
- `QualityManager.cs` - Otimização gráfica adaptativa
- `AchievementManager.cs` - Sistema de conquistas
- `LocalizationManager.cs` - Internacionalização
- `EffectManager.cs` - Gerenciamento de partículas

#### 📊 Dados
- `SaveSystem.cs` - Persistência de dados
- `GameSettings.cs` - Configurações gerais (ScriptableObject)
- `LevelSettings.cs` - Configurações por nível
- `AudioSettings.cs` - Configurações de áudio
- `UISettings.cs` - Configurações de UI

## 🔧 Configuração

### ScriptableObjects
Todos os ScriptableObjects ficam em `Assets/Settings/`. Crie-os via:
- Right Click → Create → Heróis da Bíblia → [Setting Type]

### Audio Settings
Configure em `AudioSettings.asset`:
- Volumes (Master, Music, SFX)
- Clips de música por mundo
- Efeitos sonoros

### Quality Manager
Ajuste em `QualityManager`:
- Auto-detect: Detecta hardware automaticamente
- Adaptive Quality: Ajusta em runtime baseado em FPS
- Min FPS: Limite para redução de qualidade

## 📦 Build

### WebGL
1. File → Build Settings
2. Selecione WebGL
3. Configure:
   - Compression Format: Brotli
   - Code Optimization: Size
4. Build para `docs/` folder
5. Faça commit e push
6. GitHub Settings → Pages → Deploy from `main/docs`

### Android
1. File → Build Settings → Android
2. Player Settings:
   - Minimum API Level: 21 (Android 5.0)
   - Target API: Latest
   - Scripting Backend: IL2CPP
   - Target Architectures: ARM64
3. Build APK ou AAB

### iOS
1. File → Build Settings → iOS
2. Build e abra no Xcode
3. Configure assinatura e provisioning
4. Build no Xcode

## 🎨 Customização

### Adicionar Novo Mundo
1. Crie novo script herdando de `MonoBehaviour`
2. Implemente lógica do modo
3. Adicione ao `GameFlowManager.cs`
4. Crie UI correspondente em `RuntimeUI.cs`
5. Configure em `GameConstants.cs`

### Adicionar Nova Conquista
1. Adicione enum em `AchievementType`
2. Registre em `AchievementManager.InitializeAchievements()`
3. Chame `UnlockAchievement()` quando apropriado

### Adicionar Novo Idioma
1. Adicione enum em `Language`
2. Atualize todas as traduções em `LocalizationManager.InitializeTranslations()`

## 🐛 Debug

### Console Commands (Dev Build)
Adicione este código em `GameFlowManager.cs`:
```csharp
#if UNITY_EDITOR || DEVELOPMENT_BUILD
void Update() {
    if (Input.GetKeyDown(KeyCode.F1)) UnlockAllLevels();
    if (Input.GetKeyDown(KeyCode.F2)) AddCoins(1000);
    if (Input.GetKeyDown(KeyCode.F3)) AchievementManager.Instance.ResetAll();
}
#endif
```

### Performance Monitoring
- `QualityManager`: Mostra FPS em tempo real
- `ObjectPool`: Estatísticas de pools (showStats = true)
- Unity Profiler: Análise detalhada

## 📄 Licença

MIT License - veja [LICENSE](LICENSE) para detalhes.

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch (`git checkout -b feature/NovaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona NovaFeature'`)
4. Push para a branch (`git push origin feature/NovaFeature`)
5. Abra um Pull Request

## 📮 Contato

- **Desenvolvedor**: [Seu Nome]
- **Email**: seu.email@example.com
- **Website**: https://seu-site.com

## 🙏 Agradecimentos

- Unity Technologies
- Comunidade Unity Brasil
- Beta Testers

---
**⭐ Se este projeto te ajudou, considere dar uma estrela!**
