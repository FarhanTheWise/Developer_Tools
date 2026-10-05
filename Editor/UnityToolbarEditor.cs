using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class UnityToolbarEditor
{
    private const string timeScaleDisplayPath =
        "Farhan's Tool bar/Time Scale Display";

    private const string sceneDropdownPath =
        "Farhan's Tool bar/Scene Dropdown";

    private const string LabelTooltip = "Custom time scale display";

    private static double nextCheck;
    private static float lastTimeScale = float.NaN;

    static UnityToolbarEditor()
    {
        EditorApplication.update += UpdateToolbar;

        EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChanged;
        EditorSceneManager.sceneOpened += OnSceneOpened;
        EditorSceneManager.sceneSaved += OnSceneSaved;

        EditorBuildSettings.sceneListChanged += RefreshSceneDropdown;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;

        EditorApplication.delayCall += RefreshSceneDropdown;
    }

    #region Time Scale Display

    [MainToolbarElement(
        timeScaleDisplayPath,
        defaultDockPosition = MainToolbarDockPosition.Middle,
        defaultDockIndex = 110)]
    public static MainToolbarElement DisplayTimeScale()
    {
        var content = new MainToolbarContent(
            $"Time Scale: {Time.timeScale:0.00}",
            LabelTooltip);

        return new MainToolbarLabel(content);
    }

    #endregion

    #region Open Project Settings

    [MainToolbarElement(
        "Farhan's Tool bar/Open Project Settings",
        defaultDockPosition = MainToolbarDockPosition.Left,
        defaultDockIndex = 110)]
    public static MainToolbarElement OpenProjectSettings()
    {
        var icon = EditorGUIUtility.IconContent("SettingsIcon")
            .image as Texture2D;

        var content = new MainToolbarContent(
            "Project Settings",
            icon,
            "Open Project Settings");

        return new MainToolbarButton(content, () =>
        {
            SettingsService.OpenProjectSettings();
        });
    }

    #endregion

    #region Open Player Settings

    [MainToolbarElement(
        "Farhan's Tool bar/Open Player Settings",
        defaultDockPosition = MainToolbarDockPosition.Left,
        defaultDockIndex = 120)]
    public static MainToolbarElement OpenPlayerSettings()
    {
        var icon = EditorGUIUtility.IconContent("PlayButton")
            .image as Texture2D;

        var content = new MainToolbarContent(
            "Player Settings",
            icon,
            "Open Player Settings");

        return new MainToolbarButton(content, () =>
        {
            SettingsService.OpenProjectSettings("Project/Player");
        });
    }

    #endregion

    #region Update Toolbar

    private static void UpdateToolbar()
    {
        if (EditorApplication.timeSinceStartup < nextCheck)
            return;

        nextCheck = EditorApplication.timeSinceStartup + 0.25;

        if (lastTimeScale != Time.timeScale)
        {
            lastTimeScale = Time.timeScale;
            MainToolbar.Refresh(timeScaleDisplayPath);
        }
    }

    #endregion

    #region Build Project

    [MainToolbarElement(
        "Farhan's Tool bar/Build Project",
        defaultDockPosition = MainToolbarDockPosition.Right,
        defaultDockIndex = 110)]
    public static MainToolbarElement BuildProject()
    {
        var icon = EditorGUIUtility.IconContent("BuildSettings.Editor.Small")
            .image as Texture2D;

        var content = new MainToolbarContent(
            "Build Project",
            icon,
            "Open the build window");

        return new MainToolbarButton(content, () =>
        {
            BuildPlayerWindow.ShowBuildPlayerWindow();
        });
    }

    #endregion

    #region Scene Dropdown

    [MainToolbarElement(
        sceneDropdownPath,
        defaultDockPosition = MainToolbarDockPosition.Middle,
        defaultDockIndex = 120)]
    public static MainToolbarElement SceneDropdown()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (string.IsNullOrEmpty(currentScene))
            currentScene = "Untitled Scene";

        var icon = EditorGUIUtility.IconContent("SceneAsset Icon")
            .image as Texture2D;

        var content = new MainToolbarContent(
            currentScene,
            icon,
            "Select a scene to open");

        return new MainToolbarDropdown(content, ShowSceneDropdown)
        {
            enabled = !EditorApplication.isPlayingOrWillChangePlaymode
        };
    }

    private static void ShowSceneDropdown(Rect buttonRect)
    {
        var menu = new GenericMenu();
        string currentScenePath = SceneManager.GetActiveScene().path;
        int buildIndex = 0;

        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled)
                continue;

            // Capture the full path for this menu item's callback.
            string scenePath = scene.path;
            string sceneName =
                System.IO.Path.GetFileNameWithoutExtension(scenePath);

            var menuContent = new GUIContent(
                $"{buildIndex}: {sceneName}",
                scenePath);

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) != null)
            {
                menu.AddItem(
                    menuContent,
                    scenePath == currentScenePath,
                    () => OpenBuildScene(scenePath));
            }
            else
            {
                menu.AddDisabledItem(
                    new GUIContent($"{buildIndex}: {sceneName} (Missing)"));
            }

            buildIndex++;
        }

        if (buildIndex == 0)
            menu.AddDisabledItem(new GUIContent("No enabled build scenes"));

        menu.DropDown(buttonRect);
    }

    private static void OpenBuildScene(string scenePath)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        RefreshSceneDropdown();
    }

    private static void RefreshSceneDropdown()
    {
        MainToolbar.Refresh(sceneDropdownPath);
    }

    private static void OnActiveSceneChanged(Scene previous, Scene current)
    {
        RefreshSceneDropdown();
    }

    private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        RefreshSceneDropdown();
    }

    private static void OnSceneSaved(Scene scene)
    {
        RefreshSceneDropdown();
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        RefreshSceneDropdown();
    }

    #endregion
}