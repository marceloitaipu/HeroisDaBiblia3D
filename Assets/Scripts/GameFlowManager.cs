using UnityEngine;

namespace HeroisDaBiblia3D
{
    public enum GameState{Home,Map,RunnerNoe,BossDavi,JonasPuzzle,MoisesPuzzle,JesusMenu,JesusGameplay}

    public sealed class GameFlowManager : MonoBehaviour
    {
        public GameState State { get; private set; } = GameState.Home;
        public SaveData Save { get; private set; }

        RuntimeUI _ui;
        Transform _player;
        RunnerMode _runner;
        BossMode _boss;
        JonasPuzzleMode _jonas;
        MoisesPuzzleMode _moises;
        JesusCollectMode _jesus;
        HeroCustomizer _custom;
        InputRouter _input;
        UIAnimator _animator;
        PauseManager _pause;

        // Tracking para conquistas
        private float _totalPlayTime;
        private int _totalScrollsCollected;
        private int _totalHeartsCollected;
        private bool _runnerNoHits;

        void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.orientation = ScreenOrientation.Portrait;

            Save = SaveSystem.Load();

            // --- Inicializa Managers (anteriormente dead code) ---

            // Audio simples (usado pelo jogo)
            var am = new GameObject("AudioManager").AddComponent<AudioManager>();
            am.uiBeep = Resources.Load<AudioClip>("beep");

            // Quality Manager — detecção automática de hardware e FPS adaptativo
            var qmGo = new GameObject("QualityManager");
            qmGo.AddComponent<QualityManager>();

            // Localization Manager — i18n com detecção automática de idioma
            var locGo = new GameObject("LocalizationManager");
            locGo.AddComponent<LocalizationManager>();

            // Achievement Manager — sistema de conquistas
            var achGo = new GameObject("AchievementManager");
            var achMgr = achGo.AddComponent<AchievementManager>();
            achMgr.OnAchievementUnlocked += OnAchievementUnlocked;

            // --- Cena ---

            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>(); light.type = LightType.Directional; light.intensity = 1.15f;
            lightGo.transform.rotation = Quaternion.Euler(55, -35, 0);

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = new Vector3(0, 0, 45);
            ground.transform.localScale = new Vector3(0.35f, 1f, 18f);
            var gmat = new Material(GameConstants.SafeStandardShader); gmat.color = new Color(0.70f, 0.86f, 0.72f);
            ground.GetComponent<Renderer>().material = gmat;

            // --- Player ---

            var p = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            p.name = "Player"; p.tag = "Player";
            p.transform.position = new Vector3(0, 1.1f, 2);
            var rb = p.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            _runner = p.AddComponent<RunnerMode>(); _runner.enabled = false;
            _boss = p.AddComponent<BossMode>(); _boss.enabled = false;
            _jonas = p.AddComponent<JonasPuzzleMode>(); _jonas.enabled = false;
            _moises = p.AddComponent<MoisesPuzzleMode>(); _moises.enabled = false;
            _jesus = p.AddComponent<JesusCollectMode>(); _jesus.enabled = false;

            _custom = p.AddComponent<HeroCustomizer>();
            _player = p.transform;

            _input = new GameObject("Input").AddComponent<InputRouter>();

            // --- Camera ---

            var camGo = new GameObject("Main Camera"); camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>(); cam.fieldOfView = 58;
            var follow = camGo.AddComponent<PortraitFollowCamera>(); follow.target = _player;

            // --- UI ---

            _animator = new GameObject("UIAnimator").AddComponent<UIAnimator>();
            DontDestroyOnLoad(_animator.gameObject);

            _ui = new GameObject("UI").AddComponent<RuntimeUI>();
            _ui.Bind(this, _input, _animator);

            // --- Pause ---

            _pause = new GameObject("PauseManager").AddComponent<PauseManager>();

            ApplyCosmetics();
            GoHome();
        }

