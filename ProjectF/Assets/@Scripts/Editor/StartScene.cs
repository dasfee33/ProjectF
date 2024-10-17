using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class EditorStartInit
{
  static EditorStartInit()
  {
    string startScene = EditorBuildSettings.scenes[0].path;
    var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(startScene);
    EditorSceneManager.playModeStartScene = scene;
  }
}
