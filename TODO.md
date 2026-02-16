# 📋 TODO List - Heróis da Bíblia 3D

## 🔴 Pendente (Fase 2-3)

### Fase 2: Arquitetura Avançada

#### ❌ GameFlowManager Refactoring
**Prioridade:** Alta  
**Estimativa:** 3-4 horas  
**Status:** Not Started

**Problema:** GameFlowManager tem 253+ linhas e múltiplas responsabilidades:
- Controle de UI
- Gerenciamento de estados
- Setup de cenas
- Lógica de progressão
- Controle de todos os modos

**Solução:** Dividir em componentes menores:

```
GameFlowManager (Orquestrador - 100 linhas)
├── SceneSetup.cs (Setup inicial de cenas - 80 linhas)
├── GameStateController.cs (Máquina de estados - 100 linhas)
├── LevelManager.cs (Progressão e desbloqueio - 120 linhas)
└── ModeCoordinator.cs (Inicialização de modos - 80 linhas)
```

**Passos:**
1. [ ] Criar `SceneSetup.cs` - Handle camera, lighting, básicos
2. [ ] Criar `GameStateController.cs` - Enum State + transições
3. [ ] Criar `LevelManager.cs` - Load/save progression
4. [ ] Criar `ModeCoordinator.cs` - Factory para modos de jogo
5. [ ] Refatorar `GameFlowManager.cs` - Orquestrar componentes
6. [ ] Testar cada modo individualmente
7. [ ] Testar fluxo completo menu → modo → resultado
8. [ ] Validar save/load ainda funciona

**Dependências:** Nenhuma  
**Bloqueadores:** Nenhum

---

### Fase 3: Polish & UX

#### ❌ Sistema de Transições UI
**Prioridade:** Média  
**Estimativa:** 2-3 horas  
**Status:** Not Started

**Objetivo:** Adicionar animações profissionais nas transições UI

**Features:**
1. **Fade In/Out de Painéis**
   - CanvasGroup alpha: 0 → 1 em 0.3s
   - Fade out antes de mudar de tela
   
2. **Animações de Botões**
   - Scale: 1.0 → 1.1 → 1.0 no hover
   - Color tint no press
   - Feedback visual imediato
   
3. **Transição de Telas**
   - Slide in/out (200px em 0.4s)
   - Easing: EaseOutQuad
   
4. **Entrada de Elementos**
   - Stagger: 0.05s entre elementos
   - Sequência natural (top → bottom)

**Implementação:**

```csharp
// UIAnimator.cs
public class UIAnimator : MonoBehaviour {
    public static void FadeIn(CanvasGroup cg, float duration = 0.3f) {
        StartCoroutine(FadeRoutine(cg, 0, 1, duration));
    }
    
    public static void FadeOut(CanvasGroup cg, float duration = 0.3f) {
        StartCoroutine(FadeRoutine(cg, 1, 0, duration));
    }
    
    public static void SlideIn(RectTransform rt, Vector2 from, float duration = 0.4f) {
        StartCoroutine(SlideRoutine(rt, from, rt.anchoredPosition, duration));
    }
    
    public static void ButtonPress(Button btn) {
        StartCoroutine(PressRoutine(btn.transform));
    }
}
```

**Passos:**
1. [ ] Criar `UIAnimator.cs` com métodos básicos
2. [ ] Adicionar CanvasGroup aos painéis principais
3. [ ] Integrar fade in main menu
4. [ ] Integrar fade out antes de iniciar modo
5. [ ] Adicionar scale animation em todos botões
6. [ ] Testar performance (60 FPS mantido?)
7. [ ] Adicionar opção de disable em Settings (acessibilidade)

**Dependências:** UISettings.cs (tempos de animação)  
**Bloqueadores:** Nenhum

---

## 🟡 Backlog (Fase 5+)

### Features Futuras

#### 📚 Tutorial Interativo
**Prioridade:** Alta  
**Estimativa:** 4-5 horas

- [ ] Sistema de highlight de UI
- [ ] Tooltips contextuais
- [ ] First time player experience (FTPE)
- [ ] Skip tutorial option
- [ ] Save tutorial completion

**Tech:**
- TutorialManager singleton
- Highlight shader para UI
- Sequência de steps

---

#### ⚙️ Pause Menu Completo
**Prioridade:** Alta  
**Estimativa:** 2 horas

- [ ] Pause overlay com blur
- [ ] Slider de volume (Music/SFX separated)
- [ ] Toggle de qualidade gráfica
- [ ] Botão de restart level
- [ ] Botão de quit to menu

**Tech:**
- Time.timeScale = 0
- Blur shader ou overlay escurecido
- PlayerPrefs para settings

---

#### 🎁 Daily Rewards
**Prioridade:** Média  
**Estimativa:** 3 horas

- [ ] Sistema de streak (dias consecutivos)
- [ ] Recompensas crescentes (dia 7 = especial)
- [ ] UI de claim
- [ ] Timer até próxima recompensa
- [ ] Push notification (opcional)

**Tech:**
- DateTime.Now para tracking
- PlayerPrefs: lastClaimDate
- Progression: Day1=50 coins, Day7=500 coins

---

