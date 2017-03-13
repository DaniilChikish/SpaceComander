using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Shop : MonoBehaviour 
{
	public Texture2D tex;
	
	public string nameShop;
	public int xp;
	public int protection;
	public int lvl;
	
	public int costUp;
	
	public GameObject monster1;
	public GameObject monster2;
	public GameObject monster3;
	
	public GameObject startPoint;
	
	public GameObject flag;
	public GameObject curFlag;
	
	private GlobalDB _GDB;
	private Select _SEL;

    private Text nameText;
    private Text hpText;
    private Text protectionText;
    private Text lvlText;

    private GameObject uiObject;

    private Text uiCostUp;

    private Button monster1Button;
    private Button monster2Button;
    private Button monster3Button;

    // Use this for initialization
    void Start () 
	{
		_GDB = GameObject.FindGameObjectWithTag("MainUI").GetComponent<GlobalDB>();
		_SEL = GameObject.FindGameObjectWithTag("MainUI").GetComponent<Select>();

        uiObject = GameObject.FindGameObjectWithTag("CanvasMid").transform.FindChild("Shop").gameObject;

        nameText = uiObject.transform.FindChild("TextStats").FindChild("TextNameValue").GetComponent<Text>();
        hpText = uiObject.transform.FindChild("TextStats").FindChild("TextHPValue").GetComponent<Text>();
        protectionText = uiObject.transform.FindChild("TextStats").FindChild("TextProtectionValue").GetComponent<Text>();
        lvlText = uiObject.transform.FindChild("TextStats").FindChild("TextLVLValue").GetComponent<Text>();

        monster1Button = uiObject.transform.FindChild("Monsters").FindChild("Monster1").FindChild("Button").GetComponent<Button>();
        monster2Button = uiObject.transform.FindChild("Monsters").FindChild("Monster2").FindChild("Button").GetComponent<Button>();
        monster3Button = uiObject.transform.FindChild("Monsters").FindChild("Monster3").FindChild("Button").GetComponent<Button>();

        uiCostUp = uiObject.transform.FindChild("Up").FindChild("Text").GetComponent<Text>();
    }

    void Update()
    {
        nameText.text = nameShop;
        hpText.text = xp.ToString();
        protectionText.text = protection.ToString();
        lvlText.text = lvl.ToString();

        uiCostUp.text = costUp.ToString();
    }
	
	void OnMouseDown ()
	{
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            _SEL.ClearSelect();
            if (_GDB.activeObjectInterface != null)
                _GDB.DeactivationInterface();
            _GDB.activeObjectInterface = gameObject;
            VisibleApply(true);
            GameObject.FindGameObjectWithTag("MainUI").GetComponent<BackgroudUI>().pictureSelectObject = tex;

            Button btn = uiObject.transform.FindChild("Up").FindChild("ButtonUp").GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(ButtonUp);
            btn = uiObject.transform.FindChild("Delete").FindChild("ButtonDelete").GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(ButtonDelete);
            btn = uiObject.transform.FindChild("Flag").FindChild("ButtonFlag").GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(ButtonFlag);

            monster1Button.onClick.AddListener(ButtonMonster1);
            monster2Button.onClick.AddListener(ButtonMonster2);
            monster3Button.onClick.AddListener(ButtonMonster3);
        }
	}

    public void VisibleApply(bool stat)
    {
        if (stat)
        {
            uiObject.SetActive(true);
        }
        else
        {
            uiObject.SetActive(false);
        }
    }

    public void ButtonUp()
    {
        if (_GDB.activeObjectInterface != gameObject)
            return;
        if (_GDB.money >= costUp && lvl < 3)
        {
            if (lvl == 1)
            {
                lvl++;
                xp += 300;
                protection += 3;
                costUp += 200;
                monster2Button.interactable = true;
            }
            else if (lvl == 2)
            {
                lvl++;
                xp += 500;
                protection += 5;
                monster3Button.interactable = true;
            }
            _GDB.money -= costUp;
        }
    }

    public void ButtonDelete()
    {
        if (_GDB.activeObjectInterface != gameObject)
            return;
        _GDB.obj.Remove(gameObject);
        uiObject.SetActive(false);
        Destroy(gameObject);
    }

    public void ButtonFlag ()
    {
        if (curFlag != null)
        {
            _GDB.obj.Remove(curFlag);
            Destroy(curFlag);
        }
        Camera.main.GetComponent<BuildManager>().setBuild(flag, gameObject.GetComponent<Shop>());
    }

    public void ButtonMonster1 ()
    {
        Instantiate(monster1, startPoint.transform.position, startPoint.transform.rotation);
    }

    public void ButtonMonster2()
    {
        Instantiate(monster2, startPoint.transform.position, startPoint.transform.rotation);
    }

    public void ButtonMonster3()
    {
        Instantiate(monster3, startPoint.transform.position, startPoint.transform.rotation);
    }
}
