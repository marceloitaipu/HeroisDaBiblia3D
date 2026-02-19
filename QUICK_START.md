# 🚀 Guia Rápido - Heróis da Bíblia 3D

## Setup Inicial (5 minutos)

### 1. Primeira Execução
```
1. Abra o projeto no Unity 6 (6000.3.9f1)
2. Aguarde a compilação dos scripts
3. Abra Assets/Scenes/Main.unity
4. Clique em Play
```

### 2. Criar Assets de Configuração

#### Game Settings
```
1. Right Click na pasta Assets/Settings
2. Create → Heróis da Bíblia → Game Settings
3. Configure:
   - Target FPS: 60
   - Quality Level: Medium
   - Coin Multiplier: 1.0
   - Debug Mode: True (em desenvolvimento)
```

#### Audio Settings
```
1. Create → Heróis da Bíblia → Audio Settings
2. Configure:
   - Master Volume: 1.0
   - Music Volume: 0.7
   - SFX Volume: 1.0
   - Arraste clips de áudio para os slots
```

#### Level Settings (para cada mundo)
```
1. Create → Heróis da Bíblia → Level Settings
2. Nomeie: "Level1_Runner", "Level2_Boss", etc
3. Configure:
   - Initial Speed: 5.0
   - Target Distance: 100
   - Coin Reward: 50
   - Difficulty: Easy/Medium/Hard
```

### 3. Conectar Managers aos GameObjects

#### AudioManager
```
1. Crie GameObject vazio: "AudioManager"
2. Adicione componente AdvancedAudioManager
3. Arraste AudioSettings asset para o campo settings
```

#### QualityManager
```
1. Crie GameObject vazio: "QualityManager"
2. Adicione componente QualityManager
3. Configure:
   - Auto Detect: True
   - Enable Adaptive: True
   - Target FPS: 60
```

#### AchievementManager
```
1. Crie GameObject vazio: "AchievementManager"
2. Adicione componente AchievementManager
3. Deixe Initialize On Awake: True
```

#### LocalizationManager
```
1. Crie GameObject vazio: "LocalizationManager"
2. Adicione componente LocalizationManager
3. Deixe Auto Detect Language: True
```

#### EffectManager
```
1. Crie GameObject vazio: "EffectManager"
2. Adicione componente EffectManager
3. Arraste prefabs de partículas para os slots:
   - Collect Effect
   - Jump Effect
   - Hit Effect
   - Victory Effect
   - Level Up Effect
```

## Workflow de Desenvolvimento

### Criar Novo Obstáculo (Runner Mode)
```csharp
1. Crie prefab do obstáculo
2. Em GameFlowManager.cs, método SetupRunnerObstacles():
   
   var pool = ObjectPool.Create(obstaclePrefab, 20);
   
   // Spawnar:
   var obj = pool.Get();
   obj.transform.position = spawnPos;
   
   // Retornar ao pool após X segundos:
   ObjectPool.ReturnDelayed(obj, 5f);
```

### Adicionar Novo SFX
```csharp
1. Importe o arquivo .wav para Assets/Audio/
2. Adicione ao AudioSettings asset
3. No código:
   
   AdvancedAudioManager.Instance.PlaySFX(meuClip);
   // ou
   AdvancedAudioManager.Instance.PlayHit();
```

### Criar Nova Conquista
```csharp
1. Em AchievementManager.cs:
   
   public enum AchievementType {
       // ... existentes
       MinhaNovaConquista,
   }
   
2. Em InitializeAchievements():
   
   achievements[AchievementType.MinhaNovaConquista] = new Achievement {
       id = "nova_conquista",
       title = "Título",
       description = "Descrição",
       coinReward = 100,
       isUnlocked = false
   };
   
3. No código do jogo:
   
   AchievementManager.Instance.UnlockAchievement(AchievementType.MinhaNovaConquista);
```

### Adicionar Tradução
```csharp
Em LocalizationManager.cs, método InitializeTranslations():

translations["minha_chave"] = new Dictionary<Language, string> {
    { Language.Portuguese, "Olá Mundo" },
    { Language.English, "Hello World" },
    { Language.Spanish, "Hola Mundo" }
};

// Usar no código:
string texto = LocalizationManager.Instance.Get("minha_chave");
```

## Comandos Debug (Editor)

### Console Commands
Adicione em `GameFlowManager.cs`:

```csharp
#if UNITY_EDITOR
void Update() {
    // F1 - Desbloquear todos mundos
    if (Input.GetKeyDown(KeyCode.F1)) {
        for (int i = 1; i <= 5; i++) {
            SaveData.Load().SetWorldUnlocked(i, true);
        }
        Debug.Log("Todos mundos desbloqueados");
    }
    
    // F2 - Adicionar 1000 moedas
    if (Input.GetKeyDown(KeyCode.F2)) {
        var save = SaveData.Load();
        save.coins += 1000;
        save.Save();
        Debug.Log($"Moedas: {save.coins}");
    }
    
    // F3 - Resetar conquistas
    if (Input.GetKeyDown(KeyCode.F3)) {
        AchievementManager.Instance.ResetAllAchievements();
        Debug.Log("Conquistas resetadas");
    }
    
    // F4 - Resetar save completo
    if (Input.GetKeyDown(KeyCode.F4)) {
        SaveData.ResetAll();
        Debug.Log("Save resetado");
    }
    
    // F5 - Alternar qualidade
    if (Input.GetKeyDown(KeyCode.F5)) {
        var qm = QualityManager.Instance;
        qm.SetQualityLevel((QualityTier)(((int)qm.currentTier + 1) % 3));
    }
}
#endif
```

