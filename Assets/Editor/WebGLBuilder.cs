using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class WebGLBuilder
{
    [MenuItem("Build/Build WebGL")]
    public static void BuildWebGL()
    {
        Build();
    }

    public static void Build()
    {
        Debug.Log("Iniciando build WebGL...");

        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string buildPath = Path.Combine(projectRoot, "docs");

        if (!Directory.Exists(buildPath))
        {
            Directory.CreateDirectory(buildPath);
        }

        // Garantir que a cena esteja registrada no EditorBuildSettings
        string scenePath = "Assets/Scenes/Main.unity";
        var currentScenes = EditorBuildSettings.scenes;
        if (!currentScenes.Any(s => s.path == scenePath))
        {
            var newScenes = currentScenes.ToList();
            newScenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = newScenes.ToArray();
            Debug.Log($"Cena '{scenePath}' adicionada ao EditorBuildSettings.");
        }

        // Configurar player settings para WebGL
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
        PlayerSettings.defaultWebScreenWidth = 1080;
        PlayerSettings.defaultWebScreenHeight = 1920;
        PlayerSettings.runInBackground = true;

        // Stripping settings
        PlayerSettings.SetManagedStrippingLevel(
            UnityEditor.Build.NamedBuildTarget.WebGL,
            ManagedStrippingLevel.Minimal);

        string[] scenes = { scenePath };

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.CleanBuildCache
        };

        Debug.Log($"Fazendo build em: {buildPath}");
        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        var summary = report.summary;

        if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log($"Build WebGL concluido com SUCESSO! Tamanho: {summary.totalSize} bytes");
        }
        else
        {
            Debug.LogError($"Build WebGL FALHOU! Resultado: {summary.result}");
            Debug.LogError($"Erros: {summary.totalErrors}");
            EditorApplication.Exit(1);
        }
    }
}