#### 🏆 Leaderboard Local
**Prioridade:** Média  
**Estimativa:** 2 horas

- [ ] Top 10 high scores por mundo
- [ ] Nome do jogador (input)
- [ ] UI de leaderboard
- [ ] Reset option

**Tech:**
- List<ScoreEntry> serializada
- Sort by score
- PlayerPrefs storage

---

#### 🎨 Shader Effects
**Prioridade:** Baixa  
**Estimativa:** 6+ horas

- [ ] Outline shader para heróis
- [ ] Dissolve effect para obstáculos
- [ ] Glow effect para coletáveis
- [ ] Water shader (Mar Vermelho)
- [ ] Cartoon shader (toon shading)

**Tech:**
- Shader Graph (URP)
- Custom shaders
- Material swapping

---

#### 🎵 Sound Design Completo
**Prioridade:** Média  
**Estimativa:** 4 horas (sem contar criação de assets)

- [ ] Música tema única por mundo
- [ ] Voiceover para personagens (opcional)
- [ ] SFX variações (não repetitivo)
- [ ] Audio mixer com ducking
- [ ] Reverb zones contextual

**Assets Necessários:**
- 5 músicas (1 por mundo)
- 20-30 SFX variados
- Walking/running footsteps
- Ambient sounds

---

## 🟢 Futuro Distante (Fase 6-7)

### Expansões de Conteúdo

#### 🌍 3 Novos Mundos
**Prioridade:** Baixa  
**Estimativa:** 15+ horas

Possíveis mundos:
- **Mundo 6:** Daniel na Cova dos Leões (Survival)
- **Mundo 7:** Ester (Strategy/Puzzle)
- **Mundo 8:** José no Egito (Resource Management)

Cada mundo requer:
- Game mode script
- Level design
- UI específica
- Storyline
- Assets 3D

---

#### ⚔️ Boss Rush Mode
**Prioridade:** Baixa  
**Estimativa:** 6 horas

- Derrote todos os bosses em sequência
- Timer global
- Dificuldade progressiva
- Recompensa especial ao completar
- Leaderboard de tempo

---

#### ⏱️ Time Trial Challenges
**Prioridade:** Baixa  
**Estimativa:** 4 horas

- Complete cada mundo no menor tempo
- Medalhas: Bronze, Silver, Gold
- Ghost racer (seu melhor tempo)
- Daily challenge aleatório

---

#### 🌐 Multiplayer Assíncrono
**Prioridade:** Muito Baixa  
**Estimativa:** 20+ horas

- Compare scores com amigos
- Ghost racers de outros jogadores
- Weekly tournaments
- Social features (share, invite)

**Tech Stack:**
- Backend: Firebase ou PlayFab
- Authentication necessária
- Leaderboard global

---

#### 💰 Monetização (Opcional)
**Prioridade:** Baixa  
**Estimativa:** 8+ horas

**Ads (Opcional):**
- Rewarded video (2x coins)
- Interstitial após 3 game overs
- Unity Ads integration

**IAP:**
- Skin packs premium (R$ 4.99)
- Coin bundles (R$ 2.99, R$ 9.99, R$ 19.99)
- Remove ads (R$ 6.99)
- Season Pass (R$ 12.99)

**Tech:**
- Unity IAP package
- Receipt validation
- Restore purchases

---

## 📊 Métricas de Sucesso

### KPIs Phase 2-3
- [ ] Build sem warnings
- [ ] 60 FPS constante em mobile mid-range
- [ ] Tempo de load < 3s
- [ ] Crash rate < 0.1%
- [ ] Code coverage > 70%

### KPIs Phase 5+
- [ ] Tutorial completion rate > 80%
- [ ] D1 retention > 40%
- [ ] D7 retention > 20%
- [ ] Avg session time > 10min
- [ ] 5-star rating > 4.5

---

## 🛠️ Technical Debt

### Code Quality
- [ ] Unit tests para managers críticos
- [ ] CI/CD pipeline (GitHub Actions)
- [ ] Code review checklist
- [ ] Performance benchmarks automáticos

### Documentation
- [ ] XML docs em 100% dos métodos públicos
- [ ] Architecture Decision Records (ADR)
- [ ] API reference auto-gerada (Doxygen)
- [ ] Video tutorials para novos devs

---

## 📝 Notas

### Decisões de Design
- **Portrait-only:** Simplifica UI, foco mobile
- **ScriptableObjects:** Data-driven, fácil balancear
- **Object Pooling:** Essencial para mobile performance
- **No networking:** Scope menor, menos complexidade

### Lessons Learned
- ✅ Formatação de código é CRÍTICA para manutenção
- ✅ Documentação inline economiza horas depois
- ✅ Object pooling teve impacto massivo (60-80% improvement)
- ✅ Adaptive quality salvou devices low-end

### Next Developer Notes
- Sempre teste em device real, não só emulador
- Profile antes de otimizar (measure twice, cut once)
- Pequenas features bem feitas > muitas features buggy
- User feedback é ouro, implemente analytics cedo

---

**Última Atualização:** 2026-02-16  
**Maintainer:** [Seu Nome]  
**Status:** ✅ 83% das melhorias críticas completas (Fase 1-4)
