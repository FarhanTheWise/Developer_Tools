using UnityEngine;

[System.Serializable]
public class SceneReference : ISerializationCallbackReceiver
{
#if UNITY_EDITOR
    // This field only exists inside the Editor so you can drag & drop the scene asset
    [SerializeField] private UnityEditor.SceneAsset sceneAsset;
#endif

    // This path is stored and used at runtime when the game is built
    [SerializeField] private string scenePath = string.Empty;
    [SerializeField] private string sceneName = string.Empty;

    public string ScenePath => scenePath;
    public string SceneName => sceneName;
    
    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
        {
            // Extract the secure path from the asset before Unity serializes the data
            scenePath = UnityEditor.AssetDatabase.GetAssetPath(sceneAsset);
            sceneName = sceneAsset.name;
        }
#endif
    }

    public void OnAfterDeserialize() { }
}