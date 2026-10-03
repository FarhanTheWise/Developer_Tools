using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugUIRunner : MonoBehaviour
{

    public static DebugUIRunner instance;

    [Header("References")]
    public TextMeshProUGUI consoleText;
    public ScrollRect scrollRect;

    [Header("Settings")]
    public int maxEntries = 200;
    public bool autoScroll = true;

    private readonly List<string> logs = new List<string>();
    private StringBuilder builder = new StringBuilder();
    private bool toggleUI = false;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void AddLog(string message, Color color)
    {
        string hex = ColorUtility.ToHtmlStringRGB(color);
        string formatted = $"<color=#{hex}>{message}</color>";

        logs.Add(formatted);

        // Clamp logs
        if (logs.Count > maxEntries)
            logs.RemoveAt(0);

        Redraw();
    }

    private void Redraw()
    {
        builder.Clear();

        for (int i = 0; i < logs.Count; i++)
        {
            builder.AppendLine(logs[i]);
        }

        consoleText.text = builder.ToString();
        // if (autoScroll)
        //     Canvas.ForceUpdateCanvases();
    }

    private void LateUpdate()
    {
        // if (autoScroll)
        // {
        //     scrollRect.verticalNormalizedPosition = 0f;
        // }
    }

    public void Clear()
    {
        logs.Clear();
        consoleText.text = "";
    }

    public void ToggleUI()
    {
        toggleUI = !toggleUI;
        scrollRect.gameObject.SetActive(toggleUI);
    }

}