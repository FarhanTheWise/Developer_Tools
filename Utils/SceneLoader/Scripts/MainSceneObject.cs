using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "ScriptableObjects/Scene Object/MainScene", fileName = "MainScene")]
public class MainSceneObject : SceneObject
{
    [Header("Control Scene Settings")]
    public BG_Music bgMusic;


    public override void LoadScene(Scene scene)
    {
        EffectsAndSettingsManager.setBgMusic?.Invoke(bgMusic);
    }
}