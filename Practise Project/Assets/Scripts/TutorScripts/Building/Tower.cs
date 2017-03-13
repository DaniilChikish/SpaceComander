using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tower : MonoBehaviour 
{
	public Texture2D tex;
	
	public string nameTower;
	public int xp;
	public int protection;
	public int lvl;
	public int damage;
	
	public int costUp;
	
	public GameObject shell;
	public Transform startPoint;
	
	public float timer = 1;
	private float _timerDown = 0;
	
	private GlobalDB _GDB;
	private Select _SEL;

    private Text nameText;
    private Text hpText;
    private Text protectionText;
    private Text lvlText;
    private Text damageText;

    private GameObject uiObject;

    private Text uiCostUp;

    void Awake ()
	{
		_GDB = GameObject.FindGameObjectWithTag("MainUI").GetComponent<GlobalDB>();
		_SEL = GameObject.FindGameObjectWithTag("MainUI").GetComponent<Select>();

        uiObject = GameObject.FindGameObjectWithTag("CanvasMid").transform.FindChild("Tower").gameObject;

        nameText = uiObject.transform.FindChild("TextStats").FindChild("TextNameValue").GetComponent<Text>();
        hpText = uiObject.transform.FindChild("TextStats").FindChild("TextHPValue").GetComponent<Text>();
        protectionText = uiObject.transform.FindChild("TextStats").FindChild("TextProtectionValue").GetComponent<Text>();
        lvlText = uiObject.transform.FindChild("TextStats").FindChild("TextLVLValue").GetComponent<Text>();
        damageText = uiObject.transform.FindChild("TextStats").FindChild("TextDamageValue").GetComponent<Text>();

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
            GameObject.FindGameObjectWithTag("MainUI").GetComponent<BackgroudUI>().pictureSelectObject = tex;

            Button btn = uiObject.transform.FindChild("Up").FindChild("ButtonUp").GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(ButtonUp);
            btn = uiObject.transform.FindChild("Delete").FindChild("ButtonDelete").GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(ButtonDelete);
        }
    }
	
	// Update is called once per frame
	void Update () {
		if(_timerDown > 0)
			_timerDown -= Time.deltaTime;

		if(_GDB.enemyList.Count > 0 && _timerDown <= 0)
		{
			float _enemyDist = -1;
			GameObject _enemyNear = null;
			foreach(var enemy in _GDB.enemyList)
			{
				float _tempDist = Vector3.Distance(enemy.transform.position, transform.position);
				if(_tempDist < 20 && (_tempDist < _enemyDist || _enemyDist == -1))
				{
					_enemyDist = _tempDist;
					_enemyNear = enemy;
				}
			}
			if(_enemyDist != -1)
			{
				_timerDown = timer;
				GameObject bul = (GameObject)Instantiate(shell, startPoint.transform.position, transform.rotation);
				bul.GetComponent<Shell>().target = _enemyNear.transform;
			}
		}

        nameText.text = nameTower;
        hpText.text = xp.ToString();
        protectionText.text = protection.ToString();
        lvlText.text = lvl.ToString();
        damageText.text = damage.ToString();

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
                damage += 50;
                protection += 3;
                costUp += 200;
            }
            else
            if (lvl == 2)
            {
                lvl++;
                xp += 500;
                damage += 50;
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
