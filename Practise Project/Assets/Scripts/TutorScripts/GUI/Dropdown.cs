using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dropdown : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public RectTransform container;
    public bool isOpen;
    public Text maintext;
    public Image image { get { return GetComponent<Image>(); } }

    public List<DropDownChild> children;
    public float childHeight = 30;
    public int childFontSize = 11;
    public Color normal = Color.white;
    public Color highlighted = Color.red;
    public Color pressed = Color.grey;
   

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOpen = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOpen = false; 
    }

    // Use this for initialization
    void Start ()
    {
        container = transform.FindChild("Container").GetComponent<RectTransform>();
        isOpen = false;
	}
	
	// Update is called once per frame
	public void Update ()
    {
        Vector3 scale = container.localScale;
        scale.y = Mathf.Lerp(scale.y, isOpen ? 1:0, Time.deltaTime * 12);
        container.localScale = scale;

	}
}


[System.Serializable]
public class DropDownChild
{
    public GameObject childObj;
    public Text childText;
    public Button.ButtonClickedEvent childEvents;

    private LayoutElement element { get { return childObj.GetComponent<LayoutElement>(); } }
    private Button button { get { return childObj.GetComponent<Button>(); } }
    private Image image { get { return childObj.GetComponent<Image>(); } }

    public DropDownChild(Dropdown parent)
    {
        childObj = UIUtility.NewButton("Child", "Button", parent.container).gameObject;
        childObj.AddComponent<LayoutElement>();

        childText = childObj.GetComponentInChildren<Text>();
        childEvents = button.onClick;

    }
    public bool UpdateChild(Dropdown parent)
    {
        if (childObj == null)
            return false;

        element.minHeight = parent.childHeight;
        

        image.sprite = parent.image.sprite;
        image.type = parent.image.type;

        childText.font = parent.maintext.font;
        childText.color = parent.maintext.color;
        childText.fontSize = parent.maintext.fontSize;

        ColorBlock b = button.colors;
        b.normalColor = parent.normal;
        b.highlightedColor = parent.highlighted;
        b.pressedColor = parent.pressed;
        
        button.onClick = childEvents;


        return true;
    }
}