using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;


[CustomEditor(typeof(Dropdown))]
public class DropDownEditor : Editor
{
    public Dropdown currDropdown;

    void OnEnable()
    {
        currDropdown = target as Dropdown;
    }

    public override void OnInspectorGUI()
    {
        VerifyValid();
        if(GUILayout.Button("Add Child"))
        {
            AddChild();
        }
        GUILayout.Space(5);
        currDropdown.isOpen = EditorGUILayout.Toggle("Is Open?", currDropdown.isOpen);
        currDropdown.maintext.text = EditorGUILayout.TextField("Main Text", currDropdown.maintext.text);
        currDropdown.maintext.font = (Font)EditorGUILayout.ObjectField("", currDropdown.maintext.font, typeof(Font), false);

        GUILayout.Space(5);
        currDropdown.maintext.fontSize = EditorGUILayout.IntField("Fond Size", currDropdown.maintext.fontSize);
        currDropdown.maintext.color = EditorGUILayout.ColorField("Font Color", currDropdown.maintext.color);

        GUILayout.Space(5);
        currDropdown.image.sprite = (Sprite)EditorGUILayout.ObjectField("Button Sprite",currDropdown.image.sprite,typeof(Sprite),false,GUILayout.Height(16));
        currDropdown.image.type = (Image.Type)EditorGUILayout.EnumPopup("Type", currDropdown.image.type);
        currDropdown.image.color = EditorGUILayout.ColorField("Main Color", currDropdown.image.color);

        UpdateChildren();
        currDropdown.Update();
        EditorUtility.SetDirty(currDropdown);
        Repaint();
    }


    void AddChild()
    {
        if(currDropdown.children==null)
        {
            currDropdown.children = new System.Collections.Generic.List<DropDownChild>();
        }
       
        currDropdown.children.Add(new DropDownChild(currDropdown));
        
    }

    void UpdateChildren()
    {
        if (currDropdown.children == null)
            return;
        for (int i = 0; i < currDropdown.children.Count; i++)
        {
            if (currDropdown.children[i].UpdateChild(currDropdown) == false)
            {
                currDropdown.children.RemoveAt(i);
            }
            
            
        }
        
    }
    void VerifyValid()
    {
        if (currDropdown.image == null)
        {
            currDropdown.gameObject.AddComponent<Image>();
        }
        if (currDropdown.container == null)
        {
            if (currDropdown.transform.FindChild("Container") == null)
            {
                //create
                currDropdown.container = UIUtility.NewUIElement("Container", currDropdown.transform);
                currDropdown.container.gameObject.AddComponent<VerticalLayoutGroup>();
                UIUtility.ScaleRect(currDropdown.container, 0, 0);
                currDropdown.container.anchorMin = new Vector2(0, 0);
                currDropdown.container.anchorMax = new Vector2(1,0);
            }
            else
            {
                currDropdown.container = currDropdown.transform.FindChild("Container").GetComponent<RectTransform>();
            }
        }
        if (currDropdown.maintext = null)
        {
            if (currDropdown.transform.FindChild("Text") == null)
            {
                currDropdown.maintext = UIUtility.NewText("Dropdown", currDropdown.transform);
            }
            else
                currDropdown.maintext = currDropdown.transform.FindChild("Text").GetComponent<Text>();
        }
    }

}
