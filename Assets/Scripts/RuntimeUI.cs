using System;
using UnityEngine;
using UnityEngine.UI;

namespace HeroisDaBiblia3D
{
    // UI simples e moderna (runtime) para mobile Portrait
    public sealed class RuntimeUI : MonoBehaviour
    {
        Canvas _c;
        GameFlowManager _flow;
        InputRouter _input;
        UIAnimator _animator;

        GameObject _home,_map,_hud,_modal,_quiz,_hero,_shop,_jonas,_moises,_jesus;
        Text _hudTitle,_coins,_virtues,_bossInfo,_result,_jesusStatus,_jonasStatus,_moisesStatus,_heroInfo,_shopInfo;
        Image _aimFill;
        Button _btnDavi,_btnJonas,_btnMoises,_btnJesus;
        Text _lockDavi,_lockJonas,_lockMoises,_lockJesus;

        GameObject _mobile;
        VirtualJoystick _joy;
        Button _jump,_slide,_action;

        BossMode _boss;
        JonasPuzzleMode _jonasMode;
        MoisesPuzzleMode _moisesMode;
        JesusCollectMode _jesusMode;

        public Canvas Canvas => _c;
        public Vector2 JoystickValue => _joy!=null ? _joy.Value : Vector2.zero;

        public void Bind(GameFlowManager flow, InputRouter input, UIAnimator animator = null)
        {
            _flow=flow; _input=input; _animator=animator;
            _boss=FindObjectOfType<BossMode>();
            _jonasMode=FindObjectOfType<JonasPuzzleMode>();
            _moisesMode=FindObjectOfType<MoisesPuzzleMode>();
            _jesusMode=FindObjectOfType<JesusCollectMode>();
            Build();
        }

