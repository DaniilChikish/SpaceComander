/// <summary>
/// Backgroud UI.
/// Create by Sky Games
/// </summary>
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackgroudUI : MonoBehaviour 
{
	
	public GUISkin mainUI;
	public int numDepth = 0;
	
	public string nameWindow;
	public Texture2D pictureSelectObject;
    public Sprite pictureSelectObject1;
    public int money;
	public int score;
	
	public Texture2D pictureDefault;
    public Sprite pictureDefault1;
    public RenderTexture map;
	public Material mat;
	
	private GameMenu _GM;
	private GlobalDB _GDB;

    public Image right;
    public Text moneyText;
    public Text scoreText;

	void Start () 
	{
		_GM = gameObject.GetComponent<GameMenu>();
		_GDB = gameObject.GetComponent<GlobalDB>();
	}
	
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			_GM.pause = true;
		}
		money = _GDB.money;

        if (pictureSelectObject1 != null)
        {
            if (right.sprite == pictureDefault1)
                right.sprite = pictureSelectObject1;
        }
        else
        {
            if (right.sprite != pictureDefault1)
                right.sprite = pictureDefault1;
        }

        moneyText.text = money.ToString();
        scoreText.text = score.ToString();
	}
	
	/*void OnGUI () 
	{
		GUI.depth = numDepth;
		GUI.skin = mainUI;
		#region left block
        GUI.Box(new Rect(0, Screen.height - 256, 256, 256), "", GUI.skin.GetStyle("Line"));
        if (Event.current.type.Equals(EventType.Repaint)) //Event.current.type == EventType.Repaint Event.current.type.Equals(EventType.Repaint)
			Graphics.DrawTexture(new Rect(0, Screen.height - 256, 256, 256), map, mat);
		GUI.Box(new Rect(0, Screen.height - 256, 256, 256), "", GUI.skin.GetStyle("Frame"));
		#endregion
		
		#region center block
		GUI.Box(new Rect(256, Screen.height - 200, Screen.width - 512, 200), "", GUI.skin.GetStyle("Line"));
		#endregion
		
		#region right block
		if(pictureSelectObject != null) GUI.DrawTexture(new Rect(Screen.width - 256, Screen.height - 256, 256, 256), pictureSelectObject);
		else GUI.DrawTexture(new Rect(Screen.width - 256, Screen.height - 256, 256, 256), pictureDefault);
		
		GUI.Box(new Rect(Screen.width - 256, Screen.height - 256, 256, 256), "", GUI.skin.GetStyle("Frame"));
		#endregion
		
		#region up block
		GUI.Box(new Rect(Screen.width - 200, 0, 200, 30), "");
		GUI.Label(new Rect(Screen.width - 255, 3, 200, 30), "Деньги " + money.ToString(), GUI.skin.label);
		GUI.Label(new Rect(Screen.width - 165, 3, 200, 30), "Очки " + score.ToString(), GUI.skin.label);
		
		if(GUI.Button(new Rect(0, 0, 100, 25), "Меню"))
		{
			_GM.pause = true;
		}
		#endregion
	}*/

    public void ButtonMenu ()
    {
        _GM.pause = true;
    }
}
