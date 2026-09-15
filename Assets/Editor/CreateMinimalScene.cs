using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CreateMinimalScene
{
    [MenuItem("USV Mini/Create demo scene")]
    public static void Create()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        new GameObject("Minimal USV demo").AddComponent<MinimalUSVDemo>();
        System.IO.Directory.CreateDirectory("Assets/Scenes");
        const string path = "Assets/Scenes/MinimalUSV.unity";
        EditorSceneManager.SaveScene(scene, path);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(path, true) };
        Debug.Log("Mini USV scene created. Press Play to run the baseline.");
    }
}