        void Build()
        {
            var go=new GameObject("Canvas");
            _c=go.AddComponent<Canvas>(); _c.renderMode=RenderMode.ScreenSpaceOverlay;
            var scaler=go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution=new Vector2(1080,1920);
            go.AddComponent<GraphicRaycaster>();

            if(FindObjectOfType<UnityEngine.EventSystems.EventSystem>()==null)
            {
                var es=new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            _home=Panel("Home");
            _map=Panel("Map");
            _hud=Panel("Hud",0);
            _modal=Panel("Modal",0.55f);
            _quiz=Panel("Quiz",0.55f);
            _hero=Panel("Hero",0.45f);
            _shop=Panel("Shop",0.45f);
            _jonas=Panel("Jonas",0.45f);
            _moises=Panel("Moises",0.45f);
            _jesus=Panel("Jesus",0.45f);

            BuildHome();
            BuildMap();
            BuildHud();
            BuildModal();
            BuildQuiz();
            BuildHero();
            BuildShop();
            BuildJonas();
            BuildMoises();
            BuildJesus();
            BuildMobile();

            HideAll(); _home.SetActive(true);
        }

        GameObject Panel(string name, float dark=0.35f)
        {
            var p=new GameObject(name); p.transform.SetParent(_c.transform,false);
            var img=p.AddComponent<Image>();
            img.color=new Color(0,0,0,dark);
            var rt=p.GetComponent<RectTransform>();
            rt.anchorMin=Vector2.zero; rt.anchorMax=Vector2.one; rt.offsetMin=Vector2.zero; rt.offsetMax=Vector2.zero;
            return p;
        }

        GameObject Card(Transform parent, Vector2 size)
        {
            var c=new GameObject("Card"); c.transform.SetParent(parent,false);
            var img=c.AddComponent<Image>(); img.color=new Color(1,1,1,0.97f);
            var rt=c.GetComponent<RectTransform>(); rt.sizeDelta=size; rt.anchorMin=rt.anchorMax=new Vector2(0.5f,0.5f);
            var sh=c.AddComponent<Shadow>(); sh.effectDistance=new Vector2(0,-6); sh.effectColor=new Color(0,0,0,0.25f);
            return c;
        }

        Text Txt(Transform p, string name, string v, int size, FontStyle st, TextAnchor an)
        {
            var go=new GameObject(name); go.transform.SetParent(p,false);
            var t=go.AddComponent<Text>();
            t.text=v; t.font=Resources.GetBuiltinResource<Font>("Arial.ttf");
            t.fontSize=size; t.fontStyle=st; t.color=new Color(0.12f,0.12f,0.12f); t.alignment=an;
            return t;
        }

        Button Btn(Transform p, string name, string label, Action onClick, Vector2 size)
        {
            var go=new GameObject(name); go.transform.SetParent(p,false);
            var img=go.AddComponent<Image>(); img.color=new Color(0.20f,0.45f,0.95f,1f);
            var b=go.AddComponent<Button>();
            b.onClick.AddListener(()=>{ AudioManager.I?.Beep(); onClick?.Invoke(); });
            var t=Txt(go.transform,"Label",label,24,FontStyle.Bold,TextAnchor.MiddleCenter);
            var tr=t.GetComponent<RectTransform>(); tr.anchorMin=Vector2.zero; tr.anchorMax=Vector2.one; tr.offsetMin=Vector2.zero; tr.offsetMax=Vector2.zero;
            go.GetComponent<RectTransform>().sizeDelta=size;
            return b;
        }

        void HideAll()
        {
            _home.SetActive(false); _map.SetActive(false); _hud.SetActive(false); _modal.SetActive(false); _quiz.SetActive(false);
            _hero.SetActive(false); _shop.SetActive(false); _jonas.SetActive(false); _moises.SetActive(false); _jesus.SetActive(false);
            if(_mobile!=null) _mobile.SetActive(false);
        }

        public void ShowHome(){HideAll(); _home.SetActive(true);}
        public void ShowMap(){HideAll(); _map.SetActive(true); RefreshFromSave();}
        public void ShowHud(string title)
        {
            HideAll(); _hud.SetActive(true); _mobile.SetActive(true);
            _hudTitle.text=title;
            bool runner=_flow.State==GameState.RunnerNoe;
            bool boss=_flow.State==GameState.BossDavi;
            _jump.gameObject.SetActive(runner);
            _slide.gameObject.SetActive(runner);
            _action.gameObject.SetActive(boss);
        }
        public void ShowHero(){HideAll(); _hero.SetActive(true); RefreshFromSave();}
        public void ShowShop(){HideAll(); _shop.SetActive(true); RefreshFromSave();}
        public void ShowJonas(){HideAll(); _jonas.SetActive(true); RefreshJonas();}
        public void ShowMoises(){HideAll(); _moises.SetActive(true); RefreshMoises();}
        public void ShowJesus(){HideAll(); _jesus.SetActive(true); RefreshJesus(0,_jesusMode!=null?_jesusMode.heartsTarget:18);}

        public void RefreshFromSave()
        {
            var s=_flow.Save;
            _coins.text=$"Moedas: {s.coins}";
            _virtues.text=$"Virtudes: {s.virtues}";

            bool d=s.worldsUnlocked>=2;
            bool j=s.worldsUnlocked>=3;
            bool m=s.worldsUnlocked>=4;
            bool je=s.worldsUnlocked>=5;

            _btnDavi.interactable=d; _lockDavi.gameObject.SetActive(!d);
            _btnJonas.interactable=j; _lockJonas.gameObject.SetActive(!j);
            _btnMoises.interactable=m; _lockMoises.gameObject.SetActive(!m);
            _btnJesus.interactable=je; _lockJesus.gameObject.SetActive(!je);

            _heroInfo.text=$"Herói: {HeroName(s.selectedHero)} | Skin: {SkinName(s.selectedSkin)}";
            _shopInfo.text=$"Moedas: {s.coins}\nAzul: {(s.ownedSkins[1]?"✅":"❌")} (80)  |  Roxa: {(s.ownedSkins[2]?"✅":"❌")} (120)";
        }

        public void SetBossHud(int stones,int hits,int needed)
        {
            _bossInfo.text=$"Pedras: {stones}   Acertos: {hits}/{needed}";
            if(_boss!=null) _aimFill.fillAmount=_boss.IsAiming?Mathf.Clamp01(_boss.AimValue):0f;
        }

        public void RefreshJesus(int hearts,int target){ _jesusStatus.text=$"Corações: {hearts}/{target}"; }
        public void RefreshJonas(){ if(_jonasMode!=null) _jonasStatus.text=_jonasMode.StatusText(); }
        public void RefreshMoises(){ if(_moisesMode!=null) _moisesStatus.text=_moisesMode.StatusText(); }

        public void Modal(string title,string body,string primary,Action onPrimary,string secondary=null,Action onSecondary=null)
        {
            _modal.SetActive(true);
            _modal.transform.Find("Card/Title").GetComponent<Text>().text=title;
            _modal.transform.Find("Card/Body").GetComponent<Text>().text=body;

            var p=_modal.transform.Find("Card/Primary").GetComponent<Button>();
            p.GetComponentInChildren<Text>().text=primary;
            p.onClick.RemoveAllListeners();
            p.onClick.AddListener(()=>{ AudioManager.I?.Beep(); _modal.SetActive(false); onPrimary?.Invoke(); });

            var sObj=_modal.transform.Find("Card/Secondary").gameObject;
            if(string.IsNullOrEmpty(secondary)) sObj.SetActive(false);
            else{
                sObj.SetActive(true);
                var s=sObj.GetComponent<Button>();
                s.GetComponentInChildren<Text>().text=secondary;
                s.onClick.RemoveAllListeners();
                s.onClick.AddListener(()=>{ AudioManager.I?.Beep(); _modal.SetActive(false); onSecondary?.Invoke(); });
            }
        }

        public void Quiz(string title,string q,string[] opts,int correct,string virtue,Action onContinue)
        {
            _quiz.SetActive(true);
            _quiz.transform.Find("Card/Title").GetComponent<Text>().text=title;
            _quiz.transform.Find("Card/Question").GetComponent<Text>().text=q;
            _result.text="";
            var root=_quiz.transform.Find("Card/Options");
            for(int i=0;i<3;i++){
                var b=root.GetChild(i).GetComponent<Button>();
                b.interactable=i<opts.Length;
                b.GetComponentInChildren<Text>().text=i<opts.Length?opts[i]:"";
                int idx=i;
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(()=>{ AudioManager.I?.Beep(); _result.text=(idx==correct)?$"✅ Certo! Virtude: {virtue}":"❌ Quase! Vamos aprender 😊"; });
            }
            var cont=_quiz.transform.Find("Card/Continue").GetComponent<Button>();
            cont.onClick.RemoveAllListeners();
            cont.onClick.AddListener(()=>{ AudioManager.I?.Beep(); _quiz.SetActive(false); onContinue?.Invoke(); });
        }

        void BuildHome()
        {
            var card=Card(_home.transform,new Vector2(860,1280));
            var title=Txt(card.transform,"Title","Heróis da Bíblia 3D",48,FontStyle.Bold,TextAnchor.UpperCenter);
            title.GetComponent<RectTransform>().anchorMin=new Vector2(0.08f,0.80f);
            title.GetComponent<RectTransform>().anchorMax=new Vector2(0.92f,0.95f);

            var play=Btn(card.transform,"Play","JOGAR",()=>_flow.GoMap(),new Vector2(520,110));
            play.GetComponent<RectTransform>().anchorMin=play.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.55f);

            var hero=Btn(card.transform,"Hero","HERÓI / SKIN",()=>ShowHero(),new Vector2(520,110));
            hero.GetComponent<RectTransform>().anchorMin=hero.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.43f);

