using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Magic : MonoBehaviour
{
    public Texture2D tex;

    public string nameMagic;
    public int xp;
    public int protection;
    public int lvl;

    public int costUp;

    public float reloadMagic1;
    public float reloadMagic2;
    public float reloadMagic3;

    private float curReloadMagic1;
    private float curReloadMagic2;
    private float curReloadMagic3;

    public int healMagic1;
    public bool isActiveMagic2 = false;

    private GlobalDB _GDB;
    private Select _SEL;

    private Text nameText;
    private Text hpText;
    private Text protectionText;
    private Text lvlText;

    private Button magic1;
    private Button magic2;
    private Button magic3;

    private Image magic1Image;
    private Image magic2Image;
    private Image magic3Image;

    private GameObject uiObject;

    private Text uiCostUp;

    // Use this for initialization
    void Start()
    {
        _GDB = GameObject.FindGameObjectWithTag("MainUI").GetComponent<GlobalDB>();
        _SEL = GameObject.FindGameObjectWithTag("MainUI").GetComponent<Select>();

        uiObject = GameObject.FindGameObjectWithTag("CanvasMid").transform.FindChild("Magic").gameObject;

        nameText = uiObject.transform.FindChild("TextStats").FindChild("TextNameValue").GetComponent<Text>();
        hpText = uiObject.transform.FindChild("TextStats").FindChild("TextHPValue").GetComponent<Text>();
        protectionText = uiObject.transform.FindChild("TextStats").FindChild("TextProtectionValue").GetComponent<Text>();
        lvlText = uiObject.transform.FindChild("TextStats").FindChild("TextLVLValue").GetComponent<Text>();

        lvlText = uiObject.transform.FindChild("TextStats").FindChild("TextLVLValue").GetComponent<Text>();

        magic1 = uiObject.transform.parent.parent.FindChild("Magic").FindChild("ButtonMagic1").GetComponent<Button>();
        magic2 = uiObject.transform.parent.parent.FindChild("Magic").FindChild("ButtonMagic2").GetComponent<Button>();
        magic3 = uiObject.transform.parent.parent.FindChild("Magic").FindChild("ButtonMagic3").GetComponent<Button>();

        magic1Image = magic1.transform.FindChild("Reload").GetComponent<Image>();
        magic2Image = magic2.transform.FindChild("Reload").GetComponent<Image>();
        magic3Image = magic3.transform.FindChild("Reload").GetComponent<Image>();

        magic1.interactable = true;

        uiCostUp = uiObject.transform.FindChild("Up").FindChild("Text").GetComponent<Text>();
    }

    void OnMouseDown()
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

            magic1.onClick.RemoveAllListeners();
            magic1.onClick.AddListener(Magic1Activate);

            magic2.onClick.RemoveAllListeners();
            magic2.onClick.AddListener(Magic2Activate);

            magic3.onClick.RemoveAllListeners();
            magic3.onClick.AddListener(Magic3Activate);
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

    void Update()
    {
        nameText.text = nameMagic;
        hpText.text = xp.ToString();
        protectionText.text = protection.ToString();
        lvlText.text = lvl.ToString();

        uiCostUp.text = costUp.ToString();

        // 1
        if (curReloadMagic1 > 0)
        {
            curReloadMagic1 -= Time.deltaTime;
            magic1Image.fillAmount = curReloadMagic1 / reloadMagic1;
        }
        else if (curReloadMagic1 < 0)
        {
            curReloadMagic1 = 0;
        }

        // 2
        if (curReloadMagic2 > 0)
        {
            curReloadMagic2 -= Time.deltaTime;
            magic2Image.fillAmount = curReloadMagic2 / reloadMagic2;
        }
        else if (curReloadMagic2 < 0)
        {
            curReloadMagic2 = 0;
        }

        // 3
        if (curReloadMagic3 > 0)
        {
            curReloadMagic3 -= Time.deltaTime;
            magic3Image.fillAmount = curReloadMagic3 / reloadMagic3;
        }
        else if (curReloadMagic3 < 0)
        {
            curReloadMagic3 = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Magic1Activate();
        if (Input.GetKeyDown(KeyCode.Alpha2) && lvl >= 2)
            Magic2Activate();
        if (Input.GetKeyDown(KeyCode.Alpha3) && lvl >= 3)
            Magic3Activate();

        if (isActiveMagic2 && Input.GetMouseButtonDown(0))
        {
            Ray ray;
            RaycastHit hit;

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 10000.0f))
            {
                if(hit.transform.tag == "Dwarf")
                {
                    hit.transform.gameObject.AddComponent<DefenceUnit>();
                    curReloadMagic2 = reloadMagic2;
                    isActiveMagic2 = false;
                }
            }
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
                magic2.interactable = true;
            }
            else if (lvl == 2)
            {
                lvl++;
                xp += 500;
                protection += 5;
                magic3.interactable = true;
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

    public void Magic1Activate ()
    {
        // хил всех
        if (curReloadMagic1 > 0)
            return;

        foreach (var i in _GDB.dwarfList)
        {
            i.GetComponent<Stats>().Healing(healMagic1);
        }
        curReloadMagic1 = reloadMagic1;
    }

    public void Magic2Activate()
    {
        // броня на одного
        if (curReloadMagic2 > 0)
            return;

        isActiveMagic2 = true;
    }

    public void Magic3Activate()
    {
        if (curReloadMagic3 > 0)
            return;

        curReloadMagic3 = reloadMagic3;
    }
}

