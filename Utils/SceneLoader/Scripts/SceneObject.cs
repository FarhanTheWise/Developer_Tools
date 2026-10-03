using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SceneObject : ScriptableObject
{
    [Header("Scene Settings")]
    public SceneReference sceneReference;
    public abstract void LoadScene(Scene scene);
    public virtual void UnloadScene(){}
}