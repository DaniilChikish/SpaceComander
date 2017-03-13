using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Castle : MonoBehaviour {
	
	public bool visible = false;

	public Texture2D tex;
	
	public string nameCastle;
	public int xp;
	public int protection;
	public int lvl;
	
	public int costUp;
	public int costMining;
	public int costTower;
	public int costShop;
	public int costMagic;
	
	public GameObject mining;
	public GameObject tower;
	public GameObject shop;
	public GameObject magic;
	
	public GameObject lvl2;
	public GameObject lvl3;
	
	private GlobalDB _GDB;
	private Select _SEL;

    public Text nameText;
    public Text hpText;
    public Text protectionText;
    public Text lvlText;

    public GameObject uiObject;

    public Text uiCostUp;
    public Text uiCostMining;
    public Text uiCostTower;
    public Text uiCostShop;
    public Text uiCostMagic;

    // Use this for initialization
    void Start () {
		_GDB = GameObject.FindGameObjectWithTag("MainUI").GetComponent<GlobalDB>();
		_SEL = GameObject.FindGameObjectWithTag("MainUI").GetComponent<Select>();
	}
	
	// Update is called once per frame
	void Update () {
        nameText.text = nameCastle;
        hpText.text = xp.ToString();
        protectionText.text = protection.ToString();
        lvlText.text = lvl.ToString();

        if (!visible && uiObject.activeSelf)
            uiObject.SetActive(false);
        else if (visible && !uiObject.activeSelf)
            uiObject.SetActive(true);

        uiCostUp.text = costUp.ToString();
        uiCostMining.text = costMining.ToString();
        uiCostTower.text = costTower.ToString();
        uiCostShop.text = costShop.ToString();
        uiCostMagic.text = costMagic.ToString();
    }
	
	void OnMouseDown ()
	{
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            _SEL.ClearSelect();
            if (_GDB.activeObjectInterface != null)
                _GDB.DeactivationInterface();
            _GDB.activeObjectInterface = gameObject;
            visible = true;
            GameObject.FindGameObjectWithTag("MainUI").GetComponent<BackgroudUI>().pictureSelectObject = tex;
        }
	}

    void EnableUI (bool b)
    {
        uiObject.SetActive(b);
    }

    public void ButtonUp ()
    {
        if (_GDB.activeObjectInterface != gameObject)
            return;
        if (_GDB.money >= costUp && lvl < 3)
        {
            if (lvl == 1)
            {
                lvl++;
                lvl2.SetActive(true);
                xp += 500;
                protection += 5;
                costUp += 500;
            }
            else
            if (lvl == 2)
            {
                lvl++;
                lvl3.SetActive(true);
                xp += 700;
                protection += 8;
            }
            _GDB.money -= costUp;
        }
    }

    public void ButtonMine()
    {
        if (_GDB.money >= costMining)
        {
            Camera.main.GetComponent<BuildManager>().setBuild(mining);
            _GDB.money -= costMining;
        }
    }

    public void ButtonTower()
    {
        if (_GDB.money >= costTower)
        {
            Camera.main.GetComponent<BuildManager>().setBuild(tower);
            _GDB.money -= costTower;
        }
    }

    public void ButtonShop()
    {
        if (_GDB.money >= costShop)
        {
            Camera.main.GetComponent<BuildManager>().setBuild(shop);
            _GDB.money -= costShop;
        }
    }

    public void ButtonMagic()
    {
        if (_GDB.money >= costMagic)
        {
            Camera.main.GetComponent<BuildManager>().setBuild(magic);
            _GDB.money -= costMagic;
        }
    }

    /*void OnGUI ()
	{
		if(visible)
		{
			GUI.depth = numDepth;
			GUI.skin = mainSkin;
			
			GUI.BeginGroup(new Rect((Screen.width - 880)/2, Screen.height - 190, 880, 190));
			
			GUI.Label(new Rect(0, 30, 150, 30), "Название: " + nameCastle);
			GUI.Label(new Rect(0, 60, 150, 30), "Прочность: " + xp);
			GUI.Label(new Rect(0, 90, 150, 30), "Защита: " + protection);
			GUI.Label(new Rect(0, 120, 150, 30), "LVL: " + lvl);
			
			GUI.Box(new Rect(160, 10, 2, 160), "", GUI.skin.GetStyle("BackgroudMenu"));
			
			if(GUI.Button(new Rect(170, 0, 60, 150), "Up"))
			{
				if(_GDB.money >= costUp && lvl < 3)
				{
					if(lvl == 1)
					{
						lvl++;
						lvl2.SetActive(true);
						xp += 500;
						protection += 5;
						costUp += 500;
					} else
					if(lvl == 2)
					{
						lvl++;
						lvl3.SetActive(true);
						xp += 700;
						protection += 8;
					}
					_GDB.money -= costUp;
				}
			}
			GUI.Box(new Rect(170, 155, 60, 20), costUp.ToString());
			
			GUI.Box(new Rect(240, 10, 2, 160), "", GUI.skin.GetStyle("BackgroudMenu"));
			
			if(GUI.Button(new Rect(250, 0, 150, 150), "Добыча"))
			{
				if(_GDB.money >= costMining)
				{
					Camera.main.GetComponent<BuildManager>().setBuild(mining);
					_GDB.money -= costMining;
				}
			}
			GUI.Box(new Rect(250, 155, 150, 20), costMining.ToString());
			
			if(GUI.Button(new Rect(410, 0, 150, 150), "Башня"))
			{
				if(_GDB.money >= costTower)
				{
					Camera.main.GetComponent<BuildManager>().setBuild(tower);
					_GDB.money -= costTower;
				}
			}
			GUI.Box(new Rect(410, 155, 150, 20), costTower.ToString());
			
			if(GUI.Button(new Rect(570, 0, 150, 150), "Магазин"))
			{
				if(_GDB.money >= costShop)
				{
					Camera.main.GetComponent<BuildManager>().setBuild(shop);
					_GDB.money -= costShop;
				}
			}
			GUI.Box(new Rect(570, 155, 150, 20), costShop.ToString());
			
			if(GUI.Button(new Rect(730, 0, 150, 150), "Магия"))
			{
				if(_GDB.money >= costMagic)
				{
					Camera.main.GetComponent<BuildManager>().setBuild(magic);
					_GDB.money -= costMagic;
				}
			}
			GUI.Box(new Rect(730, 155, 150, 20), costMagic.ToString());
			
			GUI.EndGroup();
		}
	}*/
}
