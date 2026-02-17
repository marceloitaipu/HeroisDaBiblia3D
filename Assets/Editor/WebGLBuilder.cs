using UnityEditor;
using UnityEngine;
using System.IO;

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

        // Configurar a pasta de saída
        string buildPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "docs");
        
        // Criar pasta se não existir
        if (!Directory.Exists(buildPath))
        {
            Directory.CreateDirectory(buildPath);
        }

        // Configurar player settings para WebGL
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
        PlayerSettings.defaultWebScreenWidth = 1080;
        PlayerSettings.defaultWebScreenHeight = 1920;
        PlayerSettings.runInBackground = true;

        // Adicionar cena ao build
        string[] scenes = { "Assets/Scenes/Main.unity" };

        // Fazer build
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        Debug.Log($"Fazendo build em: {buildPath}");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Debug.Log("Build WebGL concluído!");
    }
}