            var shop=Btn(card.transform,"Shop","LOJA",()=>ShowShop(),new Vector2(520,110));
            shop.GetComponent<RectTransform>().anchorMin=shop.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.31f);

            var reset=Btn(card.transform,"Reset","ZERAR PROGRESSO",()=>Modal("Atenção","Zerar moedas e mundos?","Sim",()=>{_flow.ResetProgress(); _flow.GoHome();},"Não",()=>ShowHome()),new Vector2(520,95));
            reset.GetComponent<RectTransform>().anchorMin=reset.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.16f);

            _home.SetActive(false);
        }

        void BuildMap()
        {
            var card=Card(_map.transform,new Vector2(940,1550));
            var title=Txt(card.transform,"Title","Mapa de Mundos",42,FontStyle.Bold,TextAnchor.UpperCenter);
            title.GetComponent<RectTransform>().anchorMin=new Vector2(0.1f,0.90f);
            title.GetComponent<RectTransform>().anchorMax=new Vector2(0.9f,0.98f);

            var b1=Btn(card.transform,"Noe","1 — Noé (Runner)",()=>_flow.StartRunnerNoe(),new Vector2(820,110));
            b1.GetComponent<RectTransform>().anchorMin=b1.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.78f);

            _btnDavi=Btn(card.transform,"Davi","2 — Davi x Golias (Boss)",()=>_flow.StartBossDavi(),new Vector2(820,110));
            _btnDavi.GetComponent<RectTransform>().anchorMin=_btnDavi.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.66f);
            _lockDavi=Txt(card.transform,"LockDavi","🔒 Complete o Mundo 1",18,FontStyle.Bold,TextAnchor.MiddleCenter);
            _lockDavi.GetComponent<RectTransform>().anchorMin=_lockDavi.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.61f);

            _btnJonas=Btn(card.transform,"Jonas","3 — Jonas (Puzzle)",()=>_flow.StartJonas(),new Vector2(820,110));
            _btnJonas.GetComponent<RectTransform>().anchorMin=_btnJonas.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.52f);
            _lockJonas=Txt(card.transform,"LockJonas","🔒 Complete o Mundo 2",18,FontStyle.Bold,TextAnchor.MiddleCenter);
            _lockJonas.GetComponent<RectTransform>().anchorMin=_lockJonas.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.47f);

            _btnMoises=Btn(card.transform,"Moises","4 — Moisés (Puzzle)",()=>_flow.StartMoises(),new Vector2(820,110));
            _btnMoises.GetComponent<RectTransform>().anchorMin=_btnMoises.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.38f);
            _lockMoises=Txt(card.transform,"LockMoises","🔒 Complete o Mundo 3",18,FontStyle.Bold,TextAnchor.MiddleCenter);
            _lockMoises.GetComponent<RectTransform>().anchorMin=_lockMoises.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.33f);

            _btnJesus=Btn(card.transform,"Jesus","5 — Jesus (Coleta)",()=>_flow.StartJesusMenu(),new Vector2(820,110));
            _btnJesus.GetComponent<RectTransform>().anchorMin=_btnJesus.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.24f);
            _lockJesus=Txt(card.transform,"LockJesus","🔒 Complete o Mundo 4",18,FontStyle.Bold,TextAnchor.MiddleCenter);
            _lockJesus.GetComponent<RectTransform>().anchorMin=_lockJesus.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.19f);

            var back=Btn(card.transform,"Back","Voltar",()=>_flow.GoHome(),new Vector2(520,105));
            back.GetComponent<RectTransform>().anchorMin=back.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.08f);

            _map.SetActive(false);
        }

        void BuildHud()
        {
            var top=new GameObject("Top"); top.transform.SetParent(_hud.transform,false);
            var img=top.AddComponent<Image>(); img.color=new Color(1,1,1,0.92f);
            var rt=top.GetComponent<RectTransform>(); rt.anchorMin=new Vector2(0,0.92f); rt.anchorMax=new Vector2(1,1); rt.offsetMin=rt.offsetMax=Vector2.zero;

            _hudTitle=Txt(top.transform,"HudTitle","Mundo",24,FontStyle.Bold,TextAnchor.MiddleLeft);
            _hudTitle.GetComponent<RectTransform>().anchorMin=new Vector2(0.04f,0); _hudTitle.GetComponent<RectTransform>().anchorMax=new Vector2(0.58f,1);

            _coins=Txt(top.transform,"Coins","Moedas: 0",20,FontStyle.Normal,TextAnchor.MiddleRight);
            _coins.GetComponent<RectTransform>().anchorMin=new Vector2(0.50f,0); _coins.GetComponent<RectTransform>().anchorMax=new Vector2(0.72f,1);

            _virtues=Txt(top.transform,"Virtues","Virtudes: 0",20,FontStyle.Normal,TextAnchor.MiddleRight);
            _virtues.GetComponent<RectTransform>().anchorMin=new Vector2(0.72f,0); _virtues.GetComponent<RectTransform>().anchorMax=new Vector2(0.90f,1);

            // Botão Pause no HUD
            var pauseBtn = Btn(top.transform, "Pause", "⏸", () => {
                var pm = FindObjectOfType<PauseManager>();
                if (pm != null) pm.TogglePause();
            }, new Vector2(70, 70));
            pauseBtn.GetComponent<RectTransform>().anchorMin = new Vector2(0.93f, 0.1f);
            pauseBtn.GetComponent<RectTransform>().anchorMax = new Vector2(0.99f, 0.9f);
            pauseBtn.GetComponent<Image>().color = new Color(0.85f, 0.30f, 0.30f, 1f);

            var boss=new GameObject("BossHud"); boss.transform.SetParent(_hud.transform,false);
            var br=boss.AddComponent<RectTransform>(); br.anchorMin=new Vector2(0.08f,0.82f); br.anchorMax=new Vector2(0.92f,0.90f);
            _bossInfo=Txt(boss.transform,"BossInfo","Pedras: 0   Acertos: 0/3",20,FontStyle.Bold,TextAnchor.MiddleCenter);
            _bossInfo.GetComponent<RectTransform>().anchorMin=new Vector2(0,0.55f); _bossInfo.GetComponent<RectTransform>().anchorMax=new Vector2(1,1);

            var barBg=new GameObject("AimBg"); barBg.transform.SetParent(boss.transform,false);
            var bg=barBg.AddComponent<Image>(); bg.color=new Color(0,0,0,0.25f);
            var bgr=barBg.GetComponent<RectTransform>(); bgr.anchorMin=new Vector2(0.05f,0.05f); bgr.anchorMax=new Vector2(0.95f,0.45f);

            var barFill=new GameObject("AimFill"); barFill.transform.SetParent(barBg.transform,false);
            _aimFill=barFill.AddComponent<Image>(); _aimFill.color=new Color(0.20f,0.75f,0.30f,1f);
            _aimFill.type=Image.Type.Filled; _aimFill.fillMethod=Image.FillMethod.Horizontal; _aimFill.fillAmount=0;
            var fr=barFill.GetComponent<RectTransform>(); fr.anchorMin=Vector2.zero; fr.anchorMax=Vector2.one;

            _hud.SetActive(false);
        }

        void BuildModal()
        {
            var card=Card(_modal.transform,new Vector2(880,700)); card.name="Card";
            var title=Txt(card.transform,"Title","Título",34,FontStyle.Bold,TextAnchor.UpperCenter);
            title.GetComponent<RectTransform>().anchorMin=new Vector2(0.1f,0.72f);
            title.GetComponent<RectTransform>().anchorMax=new Vector2(0.9f,0.92f);
            var body=Txt(card.transform,"Body","Mensagem",22,FontStyle.Normal,TextAnchor.UpperCenter);
            body.GetComponent<RectTransform>().anchorMin=new Vector2(0.1f,0.40f);
            body.GetComponent<RectTransform>().anchorMax=new Vector2(0.9f,0.72f);

            var p=Btn(card.transform,"Primary","OK",null,new Vector2(520,110));
            p.GetComponent<RectTransform>().anchorMin=p.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.18f);
            var s=Btn(card.transform,"Secondary","Voltar",null,new Vector2(520,90));
            s.GetComponent<RectTransform>().anchorMin=s.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.05f);

            _modal.SetActive(false);
        }

        void BuildQuiz()
        {
            var card=Card(_quiz.transform,new Vector2(900,1200)); card.name="Card";
            var title=Txt(card.transform,"Title","Mini-Desafio",34,FontStyle.Bold,TextAnchor.UpperCenter);
            title.GetComponent<RectTransform>().anchorMin=new Vector2(0.1f,0.86f);
            title.GetComponent<RectTransform>().anchorMax=new Vector2(0.9f,0.96f);
            var q=Txt(card.transform,"Question","Pergunta",24,FontStyle.Normal,TextAnchor.UpperCenter);
            q.GetComponent<RectTransform>().anchorMin=new Vector2(0.1f,0.72f);
            q.GetComponent<RectTransform>().anchorMax=new Vector2(0.9f,0.86f);

            var options=new GameObject("Options"); options.transform.SetParent(card.transform,false);
            var ort=options.AddComponent<RectTransform>(); ort.anchorMin=ort.anchorMax=new Vector2(0.5f,0.50f); ort.sizeDelta=new Vector2(760,420);
            for(int i=0;i<3;i++){
                var b=Btn(options.transform,$"Opt{i}",$"Opção {i+1}",null,new Vector2(760,110));
                var rt=b.GetComponent<RectTransform>(); rt.anchorMin=rt.anchorMax=new Vector2(0.5f,1f-(i*0.33f)); rt.anchoredPosition=new Vector2(0,-70);
            }
            _result=Txt(card.transform,"Result","",22,FontStyle.Bold,TextAnchor.UpperCenter);
            _result.GetComponent<RectTransform>().anchorMin=new Vector2(0.1f,0.28f);
            _result.GetComponent<RectTransform>().anchorMax=new Vector2(0.9f,0.40f);

            var cont=Btn(card.transform,"Continue","CONTINUAR",null,new Vector2(560,110));
            cont.GetComponent<RectTransform>().anchorMin=cont.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.10f);

            _quiz.SetActive(false);
        }

        void BuildHero()
        {
            var card=Card(_hero.transform,new Vector2(900,1320));
            var title=Txt(card.transform,"Title","Herói / Skin",40,FontStyle.Bold,TextAnchor.UpperCenter);
            title.GetComponent<RectTransform>().anchorMin=new Vector2(0.1f,0.88f);
            title.GetComponent<RectTransform>().anchorMax=new Vector2(0.9f,0.97f);
            _heroInfo=Txt(card.transform,"Info","Herói: —",22,FontStyle.Bold,TextAnchor.MiddleCenter);
            _heroInfo.GetComponent<RectTransform>().anchorMin=_heroInfo.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.74f);

            var h0=Btn(card.transform,"Theo","Theo",()=>_flow.SetHero(0),new Vector2(600,100));
            h0.GetComponent<RectTransform>().anchorMin=h0.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.60f);
            var h1=Btn(card.transform,"Lia","Lia",()=>_flow.SetHero(1),new Vector2(600,100));
            h1.GetComponent<RectTransform>().anchorMin=h1.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.50f);
            var h2=Btn(card.transform,"Nina","Nina",()=>_flow.SetHero(2),new Vector2(600,100));
            h2.GetComponent<RectTransform>().anchorMin=h2.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.40f);

            var s0=Btn(card.transform,"Skin0","Skin Básica",()=>_flow.SelectSkin(0),new Vector2(600,95));
            s0.GetComponent<RectTransform>().anchorMin=s0.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.27f);
            var s1=Btn(card.transform,"Skin1","Skin Azul (se tiver)",()=>_flow.SelectSkin(1),new Vector2(600,95));
            s1.GetComponent<RectTransform>().anchorMin=s1.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.19f);
            var s2=Btn(card.transform,"Skin2","Skin Roxa (se tiver)",()=>_flow.SelectSkin(2),new Vector2(600,95));
            s2.GetComponent<RectTransform>().anchorMin=s2.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.11f);

            var back=Btn(card.transform,"Back","Voltar",()=>_flow.GoHome(),new Vector2(520,110));
            back.GetComponent<RectTransform>().anchorMin=back.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.04f);

            _hero.SetActive(false);
        }

        void BuildShop()
        {
            var card=Card(_shop.transform,new Vector2(900,1320));
            var title=Txt(card.transform,"Title","Loja de Skins",40,FontStyle.Bold,TextAnchor.UpperCenter);
            title.GetComponent<RectTransform>().anchorMin=new Vector2(0.1f,0.88f);
            title.GetComponent<RectTransform>().anchorMax=new Vector2(0.9f,0.97f);
            _shopInfo=Txt(card.transform,"Info","Moedas: 0",22,FontStyle.Bold,TextAnchor.UpperCenter);
            _shopInfo.GetComponent<RectTransform>().anchorMin=new Vector2(0.1f,0.72f);
            _shopInfo.GetComponent<RectTransform>().anchorMax=new Vector2(0.9f,0.86f);

            var buy1=Btn(card.transform,"BuyBlue","Comprar Skin Azul (80)",()=>_flow.BuySkin(1,80),new Vector2(720,110));
            buy1.GetComponent<RectTransform>().anchorMin=buy1.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.52f);
            var buy2=Btn(card.transform,"BuyPurple","Comprar Skin Roxa (120)",()=>_flow.BuySkin(2,120),new Vector2(720,110));
            buy2.GetComponent<RectTransform>().anchorMin=buy2.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.40f);

            var back=Btn(card.transform,"Back","Voltar",()=>_flow.GoHome(),new Vector2(520,110));
            back.GetComponent<RectTransform>().anchorMin=back.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.12f);

            _shop.SetActive(false);
        }

        void BuildJonas()
        {
            var card=Card(_jonas.transform,new Vector2(920,1400));
            var title=Txt(card.transform,"Title","Mundo 3 — Jonas (Puzzle)",36,FontStyle.Bold,TextAnchor.UpperCenter);
            title.GetComponent<RectTransform>().anchorMin=new Vector2(0.08f,0.88f);
            title.GetComponent<RectTransform>().anchorMax=new Vector2(0.92f,0.97f);

            _jonasStatus=Txt(card.transform,"Status","— → — → —",22,FontStyle.Bold,TextAnchor.MiddleCenter);
            _jonasStatus.GetComponent<RectTransform>().anchorMin=_jonasStatus.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.68f);

            var b0=Btn(card.transform,"S0","1) Jonas foge",()=>{_flow.JonasPick(0); RefreshJonas();},new Vector2(720,110));
            b0.GetComponent<RectTransform>().anchorMin=b0.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.52f);
            var b1=Btn(card.transform,"S1","2) Peixe grande",()=>{_flow.JonasPick(1); RefreshJonas();},new Vector2(720,110));
            b1.GetComponent<RectTransform>().anchorMin=b1.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.40f);
            var b2=Btn(card.transform,"S2","3) Jonas obedece",()=>{_flow.JonasPick(2); RefreshJonas();},new Vector2(720,110));
            b2.GetComponent<RectTransform>().anchorMin=b2.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.28f);

            var back=Btn(card.transform,"Back","Voltar",()=>_flow.GoMap(),new Vector2(520,110));
            back.GetComponent<RectTransform>().anchorMin=back.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.10f);

            _jonas.SetActive(false);
        }

        void BuildMoises()
        {
            var card=Card(_moises.transform,new Vector2(960,1500));
            var title=Txt(card.transform,"Title","Mundo 4 — Moisés (Puzzle)",36,FontStyle.Bold,TextAnchor.UpperCenter);
            title.GetComponent<RectTransform>().anchorMin=new Vector2(0.08f,0.88f);
            title.GetComponent<RectTransform>().anchorMax=new Vector2(0.92f,0.97f);

            _moisesStatus=Txt(card.transform,"Status","— → — → — → —",20,FontStyle.Bold,TextAnchor.MiddleCenter);
            _moisesStatus.GetComponent<RectTransform>().anchorMin=_moisesStatus.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.68f);

            var b0=Btn(card.transform,"S0","1) Cajado ao alto",()=>{_flow.MoisesPick(0); RefreshMoises();},new Vector2(760,110));
            b0.GetComponent<RectTransform>().anchorMin=b0.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.56f);
            var b1=Btn(card.transform,"S1","2) Mar se abre",()=>{_flow.MoisesPick(1); RefreshMoises();},new Vector2(760,110));
            b1.GetComponent<RectTransform>().anchorMin=b1.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.44f);
            var b2=Btn(card.transform,"S2","3) Povo atravessa",()=>{_flow.MoisesPick(2); RefreshMoises();},new Vector2(760,110));
            b2.GetComponent<RectTransform>().anchorMin=b2.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.32f);
            var b3=Btn(card.transform,"S3","4) Mar fecha",()=>{_flow.MoisesPick(3); RefreshMoises();},new Vector2(760,110));
            b3.GetComponent<RectTransform>().anchorMin=b3.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.20f);

            var back=Btn(card.transform,"Back","Voltar",()=>_flow.GoMap(),new Vector2(520,110));
            back.GetComponent<RectTransform>().anchorMin=back.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.08f);

            _moises.SetActive(false);
        }

        void BuildJesus()
        {
            var card=Card(_jesus.transform,new Vector2(920,1200));
            var title=Txt(card.transform,"Title","Mundo 5 — Jesus (Coleta)",36,FontStyle.Bold,TextAnchor.UpperCenter);
            title.GetComponent<RectTransform>().anchorMin=new Vector2(0.08f,0.88f);
            title.GetComponent<RectTransform>().anchorMax=new Vector2(0.92f,0.97f);

            _jesusStatus=Txt(card.transform,"Status","Corações: 0/0",26,FontStyle.Bold,TextAnchor.MiddleCenter);
            _jesusStatus.GetComponent<RectTransform>().anchorMin=_jesusStatus.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.58f);

            var start=Btn(card.transform,"Start","COMEÇAR",()=>_flow.StartJesusGameplay(),new Vector2(560,120));
            start.GetComponent<RectTransform>().anchorMin=start.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.38f);

            var back=Btn(card.transform,"Back","Voltar",()=>_flow.GoMap(),new Vector2(520,110));
            back.GetComponent<RectTransform>().anchorMin=back.GetComponent<RectTransform>().anchorMax=new Vector2(0.5f,0.18f);

            _jesus.SetActive(false);
        }

        void BuildMobile()
        {
            _mobile=new GameObject("Mobile"); _mobile.transform.SetParent(_c.transform,false);
            var rt=_mobile.AddComponent<RectTransform>(); rt.anchorMin=Vector2.zero; rt.anchorMax=Vector2.one;

            var joyBg=new GameObject("JoyBG"); joyBg.transform.SetParent(_mobile.transform,false);
            var bg=joyBg.AddComponent<Image>(); bg.color=new Color(1,1,1,0.18f);
            var bgrt=joyBg.GetComponent<RectTransform>(); bgrt.anchorMin=new Vector2(0.05f,0.05f); bgrt.anchorMax=new Vector2(0.25f,0.18f);

            var joyHandle=new GameObject("JoyHandle"); joyHandle.transform.SetParent(joyBg.transform,false);
            var hi=joyHandle.AddComponent<Image>(); hi.color=new Color(0.20f,0.45f,0.95f,0.75f);
            var hrt=joyHandle.GetComponent<RectTransform>(); hrt.anchorMin=hrt.anchorMax=new Vector2(0.5f,0.5f); hrt.sizeDelta=new Vector2(90,90);

            _joy=joyBg.AddComponent<VirtualJoystick>(); _joy.bg=bgrt; _joy.handle=hrt;

            _jump=Round("Jump","Pular",new Vector2(0.86f,0.12f),()=>_input.PressJump());
            _slide=Round("Slide","Deslizar",new Vector2(0.70f,0.07f),()=>_input.PressSlide());
            _action=Round("Action","Ação",new Vector2(0.84f,0.08f),()=>_input.PressAction());

            _mobile.SetActive(false);
        }

        Button Round(string name,string label,Vector2 anchor,Action onClick)
        {
            var go=new GameObject(name); go.transform.SetParent(_mobile.transform,false);
            var img=go.AddComponent<Image>(); img.color=new Color(0.20f,0.45f,0.95f,0.85f);
            var b=go.AddComponent<Button>();
            b.onClick.AddListener(()=>{AudioManager.I?.Beep(); onClick?.Invoke();});
            var rt=go.GetComponent<RectTransform>(); rt.anchorMin=rt.anchorMax=anchor; rt.sizeDelta=new Vector2(220,110);
            var t=Txt(go.transform,"Label",label,22,FontStyle.Bold,TextAnchor.MiddleCenter);
            t.GetComponent<RectTransform>().anchorMin=Vector2.zero; t.GetComponent<RectTransform>().anchorMax=Vector2.one;
            return b;
        }

        static string HeroName(int i)=> i==0?"Theo":(i==1?"Lia":"Nina");
        static string SkinName(int i)=> i==0?"Básica":(i==1?"Azul":"Roxa");
    }
}
