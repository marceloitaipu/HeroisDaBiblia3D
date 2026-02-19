using UnityEngine;
using UnityEngine.Rendering;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Gerencia temas visuais por mundo: céu, névoa, iluminação, decorações procedurais.
    /// </summary>
    public sealed class EnvironmentBuilder : MonoBehaviour
    {
        public static EnvironmentBuilder Instance { get; private set; }

        private GameObject _decoRoot;
        private Light _fillLight;
        private Camera _mainCam;
        private Light _mainLight;
        private int _currentWorld;

        #region Temas por Mundo

        public struct WorldTheme
        {
            public Color skyColor;
            public Color ambientColor;
            public Color fogColor;
            public float fogDensity;
            public Color groundColor;
            public Color pathColor;
            public Color lightColor;
            public float lightIntensity;
        }

        public static readonly WorldTheme[] Themes = new WorldTheme[]
        {
            // [0] Menu / Padrão — céu azul claro, grama verde
            new WorldTheme
            {
                skyColor       = new Color(0.53f, 0.75f, 0.95f),
                ambientColor   = new Color(0.55f, 0.60f, 0.70f),
                fogColor       = new Color(0.65f, 0.80f, 0.92f),
                fogDensity     = 0.005f,
                groundColor    = new Color(0.40f, 0.68f, 0.35f),
                pathColor      = new Color(0.55f, 0.48f, 0.35f),
                lightColor     = new Color(1f, 0.97f, 0.92f),
                lightIntensity = 1.15f
            },
            // [1] Mundo 1 — Noé (floresta, pré‑dilúvio)
            new WorldTheme
            {
                skyColor       = new Color(0.38f, 0.58f, 0.82f),
                ambientColor   = new Color(0.45f, 0.55f, 0.65f),
                fogColor       = new Color(0.55f, 0.68f, 0.82f),
                fogDensity     = 0.011f,
                groundColor    = new Color(0.30f, 0.56f, 0.26f),
                pathColor      = new Color(0.50f, 0.42f, 0.30f),
                lightColor     = new Color(0.88f, 0.92f, 1f),
                lightIntensity = 1.05f
            },
            // [2] Mundo 2 — Davi (vale desértico)
            new WorldTheme
            {
                skyColor       = new Color(0.78f, 0.68f, 0.45f),
                ambientColor   = new Color(0.68f, 0.58f, 0.42f),
                fogColor       = new Color(0.82f, 0.75f, 0.58f),
                fogDensity     = 0.008f,
                groundColor    = new Color(0.72f, 0.58f, 0.35f),
                pathColor      = new Color(0.62f, 0.48f, 0.28f),
                lightColor     = new Color(1f, 0.92f, 0.78f),
                lightIntensity = 1.30f
            },
            // [3] Mundo 3 — Jonas (oceano)
            new WorldTheme
            {
                skyColor       = new Color(0.28f, 0.52f, 0.78f),
                ambientColor   = new Color(0.35f, 0.50f, 0.68f),
                fogColor       = new Color(0.42f, 0.62f, 0.82f),
                fogDensity     = 0.014f,
                groundColor    = new Color(0.28f, 0.46f, 0.60f),
                pathColor      = new Color(0.22f, 0.40f, 0.52f),
                lightColor     = new Color(0.85f, 0.92f, 1f),
                lightIntensity = 1.0f
            },
            // [4] Mundo 4 — Moisés (deserto / Mar Vermelho)
            new WorldTheme
            {
                skyColor       = new Color(0.88f, 0.72f, 0.48f),
                ambientColor   = new Color(0.72f, 0.60f, 0.45f),
                fogColor       = new Color(0.85f, 0.72f, 0.55f),
                fogDensity     = 0.009f,
                groundColor    = new Color(0.80f, 0.66f, 0.40f),
                pathColor      = new Color(0.70f, 0.55f, 0.32f),
                lightColor     = new Color(1f, 0.88f, 0.72f),
                lightIntensity = 1.32f
            },
            // [5] Mundo 5 — Jesus (pastoral, luz quente)
            new WorldTheme
            {
                skyColor       = new Color(0.52f, 0.76f, 0.95f),
                ambientColor   = new Color(0.60f, 0.62f, 0.55f),
                fogColor       = new Color(0.70f, 0.80f, 0.70f),
                fogDensity     = 0.006f,
                groundColor    = new Color(0.38f, 0.65f, 0.32f),
                pathColor      = new Color(0.52f, 0.46f, 0.32f),
                lightColor     = new Color(1f, 0.95f, 0.82f),
                lightIntensity = 1.22f
            }
        };

        #endregion

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        /// <summary>Configura referências da câmera e luz principal.</summary>
        public void Initialize(Camera cam, Light mainLight)
        {
            _mainCam = cam;
            _mainLight = mainLight;
        }

        /// <summary>Aplica o tema visual para o mundo indicado.</summary>
        public void ApplyTheme(int worldIndex)
        {
            _currentWorld = Mathf.Clamp(worldIndex, 0, Themes.Length - 1);
            var t = Themes[_currentWorld];

            // --- Céu ---
            if (_mainCam != null)
            {
                _mainCam.clearFlags = CameraClearFlags.SolidColor;
                _mainCam.backgroundColor = t.skyColor;
            }

            // --- Luz ambiente ---
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = t.ambientColor;

            // --- Névoa ---
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = t.fogColor;
            RenderSettings.fogDensity = t.fogDensity;

            // --- Luz principal ---
            if (_mainLight != null)
            {
                _mainLight.color = t.lightColor;
                _mainLight.intensity = t.lightIntensity;
            }

            // --- Luz de preenchimento ---
            EnsureFillLight(t);

            // --- Chão ---
            UpdateGround(t);

            // --- Estrada ---
            UpdateRoad(t);

            // Limpa decorações antigas
            CleanupAllDecorations();
        }

        public WorldTheme GetCurrentTheme()
        {
            return Themes[Mathf.Clamp(_currentWorld, 0, Themes.Length - 1)];
        }

        #region Luz de Preenchimento

        void EnsureFillLight(WorldTheme t)
        {
            if (_fillLight == null)
            {
                var go = new GameObject("Fill Light");
                _fillLight = go.AddComponent<Light>();
                _fillLight.type = LightType.Directional;
                _fillLight.shadows = LightShadows.None;
                _fillLight.transform.rotation = Quaternion.Euler(28, 155, 0);
            }
            _fillLight.intensity = 0.35f;
            _fillLight.color = Color.Lerp(t.ambientColor, Color.white, 0.4f);
        }

        #endregion

        #region Chão e Estrada

        void UpdateGround(WorldTheme t)
        {
            var ground = GameObject.Find("Ground");
            if (ground == null) return;
            var r = ground.GetComponent<Renderer>();
            if (r == null) return;
            r.material.color = t.groundColor;
            r.material.SetFloat("_Glossiness", 0.12f);
        }

        void UpdateRoad(WorldTheme t)
        {
            // Estrada central
            var road = GameObject.Find("Road");
            if (road != null)
            {
                var r = road.GetComponent<Renderer>();
                if (r != null) r.material.color = t.pathColor;
            }

            // Linhas das lanes
            foreach (var name in new[] { "LaneLine0", "LaneLine1" })
            {
                var line = GameObject.Find(name);
                if (line != null)
                {
                    var r = line.GetComponent<Renderer>();
                    if (r != null)
                        r.material.color = Color.Lerp(t.pathColor, Color.white, 0.35f);
                }
            }
        }

        #endregion

        #region Decorações

        public GameObject GetDecoRoot()
        {
            if (_decoRoot == null)
                _decoRoot = new GameObject("Decorations");
            return _decoRoot;
        }

        public void CleanupAllDecorations()
        {
            if (_decoRoot != null)
            {
                Destroy(_decoRoot);
                _decoRoot = null;
            }
        }

        /// <summary>Remove decorações atrás de uma posição Z.</summary>
        public void CleanupBehindZ(float playerZ)
        {
            if (_decoRoot == null) return;
            float threshold = playerZ - 22f;
            // Coleta antes de destruir para evitar modificar durante iteração
            var toDestroy = new System.Collections.Generic.List<Transform>();
            foreach (Transform child in _decoRoot.transform)
            {
                if (child.position.z < threshold)
                    toDestroy.Add(child);
            }
            foreach (var t in toDestroy)
                Destroy(t.gameObject);
        }

        /// <summary>Spawna decorações nos dois lados da pista em Z.</summary>
        public void SpawnSideDecorations(float z)
        {
            SpawnOneDecoration(z, -1);
            if (Random.value < 0.60f)
                SpawnOneDecoration(z, 1);
        }

        void SpawnOneDecoration(float z, int side)
        {
            float x = side * Random.Range(4.5f, 9f);
            Vector3 pos = new Vector3(x, 0, z + Random.Range(-2f, 2f));

            switch (_currentWorld)
            {
                case 0: // Menu
                case 1: // Noé — floresta
                case 5: // Jesus — pastoral
                    if (Random.value < 0.55f)
                        CreateTree(pos);
                    else
                        CreateBush(pos);
                    break;

                case 2: // Davi — deserto
                case 4: // Moisés — deserto
                    if (Random.value < 0.45f)
                        CreateRock(pos);
                    else
                        CreateCactus(pos);
                    break;

                case 3: // Jonas — oceano
                    CreateCoral(pos);
                    break;
            }
        }

        #endregion

        #region Fábricas de Decoração

        void CreateTree(Vector3 pos)
        {
            var root = GetDecoRoot();
            var tree = new GameObject("Deco_Tree");
            tree.transform.SetParent(root.transform);
            tree.transform.position = pos;

            float scale = Random.Range(0.7f, 1.3f);

            // Tronco
            var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.transform.SetParent(tree.transform);
            trunk.transform.localPosition = new Vector3(0, 1f * scale, 0);
            trunk.transform.localScale = new Vector3(0.22f * scale, 1f * scale, 0.22f * scale);
            trunk.GetComponent<Renderer>().material = CreateMat(
                new Color(0.42f + Random.Range(-0.04f, 0.04f), 0.30f, 0.18f), 0f, 0.2f);
            Destroy(trunk.GetComponent<Collider>());

            // Copa
            var crown = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            crown.transform.SetParent(tree.transform);
            crown.transform.localPosition = new Vector3(0, 2.5f * scale, 0);
            float cs = Random.Range(1.2f, 1.8f) * scale;
            crown.transform.localScale = new Vector3(cs, cs * 0.82f, cs);
            crown.GetComponent<Renderer>().material = CreateMat(
                new Color(0.18f + Random.Range(-0.04f, 0.04f),
                          0.50f + Random.Range(-0.08f, 0.08f),
                          0.12f + Random.Range(-0.03f, 0.03f)), 0f, 0.12f);
            Destroy(crown.GetComponent<Collider>());
        }

        void CreateBush(Vector3 pos)
        {
            var root = GetDecoRoot();
            var bush = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bush.name = "Deco_Bush";
            bush.transform.SetParent(root.transform);
            bush.transform.position = pos + new Vector3(0, 0.30f, 0);
            float s = Random.Range(0.45f, 0.85f);
            bush.transform.localScale = new Vector3(s * 1.2f, s * 0.65f, s);
            bush.GetComponent<Renderer>().material = CreateMat(
                new Color(0.14f, 0.40f + Random.Range(-0.06f, 0.06f), 0.10f), 0f, 0.08f);
            Destroy(bush.GetComponent<Collider>());
        }

        void CreateRock(Vector3 pos)
        {
            var root = GetDecoRoot();
            var rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.name = "Deco_Rock";
            rock.transform.SetParent(root.transform);
            rock.transform.position = pos + new Vector3(0, 0.25f, 0);
            float s = Random.Range(0.35f, 0.95f);
            rock.transform.localScale = new Vector3(s * 1.1f, s * 0.55f, s * 0.85f);
            rock.transform.rotation = Quaternion.Euler(
                Random.Range(-12, 12), Random.Range(0, 360), Random.Range(-8, 8));
            rock.GetComponent<Renderer>().material = CreateMat(
                new Color(0.52f + Random.Range(-0.06f, 0.06f),
                          0.48f + Random.Range(-0.04f, 0.04f), 0.40f), 0.08f, 0.28f);
            Destroy(rock.GetComponent<Collider>());
        }

        void CreateCactus(Vector3 pos)
        {
            var root = GetDecoRoot();
            var cactus = new GameObject("Deco_Cactus");
            cactus.transform.SetParent(root.transform);
            cactus.transform.position = pos;

            float h = Random.Range(0.8f, 1.8f);
            Color cactusGreen = new Color(0.22f, 0.50f, 0.20f);

            var body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            body.transform.SetParent(cactus.transform);
            body.transform.localPosition = new Vector3(0, h * 0.5f, 0);
            body.transform.localScale = new Vector3(0.28f, h * 0.5f, 0.28f);
            body.GetComponent<Renderer>().material = CreateMat(cactusGreen, 0f, 0.18f);
            Destroy(body.GetComponent<Collider>());

            if (Random.value < 0.60f)
            {
                var arm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                arm.transform.SetParent(cactus.transform);
                arm.transform.localPosition = new Vector3(0.32f, h * 0.38f, 0);
                arm.transform.localScale = new Vector3(0.16f, 0.30f, 0.16f);
                arm.transform.localRotation = Quaternion.Euler(0, 0, 28);
                arm.GetComponent<Renderer>().material = CreateMat(cactusGreen * 0.9f, 0f, 0.18f);
                Destroy(arm.GetComponent<Collider>());
            }
        }

        void CreateCoral(Vector3 pos)
        {
            var root = GetDecoRoot();
            var coral = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            coral.name = "Deco_Coral";
            coral.transform.SetParent(root.transform);
            coral.transform.position = pos + new Vector3(0, 0.2f, 0);
            float s = Random.Range(0.3f, 0.7f);
            coral.transform.localScale = new Vector3(s, s * 1.15f, s);

            Color[] colors =
            {
                new Color(0.82f, 0.42f, 0.32f),
                new Color(0.90f, 0.60f, 0.25f),
                new Color(0.42f, 0.72f, 0.62f),
                new Color(0.78f, 0.32f, 0.52f)
            };
            coral.GetComponent<Renderer>().material =
                CreateMat(colors[Random.Range(0, colors.Length)], 0.1f, 0.4f);
            Destroy(coral.GetComponent<Collider>());
        }

        #endregion

        #region Efeitos Visuais

        /// <summary>Efeito burst de partículas simples ao coletar/acertar.</summary>
        public static void PlayBurstEffect(Vector3 position, Color color, int count = 4)
        {
            for (int i = 0; i < count; i++)
            {
                var p = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                p.name = "FX";
                p.transform.position = position;
                p.transform.localScale = Vector3.one * 0.10f;
                p.GetComponent<Renderer>().material = CreateMat(color, 0.2f, 0.5f, color * 0.6f);
                Destroy(p.GetComponent<Collider>());

                var rb = p.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.velocity = Random.insideUnitSphere * 2.5f + Vector3.up * 1.8f;
                rb.drag = 3f;

                Destroy(p, 0.45f);
            }
        }

        /// <summary>Efeito de escala pulsante (cresce e some).</summary>
        public static void PlayPopEffect(Vector3 position, Color color, float size = 1.2f)
        {
            var ring = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ring.name = "FX_Pop";
            ring.transform.position = position;
            ring.transform.localScale = Vector3.one * 0.1f;
            ring.GetComponent<Renderer>().material = CreateMat(color, 0f, 0f, color * 0.5f);
            Destroy(ring.GetComponent<Collider>());
            ring.AddComponent<PopEffect>().maxScale = size;
        }

        #endregion

        #region Material Helper

        /// <summary>Cria material com propriedades visuais melhoradas.</summary>
        public static Material CreateMat(Color color, float metallic = 0f, float smoothness = 0.3f, Color? emission = null)
        {
            var mat = new Material(GameConstants.SafeStandardShader);
            mat.color = color;
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Glossiness", smoothness);

            if (emission.HasValue)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", emission.Value);
            }

            return mat;
        }

        #endregion
    }

    /// <summary>
    /// Efeito simples de pop/expansão. Cresce e desaparece.
    /// </summary>
    public sealed class PopEffect : MonoBehaviour
    {
        public float maxScale = 1.2f;
        private float _t;

        void Update()
        {
            _t += Time.deltaTime * 4f;
            float s = Mathf.Lerp(0.1f, maxScale, _t);
            transform.localScale = Vector3.one * s;

            var r = GetComponent<Renderer>();
            if (r != null)
            {
                var c = r.material.color;
                c.a = Mathf.Lerp(1f, 0f, _t);
                r.material.color = c;
            }

            if (_t >= 1f)
                Destroy(gameObject);
        }
    }
}