### Inspetor Customizado
Para ver stats em runtime, adicione ao `ObjectPool.cs`:

```csharp
[Header("Debug")]
[SerializeField] private bool showStats = true;

void OnGUI() {
    if (!showStats) return;
    
    GUILayout.BeginArea(new Rect(10, 10, 300, 200));
    GUILayout.Label($"Pool: {prefab.name}");
    GUILayout.Label($"Size: {pool.Count}");
    GUILayout.Label($"Active: {activeObjects.Count}");
    GUILayout.Label($"Gets: {totalGets}");
    GUILayout.Label($"Returns: {totalReturns}");
    GUILayout.EndArea();
}
```

## Build & Deploy

### WebGL (GitHub Pages)
```bash
# 1. Build
File → Build Settings → WebGL → Build
Target: docs/

# 2. Deploy
git add docs/
git commit -m "Build WebGL"
git push origin main

# 3. Ativar Pages
GitHub → Settings → Pages → Source: main/docs → Save
```

### Android APK
```bash
# 1. Configure
File → Build Settings → Android
Player Settings:
  - API Level: 21+
  - Backend: IL2CPP
  - ARM64: Checked
  
# 2. Build
Build Settings → Build → herois_biblia.apk

# 3. Testar
adb install herois_biblia.apk
adb logcat -s Unity
```

### iOS
```bash
# 1. Build Unity
File → Build Settings → iOS → Build
Target: ios_build/

# 2. Xcode
Open ios_build/Unity-iPhone.xcodeproj
Signing: Automatic
Build → Archive → Distribute
```

## Troubleshooting

### "Modo X não inicia"
```
Verifique:
1. GameObject do modo está ativo?
2. GameFlowManager está chamando Setup() correto?
3. Console mostra erros?
```

### "Sem áudio"
```
Verifique:
1. AudioSettings asset criado?
2. AdvancedAudioManager no GameObject?
3. Clips arrastados para os slots?
4. Volume > 0 no Inspector?
```

### "FPS baixo"
```
Soluções:
1. QualityManager → Enable Adaptive Quality
2. Reduza Max Pool Size nos Object Pools
3. Profile → CPU/Rendering
4. Desative vsync: QualitySettings.vSyncCount = 0
```

### "Crash ao salvar"
```
Verifique:
1. SaveData.cs tem try-catch?
2. Path de save é válido?
3. Permissões de escrita (mobile)?
4. JSON não está corrompido?
```

## Performance Tips

### Object Pooling
✅ **Usar para:**
- Obstáculos (Runner)
- Coletáveis (Coins, Hearts)
- Efeitos de partículas
- Projéteis (Boss)
- UI elements temporários

❌ **NÃO usar para:**
- GameObjects estáticos
- Managers (Singletons)
- Objetos únicos na cena

### Memory Optimization
```csharp
// Ruim
void Update() {
    string texto = "Score: " + score; // Aloca string toda frame
}

// Bom
private StringBuilder sb = new StringBuilder();
void Update() {
    sb.Clear();
    sb.Append("Score: ");
    sb.Append(score);
    string texto = sb.ToString();
}
```

### Mobile Battery
```csharp
// Configure no inicio
Application.targetFrameRate = 60; // Não deixe ilimitado
Screen.sleepTimeout = SleepTimeout.NeverSleep; // Previne sleep durante gameplay
QualitySettings.vSyncCount = 1; // Sincroniza com display
```

## Referências Rápidas

### Constantes Importantes
```csharp
GameConstants.LanesX // Posições X das lanes: -2.5f, 0f, 2.5f
GameConstants.CollectibleValue // Valor base de coletáveis: 10
GameConstants.ObstaclePoolSize // Tamanho dos pools: 20
```

### Eventos de Save
```csharp
SaveData save = SaveData.Load(); // Carrega save atual
save.coins += 100; // Modifica
save.SetWorldUnlocked(3, true); // Desbloqueia mundo
save.Save(); // Persiste
```

### Timeline de Chamadas
```
1. Awake() - Inicializa singletons
2. Start() - Setup de referências
3. GameFlowManager.SetupMainMenu()
4. User Click → StartWorld(N)
5. GameFlowManager.StartRunnerMode() / StartBossMode() / etc
6. Modo roda até OnVictory() / OnDefeat()
7. GameFlowManager mostra resultado
8. Back to menu ou próximo mundo
```

---
**⚡ Dica:** Mantenha este guia aberto enquanto desenvolve!