        void Update()
        {
            // Tracking de tempo total de jogo (para conquista PlayFor1Hour)
            _totalPlayTime += Time.deltaTime;
            if (_totalPlayTime >= 3600f && AchievementManager.Instance != null)
            {
                AchievementManager.Instance.UnlockAchievement(AchievementType.PlayFor1Hour);
            }

            var axis = _ui != null ? _ui.JoystickValue : Vector2.zero;

            if (State == GameState.RunnerNoe && _runner.enabled)
            {
                _runner.SetLaneByAxis(axis.x);
                if (_input.jumpPressed) _runner.Jump();
                if (_input.slidePressed) _runner.Slide();
                _input.Consume();
            }
            else if (State == GameState.BossDavi && _boss.enabled)
            {
                _boss.SetLaneByAxis(axis.x);
                if (_input.actionPressed) _boss.Action();
                _input.Consume();
            }
            else if (State == GameState.JesusGameplay && _jesus.enabled)
            {
                _jesus.SetLaneByAxis(axis.x);
                _ui.RefreshJesus(_jesus.hearts, _jesus.heartsTarget);
            }
        }

        #region Navegação

        public void GoHome()
        {
            Cleanup();
            State = GameState.Home;
            _ui.ShowHome();
            SetupPause(null, null); // Desativa pause no menu
        }

        public void GoMap()
        {
            Cleanup();
            State = GameState.Map;
            _ui.ShowMap();
            SetupPause(null, null);
        }

        #endregion

        #region Modos de Jogo

        public void StartRunnerNoe()
        {
            Cleanup(); State = GameState.RunnerNoe;
            _ui.ShowHud(L("world1_title", "Mundo 1 — Noé e a Arca"));

            _runner.enabled = true;
            _runner.ResetRun();
            _runnerNoHits = true; // Tracking para conquista RunnerNoHits

            SetupPause(() => StartRunnerNoe(), () => GoMap());

            _runner.onRunFinished = () =>
            {
                Save.virtues += 1;
                int coinsEarned = Mathf.Clamp(_runner.pergaminhos, 10, 120);
                Save.coins += coinsEarned;
                _totalScrollsCollected += _runner.pergaminhos;
                if (Save.worldsUnlocked < 2) Save.worldsUnlocked = 2;
                SaveSystem.Save(Save);

                // Conquistas
                TryUnlock(AchievementType.CompleteWorld1);
                TryUnlock(AchievementType.FirstVictory);
                if (_runnerNoHits) TryUnlock(AchievementType.RunnerNoHits);
                CheckCoinAchievements();
                CheckScrollAchievements();
                CheckAllWorldsComplete();

                _ui.Quiz(
                    L("quiz_title", "Mini-Desafio"),
                    L("quiz_noe", "Deus chamou Noé para fazer o quê?"),
                    new[] { L("quiz_noe_a1", "Construir a arca"), L("quiz_noe_a2", "Plantar uma horta"), L("quiz_noe_a3", "Virar rei") },
                    0,
                    L("virtue_obedience", "Obediência"),
                    () => GoMap()
                );
            };

            _runner.onPlayerHit = () =>
            {
                _runnerNoHits = false;
                _ui.Modal(L("oops", "Ops!"), L("hit_obstacle", "Você esbarrou num obstáculo. Tente de novo 😊"),
                    L("restart", "Recomeçar"), () => StartRunnerNoe(),
                    L("map", "Mapa"), () => GoMap());
            };
        }

