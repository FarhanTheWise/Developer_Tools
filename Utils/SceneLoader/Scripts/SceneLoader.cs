using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;


public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    
    [Header("Scene Settings")]
    public bool isTesting;
    public SceneLoad currentModeScene;
    // public int envLoad;
    public List<SceneSequence> sceneSequence;

    [Header("Splash Screen")]
    public GameObject splashScreen;

    [Header("Loading Screen")]
    public float fadeDuration = 0.3f;
    public float fadeRemainDuration = 3f;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image progressBar;
    public Image loadingBgImage;
    public CanvasGroup fadeImage;
    // public TextMeshProUGUI modeNameTxt;
    // public TextMeshProUGUI modeDescription;
    // public TextMeshProUGUI loadingText;


    public List<Scene> currentScenes;
    public List<SceneObject> currentSelectedScenes;
    private bool isLoading;
    private SceneSequence currentSceneSequence;
    // private Queue<string> loadingStrings;
    // private float nextTextProgress = 0.125f;
    // private readonly float textStep = 0.125f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        //SetEnvLoad(0);
        currentScenes = new List<Scene>();
        //splashScreen.SetActive(true);
        LoadLevel(SceneLoad.MainMenu);
    }

    public void ReloadLevel()
    {
        if(isLoading)
            return;

        if(currentSelectedScenes.Count > 0) 
        {

            foreach(var sceneObject in currentSelectedScenes)
            {
                sceneObject.UnloadScene();
            }

            currentSelectedScenes.Clear();
        }

        progressBar.fillAmount = 0f;
        StartCoroutine(LoadRoutine(currentModeScene));
    }

    public void LoadLevel(SceneLoad loadScene)
    {
        if (isLoading)
            return;

        if(currentSelectedScenes.Count > 0) 
        {

            foreach(var sceneObject in currentSelectedScenes)
            {
                sceneObject.UnloadScene();
            }

            currentSelectedScenes.Clear();
        }

        // if(sceneSequence[loadScene.GetHashCode()].envScenes.Count > 0) 
        //     SetEnvLoad(Random.Range(0, sceneSequence[loadScene.GetHashCode()].envScenes.Count));
        // else SetEnvLoad(0);
    

        currentModeScene = loadScene;
        //DataManager.instance.sessionData.currentMode = currentModeScene.GetHashCode();
        progressBar.fillAmount = 0f;
        StartCoroutine(LoadRoutine(loadScene));
    }

    //     private void UpdateLoadingText(float progress)
    // {
    //     if (loadingText == null)
    //         return;

    //     if(loadingStrings.Count <= 0) return;

    //     if (progress >= nextTextProgress)
    //     {
    //         if (loadingStrings.Count > 0)
    //         {
    //             loadingText.text = loadingStrings.Dequeue();
    //         }

    //         nextTextProgress += textStep;
    //     }

    // }

    private IEnumerator LoadRoutine(SceneLoad loadScenes)
{
    isLoading = true;
    currentSceneSequence = sceneSequence[loadScenes.GetHashCode()];
    //SetLoadingScreen(currentSceneSequence);
    //loadingStrings = new Queue<string>(currentSceneSequence.loadingTextStrings);

    yield return FadeLoadingScreen(true);

    progressBar.fillAmount = 0f;

    // -------------------------
    // UNLOAD PREVIOUS SCENES
    // -------------------------

    if (currentScenes.Count > 0)
    {
        foreach (var scene in currentScenes)
        {
            if (scene.IsValid())
            {
                yield return SceneManager.UnloadSceneAsync(scene);
            }
        }

        currentScenes.Clear();
    }

    yield return Resources.UnloadUnusedAssets();

    GC.Collect();

    // -------------------------
    // PREPARE SCENE LIST
    // -------------------------

    if (currentSceneSequence.envScene != null)
    {
        currentSelectedScenes.Add(currentSceneSequence.envScene);
    }

    foreach(var controlScene in currentSceneSequence.controlScenes)
    {
        currentSelectedScenes.Add(controlScene);
    }

    // -------------------------
    // FAKE LOADING BAR
    // -------------------------

    float timer = 0f;

    while (timer < currentSceneSequence.loadingDuration)
    {
        timer += Time.unscaledDeltaTime;

        float progress = timer / currentSceneSequence.loadingDuration;

        progressBar.fillAmount = Mathf.Clamp01(progress);

        //UpdateLoadingText(progress);

        yield return null;
    }

    progressBar.fillAmount = 1f;

    // -------------------------
    // BAR FINISHED
    // NOW LOAD / ACTIVATE SCENES
    // -------------------------

    for (int i = 0; i < currentSelectedScenes.Count; i++)
    {
        var currentScene = currentSelectedScenes[i];
        var scenePath = currentScene.sceneReference.ScenePath;

        AsyncOperation loadOperation =
            SceneManager.LoadSceneAsync(
                scenePath,
                LoadSceneMode.Additive
            );

        if (loadOperation == null)
            continue;

        // Scene will activate normally here.
        yield return loadOperation;

        Scene scene = SceneManager.GetSceneByPath(scenePath);

        currentScenes.Add(scene);

        currentScene.LoadScene(scene);
    }

    // -------------------------
    // EVERYTHING IS READY
    // -------------------------

    yield return null;

    yield return FadeLoadingScreen(false);

    isLoading = false;
}
    

    // private void SetLoadingScreen(SceneSequence currentSequence)
    // {
    //     if(currentSequence.loadingBgSprites.Count > 0)
    //     {
    //         loadingBgImage.sprite = currentSequence.loadingBgSprites[envLoad];
    //     }

    //     modeNameTxt.text = $"{currentSequence.sequenceName}";
    //     modeDescription.text = $"{currentSequence.loadingDescriptions[Random.Range(0, currentSequence.loadingDescriptions.Count)]}";
    // }


    private IEnumerator FadeLoadingScreen(bool fadeIn)
    {
        

        if (!fadeIn)
        {
            if(splashScreen.activeSelf) splashScreen.SetActive(false);
            loadingScreen.SetActive(false);

        }
        else
        {
           // loadingText.text = "";
            fadeImage.alpha = 1f;
           // nextTextProgress = textStep;
            loadingScreen.SetActive(true); 

            yield return new WaitForSeconds(fadeRemainDuration);

            yield return fadeImage.DOFade(0f, fadeDuration);
        }

        
    }
    
}

public enum SceneLoad
{
    MainMenu = 0,
    CtfMode_Scene = 1,
    ZoneControl_Scene = 2,
    ZombieMode_Scene = 3,
}

[Serializable]
public class SceneSequence
{
    [Header("Scene Settings")]
    public string sequenceName;
    public List<SceneObject> controlScenes;
    public SceneObject envScene;

    [Header("Load Settings")]
    public float loadingDuration;
    // public List<string> loadingDescriptions;
    // public List<string> loadingTextStrings;
    // public List<Sprite> loadingBgSprites;
}