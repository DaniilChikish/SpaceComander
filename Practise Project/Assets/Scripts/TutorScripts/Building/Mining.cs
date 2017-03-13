using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Mining : MonoBehaviour 
{
	public Texture2D tex;
	
	public string nameMining;
	public int xp;
	public int protection;
	public int lvl;
	public int income;
	
	public int costUp;
	
	public float timer;
	private float _timerDown;
	
	private GlobalDB _GDB;
	private Select _SEL;

    private Text nameText;
    private Text hpText;
    private Text protectionText;
    private Text lvlText;
    private Text incomingText;

    private GameObject uiObject;

    private Text uiCostUp;

    // Use this for initialization
    void Start () 
	{
		_timerDown = timer;
		_GDB = GameObject.FindGameObjectWithTag("MainUI").GetComponent<GlobalDB>();
		_SEL = GameObject.FindGameObjectWithTag("MainUI").GetComponent<Select>();

        uiObject = GameObject.FindGameObjectWithTag("CanvasMid").transform.FindChild("Mine").gameObject;

        nameText = uiObject.transform.FindChild("TextStats").FindChild("TextNameValue").GetComponent<Text>();
        hpText = uiObject.transform.FindChild("TextStats").FindChild("TextHPValue").GetComponent<Text>();
        protectionText = uiObject.transform.FindChild("TextStats").FindChild("TextProtectionValue").GetComponent<Text>();
        lvlText = uiObject.transform.FindChild("TextStats").FindChild("TextLVLValue").GetComponent<Text>();
        incomingText = uiObject.transform.FindChild("TextStats").FindChild("TextIncomeValue").GetComponent<Text>();

        uiCostUp = uiObject.transform.FindChild("Up").FindChild("Text").GetComponent<Text>();
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
            GameObject.FindGameObjectWithTag("MainUI").GetComponent<BackgroudUI>().pictureSelectObject = tex;   // изменить

            Button btn = uiObject.transform.FindChild("Up").FindChild("ButtonUp").GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(ButtonUp);
            btn = uiObject.transform.FindChild("Delete").FindChild("ButtonDelete").GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(ButtonDelete);
        }
    }
	
	// Update is called once per frame
	void Update () 
	{
		if(_timerDown < 0)
		{
			_GDB.money += income;
			_timerDown = timer;
		} else
			_timerDown -= Time.deltaTime;

        nameText.text = nameMining;
        hpText.text = xp.ToString();
        protectionText.text = protection.ToString();
        lvlText.text = lvl.ToString();
        incomingText.text = income.ToString();

        uiCostUp.text = costUp.ToString();
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
                income += 50;
                protection += 3;
                costUp += 200;
            }
            else
            if (lvl == 2)
            {
                lvl++;
                xp += 500;
                income += 50;
                protection += 5;
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
}