        public void StartBossDavi()
        {
            if (Save.worldsUnlocked < 2)
            {
                _ui.Modal(L("locked", "Bloqueado"), L("complete_prev_1", "Complete o Mundo 1 primeiro."), "OK", () => GoMap());
                return;
            }

            Cleanup(); State = GameState.BossDavi;
            _ui.ShowHud(L("world2_title", "Mundo 2 — Davi e Golias"));

            _boss.enabled = true;
            _boss.ResetBoss();
            _boss.onHud = (s, h, n) => _ui.SetBossHud(s, h, n);

            SetupPause(() => StartBossDavi(), () => GoMap());

            _boss.onBossWin = () =>
            {
                Save.virtues += 2; Save.coins += 80;
                if (Save.worldsUnlocked < 3) Save.worldsUnlocked = 3;
                SaveSystem.Save(Save);

                TryUnlock(AchievementType.CompleteWorld2);
                TryUnlock(AchievementType.FirstVictory);
                TryUnlock(AchievementType.DefeatGoliathPerfect); // TODO: track perfect aim
                CheckCoinAchievements();
                CheckAllWorldsComplete();

                _ui.Modal(L("victory", "Vitória!"),
                    L("boss_win_msg", "Você venceu com coragem e estratégia! 🌟\nVirtudes: Coragem + Fé"),
                    L("map", "Mapa"), () => GoMap());
            };

            _boss.onPlayerFail = () =>
                _ui.Modal(L("try_again", "Quase!"),
                    L("boss_fail_msg", "Golias te assustou, mas você pode tentar de novo 💪"),
                    L("retry", "Tentar de novo"), () => StartBossDavi(),
                    L("map", "Mapa"), () => GoMap());
        }

        public void StartJonas()
        {
            if (Save.worldsUnlocked < 3)
            {
                _ui.Modal(L("locked", "Bloqueado"), L("complete_prev_2", "Complete o Mundo 2 primeiro."), "OK", () => GoMap());
                return;
            }

            Cleanup(); State = GameState.JonasPuzzle;
            _jonas.enabled = true; _jonas.ResetPuzzle();
            _ui.ShowJonas();

            SetupPause(() => StartJonas(), () => GoMap());

            _jonas.onWin = () =>
            {
                Save.virtues += 2; Save.coins += 70;
                if (Save.worldsUnlocked < 4) Save.worldsUnlocked = 4;
                SaveSystem.Save(Save);

                TryUnlock(AchievementType.CompleteWorld3);
                TryUnlock(AchievementType.FirstVictory);
                CheckCoinAchievements();
                CheckVirtueAchievements();
                CheckAllWorldsComplete();

                _ui.Modal(L("great_job", "Muito bem!"),
                    L("jonas_win_msg", "Você colocou a história na ordem certa!\nVirtude: Obediência"),
                    L("map", "Mapa"), () => GoMap());
            };

            _jonas.onFail = () =>
                _ui.Modal(L("try_again", "Quase!"),
                    L("puzzle_fail", "Não foi essa ordem. Tente de novo 😊"),
                    L("retry", "Tentar"), () => { _jonas.ResetPuzzle(); _ui.ShowJonas(); },
                    L("map", "Mapa"), () => GoMap());
        }

        public void StartMoises()
        {
            if (Save.worldsUnlocked < 4)
            {
                _ui.Modal(L("locked", "Bloqueado"), L("complete_prev_3", "Complete o Mundo 3 primeiro."), "OK", () => GoMap());
                return;
            }

            Cleanup(); State = GameState.MoisesPuzzle;
            _moises.enabled = true; _moises.ResetPuzzle();
            _ui.ShowMoises();

            SetupPause(() => StartMoises(), () => GoMap());

            _moises.onWin = () =>
            {
                Save.virtues += 2; Save.coins += 90;
                if (Save.worldsUnlocked < 5) Save.worldsUnlocked = 5;
                SaveSystem.Save(Save);

                TryUnlock(AchievementType.CompleteWorld4);
                TryUnlock(AchievementType.FirstVictory);
                CheckCoinAchievements();
                CheckVirtueAchievements();
                CheckAllWorldsComplete();

                _ui.Modal(L("amazing", "Incrível!"),
                    L("moises_win_msg", "Você lembrou a sequência do milagre!\nVirtude: Fé"),
                    L("map", "Mapa"), () => GoMap());
            };

            _moises.onFail = () =>
                _ui.Modal(L("try_again", "Quase!"),
                    L("puzzle_fail", "Não foi essa ordem. Tente de novo 😊"),
                    L("retry", "Tentar"), () => { _moises.ResetPuzzle(); _ui.ShowMoises(); },
                    L("map", "Mapa"), () => GoMap());
        }

