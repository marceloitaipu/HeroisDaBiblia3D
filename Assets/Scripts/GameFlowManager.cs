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

        void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.orientation = ScreenOrientation.Portrait;

            Save = SaveSystem.Load();

            var am = new GameObject("AudioManager").AddComponent<AudioManager>();
            am.uiBeep = Resources.Load<AudioClip>("beep");

            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>(); light.type = LightType.Directional; light.intensity = 1.15f;
            lightGo.transform.rotation = Quaternion.Euler(55, -35, 0);

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = new Vector3(0, 0, 45);
            ground.transform.localScale = new Vector3(0.35f, 1f, 18f);
            var gmat = new Material(Shader.Find("Standard")); gmat.color = new Color(0.70f, 0.86f, 0.72f);
            ground.GetComponent<Renderer>().material = gmat;

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

            var camGo = new GameObject("Main Camera"); camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>(); cam.fieldOfView = 58;
            var follow = camGo.AddComponent<PortraitFollowCamera>(); follow.target = _player;

            _ui = new GameObject("UI").AddComponent<RuntimeUI>();
            _ui.Bind(this, _input);

            ApplyCosmetics();
            GoHome();
        }

        void Update()
        {
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
                _ui.SetBossHud(0,0,3); // BossMode atualiza via callback, isso é só fallback
            }
            else if (State == GameState.JesusGameplay && _jesus.enabled)
            {
                _jesus.SetLaneByAxis(axis.x);
                _ui.RefreshJesus(_jesus.hearts, _jesus.heartsTarget);
            }
        }

        public void GoHome(){ Cleanup(); State = GameState.Home; _ui.ShowHome(); }
        public void GoMap(){ Cleanup(); State = GameState.Map; _ui.ShowMap(); }

        public void StartRunnerNoe()
        {
            Cleanup(); State = GameState.RunnerNoe;
            _ui.ShowHud("Mundo 1 — Noé e a Arca");

            _runner.enabled = true;
            _runner.ResetRun();

            _runner.onRunFinished = () =>
            {
                Save.virtues += 1;
                Save.coins += Mathf.Clamp(_runner.pergaminhos, 10, 120);
                if (Save.worldsUnlocked < 2) Save.worldsUnlocked = 2;
                SaveSystem.Save(Save);

                _ui.Quiz(
                    "Mini-Desafio",
                    "Deus chamou Noé para fazer o quê?",
                    new[] { "Construir a arca", "Plantar uma horta", "Virar rei" },
                    0,
                    "Obediência",
                    () => GoMap()
                );
            };

            _runner.onPlayerHit = () =>
                _ui.Modal("Ops!", "Você esbarrou num obstáculo. Tente de novo 😊", "Recomeçar", () => StartRunnerNoe(), "Mapa", () => GoMap());
        }

        public void StartBossDavi()
        {
            if (Save.worldsUnlocked < 2){ _ui.Modal("Bloqueado", "Complete o Mundo 1 primeiro.", "OK", () => GoMap()); return; }

            Cleanup(); State = GameState.BossDavi;
            _ui.ShowHud("Mundo 2 — Davi e Golias");

            _boss.enabled = true;
            _boss.ResetBoss();
            _boss.onHud = (s, h, n) => _ui.SetBossHud(s, h, n);
            _boss.onBossWin = () =>
            {
                Save.virtues += 2; Save.coins += 80;
                if (Save.worldsUnlocked < 3) Save.worldsUnlocked = 3;
                SaveSystem.Save(Save);
                _ui.Modal("Vitória!", "Você venceu com coragem e estratégia! 🌟\nVirtudes: Coragem + Fé", "Mapa", () => GoMap());
            };
            _boss.onPlayerFail = () => _ui.Modal("Quase!", "Golias te assustou, mas você pode tentar de novo 💪", "Tentar de novo", () => StartBossDavi(), "Mapa", () => GoMap());
        }

        public void StartJonas()
        {
            if (Save.worldsUnlocked < 3){ _ui.Modal("Bloqueado", "Complete o Mundo 2 primeiro.", "OK", () => GoMap()); return; }

            Cleanup(); State = GameState.JonasPuzzle;
            _jonas.enabled = true; _jonas.ResetPuzzle();
            _ui.ShowJonas();

            _jonas.onWin = () =>
            {
                Save.virtues += 2; Save.coins += 70;
                if (Save.worldsUnlocked < 4) Save.worldsUnlocked = 4;
                SaveSystem.Save(Save);
                _ui.Modal("Muito bem!", "Você colocou a história na ordem certa!\nVirtude: Obediência", "Mapa", () => GoMap());
            };
            _jonas.onFail = () => _ui.Modal("Quase!", "Não foi essa ordem. Tente de novo 😊", "Tentar", () => { _jonas.ResetPuzzle(); _ui.ShowJonas(); }, "Mapa", () => GoMap());
        }

        public void StartMoises()
        {
            if (Save.worldsUnlocked < 4){ _ui.Modal("Bloqueado", "Complete o Mundo 3 primeiro.", "OK", () => GoMap()); return; }

            Cleanup(); State = GameState.MoisesPuzzle;
            _moises.enabled = true; _moises.ResetPuzzle();
            _ui.ShowMoises();

            _moises.onWin = () =>
            {
                Save.virtues += 2; Save.coins += 90;
                if (Save.worldsUnlocked < 5) Save.worldsUnlocked = 5;
                SaveSystem.Save(Save);
                _ui.Modal("Incrível!", "Você lembrou a sequência do milagre!\nVirtude: Fé", "Mapa", () => GoMap());
            };
            _moises.onFail = () => _ui.Modal("Quase!", "Não foi essa ordem. Tente de novo 😊", "Tentar", () => { _moises.ResetPuzzle(); _ui.ShowMoises(); }, "Mapa", () => GoMap());
        }

        public void StartJesusMenu()
        {
            if (Save.worldsUnlocked < 5){ _ui.Modal("Bloqueado", "Complete o Mundo 4 primeiro.", "OK", () => GoMap()); return; }
            Cleanup(); State = GameState.JesusMenu;
            _ui.ShowJesus();
        }

        public void StartJesusGameplay()
        {
            Cleanup(); State = GameState.JesusGameplay;
            _ui.ShowHud("Mundo 5 — Jesus (Coleta)");

            _jesus.enabled = true;
            _jesus.ResetMode();
            _ui.RefreshJesus(_jesus.hearts, _jesus.heartsTarget);

            _jesus.onWin = () =>
            {
                Save.virtues += 2; Save.coins += 100;
                SaveSystem.Save(Save);
                _ui.Modal("Parabéns!", "Você coletou amor e bondade no caminho 💗\nVirtudes: Amor + Bondade", "Mapa", () => GoMap());
            };
        }

        public void JonasPick(int step){ _jonas.Pick(step); _ui.RefreshJonas(); }
        public void MoisesPick(int step){ _moises.Pick(step); _ui.RefreshMoises(); }

        public void SetHero(int idx){ Save.selectedHero=Mathf.Clamp(idx,0,2); SaveSystem.Save(Save); ApplyCosmetics(); _ui.RefreshFromSave(); }
        public void SelectSkin(int idx)
        {
            idx=Mathf.Clamp(idx,0,GameConstants.MaxSkins-1);
            if(!Save.ownedSkins[idx]){ _ui.Modal("Skin bloqueada","Compre a skin na loja primeiro.","OK",()=>_ui.ShowHero()); return; }
            Save.selectedSkin=idx; SaveSystem.Save(Save); ApplyCosmetics(); _ui.RefreshFromSave();
        }
        public void BuySkin(int idx,int price)
        {
            idx=Mathf.Clamp(idx,0,GameConstants.MaxSkins-1);
            if(Save.ownedSkins[idx]){ _ui.Modal("Já possui","Você já tem essa skin ✅","OK",()=>_ui.ShowShop()); return; }
            if(Save.coins<price){ _ui.Modal("Moedas insuficientes",$"Você precisa de {price} moedas.","OK",()=>_ui.ShowShop()); return; }
            Save.coins-=price; Save.ownedSkins[idx]=true; SaveSystem.Save(Save);
            _ui.Modal("Comprado!","Skin liberada ✅","OK",()=>_ui.ShowShop()); _ui.RefreshFromSave();
        }

        public void ResetProgress(){ SaveSystem.Reset(); Save=SaveSystem.Load(); ApplyCosmetics(); }

        void ApplyCosmetics(){ _custom?.Apply(Save); }

        void Cleanup()
        {
            if(_runner!=null) _runner.enabled=false;
            if(_boss!=null) _boss.enabled=false;
            if(_jonas!=null) _jonas.enabled=false;
            if(_moises!=null) _moises.enabled=false;
            if(_jesus!=null) _jesus.enabled=false;

            if(_player!=null)
            {
                var rb=_player.GetComponent<Rigidbody>();
                if(rb!=null){ rb.isKinematic=false; rb.velocity=Vector3.zero; }
                _player.position=new Vector3(0,1.1f,2);
            }

            foreach(var o in GameObject.FindGameObjectsWithTag("Obstacle")) Destroy(o);
            foreach(var c in GameObject.FindGameObjectsWithTag("Collectible")) Destroy(c);
            foreach(var b in GameObject.FindGameObjectsWithTag("Boss")) Destroy(b);
        }
    }
}
