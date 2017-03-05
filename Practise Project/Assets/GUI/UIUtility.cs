using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIUtility : MonoBehaviour
{
    [MenuItem("GameObject/UI/Dropdown")]
    public static void NewDropdown()
    {
        NewButton("Dropdown", "Dropdown", GetCanvas().transform).gameObject.AddComponent<Dropdown>();
    }
    public static Canvas GetCanvas()
    {
        Canvas c = FindObjectOfType<Canvas>();
        if (c == null)
        {
            c = new GameObject().AddComponent<Canvas>();
            c.name = "Canvas";
            c.gameObject.layer = 5;
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            c.gameObject.AddComponent<CanvasScaler>();
            c.gameObject.AddComponent<GraphicRaycaster>();

            if (FindObjectOfType<EventSystem>() == null)
            {
                EventSystem e = new GameObject().AddComponent<EventSystem>();
                e.name = "Event System";
                e.gameObject.AddComponent<StandaloneInputModule>();
                e.gameObject.AddComponent<TouchInputModule>();
            }

        }
        return c;
    }
    public static RectTransform NewUIElement(string name, Transform parent)
    {
        RectTransform temp = new GameObject().AddComponent<RectTransform>();
        temp.name = name;
        temp.gameObject.layer = 5;
        temp.SetParent(parent);
        temp.localScale = new Vector3(1, 1, 1);
        temp.localPosition = new Vector3(0, 0, 0);

        return temp;
    }
    public static Button NewButton(string name, string text, Transform parent)
    {
        RectTransform btnRect = NewUIElement(name, parent);
        btnRect.gameObject.AddComponent<Image>();
        btnRect.gameObject.AddComponent<Button>();
        ScaleRect(btnRect, 100, 30);
        NewText(text, btnRect); 
       
        return btnRect.GetComponent<Button>();
    }
    public static Text NewText(string text, Transform parent)
    {
        RectTransform textRect = NewUIElement("Text", parent);
        Text t = textRect.gameObject.AddComponent<Text>();
        t.text = text;
        t.color = Color.black;
        t.alignment = TextAnchor.MiddleCenter;
        ScaleRect(textRect, 0, 0);
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1,1);

        return t;
    }
    public static void ScaleRect(RectTransform r, float w, float h)
    {

    }
}