        public void StartJesusMenu()
        {
            if (Save.worldsUnlocked < 5)
            {
                _ui.Modal(L("locked", "Bloqueado"), L("complete_prev_4", "Complete o Mundo 4 primeiro."), "OK", () => GoMap());
                return;
            }

            Cleanup(); State = GameState.JesusMenu;
            _ui.ShowJesus();
        }

        public void StartJesusGameplay()
        {
            Cleanup(); State = GameState.JesusGameplay;
            _ui.ShowHud(L("world5_title", "Mundo 5 — Jesus (Coleta)"));

            _jesus.enabled = true;
            _jesus.ResetMode();
            _ui.RefreshJesus(_jesus.hearts, _jesus.heartsTarget);

            SetupPause(() => StartJesusGameplay(), () => GoMap());

            _jesus.onWin = () =>
            {
                Save.virtues += 2; Save.coins += 100;
                _totalHeartsCollected += _jesus.hearts;
                SaveSystem.Save(Save);

                TryUnlock(AchievementType.CompleteWorld5);
                TryUnlock(AchievementType.FirstVictory);
                CheckCoinAchievements();
                CheckHeartAchievements();
                CheckVirtueAchievements();
                CheckAllWorldsComplete();

                _ui.Modal(L("congratulations", "Parabéns!"),
                    L("jesus_win_msg", "Você coletou amor e bondade no caminho 💗\nVirtudes: Amor + Bondade"),
                    L("map", "Mapa"), () => GoMap());
            };
        }

        #endregion

        #region Puzzle Picks

        public void JonasPick(int step){ _jonas.Pick(step); _ui.RefreshJonas(); }
        public void MoisesPick(int step){ _moises.Pick(step); _ui.RefreshMoises(); }

        #endregion

        #region Customização

        public void SetHero(int idx)
        {
            Save.selectedHero = Mathf.Clamp(idx, 0, 2);
            SaveSystem.Save(Save);
            ApplyCosmetics();
            _ui.RefreshFromSave();
        }

        public void SelectSkin(int idx)
        {
            idx = Mathf.Clamp(idx, 0, GameConstants.MaxSkins - 1);
            if (!Save.ownedSkins[idx])
            {
                _ui.Modal(L("skin_locked", "Skin bloqueada"),
                    L("buy_first", "Compre a skin na loja primeiro."), "OK", () => _ui.ShowHero());
                return;
            }
            Save.selectedSkin = idx;
            SaveSystem.Save(Save);
            ApplyCosmetics();
            _ui.RefreshFromSave();
        }

        public void BuySkin(int idx, int price)
        {
            idx = Mathf.Clamp(idx, 0, GameConstants.MaxSkins - 1);
            if (Save.ownedSkins[idx])
            {
                _ui.Modal(L("already_own", "Já possui"), L("you_have_skin", "Você já tem essa skin ✅"), "OK", () => _ui.ShowShop());
                return;
            }
            if (Save.coins < price)
            {
                _ui.Modal(L("not_enough_coins", "Moedas insuficientes"),
                    string.Format(L("need_coins", "Você precisa de {0} moedas."), price), "OK", () => _ui.ShowShop());
                return;
            }

            Save.coins -= price;
            Save.ownedSkins[idx] = true;
            SaveSystem.Save(Save);

            TryUnlock(AchievementType.BuyFirstSkin);
            bool allSkins = true;
            for (int i = 0; i < Save.ownedSkins.Length; i++)
                if (!Save.ownedSkins[i]) allSkins = false;
            if (allSkins) TryUnlock(AchievementType.UnlockAllSkins);

            _ui.Modal(L("purchased", "Comprado!"), L("skin_unlocked", "Skin liberada ✅"), "OK", () => _ui.ShowShop());
            _ui.RefreshFromSave();
        }

        public void ResetProgress()
        {
            SaveSystem.Reset();
            Save = SaveSystem.Load();
            ApplyCosmetics();
        }

        #endregion

        #region Conquistas

        private void TryUnlock(AchievementType type)
        {
            if (AchievementManager.Instance != null)
                AchievementManager.Instance.UnlockAchievement(type);
        }

