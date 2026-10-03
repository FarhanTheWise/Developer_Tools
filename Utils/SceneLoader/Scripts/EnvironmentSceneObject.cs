using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "ScriptableObjects/Scene Object/EnvironmentScene", fileName = "EnvironmentScene")]
public class EnvironmentSceneObject : SceneObject
{
    public override void LoadScene(Scene scene)
    {
        SceneManager.SetActiveScene(scene);

        DynamicGI.UpdateEnvironment();
        LightProbes.Tetrahedralize();
    }

    public override void UnloadScene()
    {
        GameManager.exitGameMode?.Invoke();
    }
}