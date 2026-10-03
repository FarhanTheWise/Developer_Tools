

using TMPro;
using UnityEngine;

public class FluentDebuggerSystem
{
    private bool hasPredicateBeenSet = false;
    private bool predicateCondition = false;
    private Color debugColour = Color.black;
    private TextMeshProUGUI textUI;
    private string messageIfFalse = "";

    public static FluentDebuggerSystem Begin()
    {
        return new FluentDebuggerSystem();
    }

    public void Log(string message)
    {

        if (DebugUIRunner.instance)
        {
            if (!hasPredicateBeenSet)
            {
                DebugUIRunner.instance.AddLog(message, debugColour);
                return;
            }

            if (predicateCondition)
            {
                DebugUIRunner.instance.AddLog(message, debugColour);
            }
            else
            {
                DebugUIRunner.instance.AddLog(messageIfFalse, Color.red);
            }
        }
        else
        {
            if (!hasPredicateBeenSet)
            {
                Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColour)}>{message}</color>\n");
                return;
            }

            if (predicateCondition)
            {
                Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColour)}>{message}</color>\n");
            }
            else
            {
                Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(Color.red)}>{messageIfFalse}</color>\n");
            }
        }

        hasPredicateBeenSet = false;
    }

    public FluentDebuggerSystem WithTextUI(TextMeshProUGUI textUI)
    {
        this.textUI = textUI;
        return this;
    }

    public FluentDebuggerSystem SetPredicate(bool predicate, string messageIfFalse)
    {
        predicateCondition = predicate;
        hasPredicateBeenSet = true;
        this.messageIfFalse = messageIfFalse;
        return this;
    }

    public FluentDebuggerSystem SetColour(Color color)
    {
        debugColour = color;
        return this;
    }

    
}