        private void CheckCoinAchievements()
        {
            if (Save.coins >= 100) TryUnlock(AchievementType.Collect100Coins);
            if (Save.coins >= 500) TryUnlock(AchievementType.Collect500Coins);
            if (Save.coins >= 1000) TryUnlock(AchievementType.Collect1000Coins);
        }

        private void CheckScrollAchievements()
        {
            if (_totalScrollsCollected >= 50) TryUnlock(AchievementType.Collect50Scrolls);
        }

        private void CheckHeartAchievements()
        {
            if (_totalHeartsCollected >= 100) TryUnlock(AchievementType.Collect100Hearts);
        }

        private void CheckVirtueAchievements()
        {
            if (Save.virtues >= 10) TryUnlock(AchievementType.Earn10Virtues);
            if (Save.virtues >= 25) TryUnlock(AchievementType.Earn25Virtues);
        }

        private void CheckAllWorldsComplete()
        {
            if (Save.worldsUnlocked >= 6 ||
                (AchievementManager.Instance != null &&
                 AchievementManager.Instance.IsUnlocked(AchievementType.CompleteWorld1) &&
                 AchievementManager.Instance.IsUnlocked(AchievementType.CompleteWorld2) &&
                 AchievementManager.Instance.IsUnlocked(AchievementType.CompleteWorld3) &&
                 AchievementManager.Instance.IsUnlocked(AchievementType.CompleteWorld4) &&
                 AchievementManager.Instance.IsUnlocked(AchievementType.CompleteWorld5)))
            {
                TryUnlock(AchievementType.CompleteAllWorlds);
            }
        }

        private void OnAchievementUnlocked(Achievement achievement)
        {
            // Mostra notificação visual de conquista
            if (_animator != null && _ui != null)
            {
                _animator.ShowNotification(_ui.Canvas,
                    $"🏆 {achievement.title} (+ {achievement.coinReward} moedas)");
            }

            // Adiciona recompensa em moedas
            Save.coins += achievement.coinReward;
            SaveSystem.Save(Save);
        }

        #endregion

        #region Localização Helper

        /// <summary>
        /// Helper de localização com fallback para português.
        /// </summary>
        private string L(string key, string fallback)
        {
            if (LocalizationManager.Instance != null)
            {
                string result = LocalizationManager.Instance.GetString(key);
                if (result != key) return result; // Encontrou tradução
            }
            return fallback;
        }

        #endregion

        #region Pause

        private void SetupPause(System.Action onRestart, System.Action onQuit)
        {
            if (_pause == null) return;

            if (onRestart == null && onQuit == null)
            {
                // Desativa pause em menus
                _pause.enabled = false;
                return;
            }

            _pause.enabled = true;
            _pause.onRestart = onRestart;
            _pause.onQuit = onQuit;

            if (_ui != null && _ui.Canvas != null)
                _pause.Initialize(_ui.Canvas, _animator);
        }

        #endregion

        #region Internals

        void ApplyCosmetics() { _custom?.Apply(Save); }

        void Cleanup()
        {
            // Garante resume se estava pausado
            if (_pause != null && _pause.IsPaused)
                _pause.Resume();

            if (_runner != null) _runner.enabled = false;
            if (_boss != null) _boss.enabled = false;
            if (_jonas != null) _jonas.enabled = false;
            if (_moises != null) _moises.enabled = false;
            if (_jesus != null) _jesus.enabled = false;

            if (_player != null)
            {
                var rb = _player.GetComponent<Rigidbody>();
                if (rb != null) { rb.isKinematic = false; rb.velocity = Vector3.zero; }
                _player.position = new Vector3(0, 1.1f, 2);
            }

            foreach (var o in GameObject.FindGameObjectsWithTag("Obstacle")) Destroy(o);
            foreach (var c in GameObject.FindGameObjectsWithTag("Collectible")) Destroy(c);
            foreach (var b in GameObject.FindGameObjectsWithTag("Boss")) Destroy(b);
        }

        #endregion
    }
}
