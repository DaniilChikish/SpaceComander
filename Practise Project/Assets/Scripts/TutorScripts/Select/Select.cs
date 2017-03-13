using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Select : MonoBehaviour {
	
	public bool isSelect;
	private float _selX;
	private float _selY;
	private float _selWidth;
	private float _selHeight;
	
	private float _selXOld;
	private float _selYOld;
	
	public Texture2D texSelect;
	
	public GUISkin mainSkin;
	public int numDepth = 1;

    public List<Position> pos;
	
	private Vector3 _startPoint;
	private Vector3 _endPoint;
	private Ray _ray;
	private RaycastHit _hit;
	
	private GlobalDB _GDB;

	// Use this for initialization
	void Start () 
	{
		_GDB = GameObject.FindGameObjectWithTag("MainUI").GetComponent<GlobalDB>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonDown(0))
		{
			isSelect = true;
			_selXOld = Input.mousePosition.x;
			_selYOld = Input.mousePosition.y;
			
			_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(_ray, out _hit, 10000.0f))
			{
				_startPoint = _hit.point;
			}
		}
		if(Input.GetMouseButtonUp(0))
		{
			ClearSelect();
			
			isSelect = false;
			
			_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(_ray, out _hit, 10000.0f))
			{
				_endPoint = _hit.point;
			}
			FindSelect();
		}
		
		if(isSelect)
		{
			_selX = Input.mousePosition.x;
			_selY = Screen.height - Input.mousePosition.y;
			_selWidth = _selXOld - Input.mousePosition.x;
			_selHeight = Input.mousePosition.y - _selYOld;
		}
        if (Input.GetMouseButtonDown(1))
        {
            if(_GDB.selectList.Count != 0)
            {
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out _hit, 10000.0f))
                {
                    if (_hit.transform.gameObject.tag != "Untagged")
                    {
                        if (_hit.transform.gameObject.tag == "Enemy")
                            StateInSelect(_hit.transform);
                        else
                            StateInSelect(_hit.point);
                    }
                }
            }
        }
	}
	
	void OnGUI ()
	{
		if(isSelect)
		{
			GUI.DrawTexture(new Rect(_selX, _selY, _selWidth, _selHeight), texSelect);
		}
		
		GUI.depth = numDepth;
		GUI.skin = mainSkin;
			
		GUI.BeginGroup(new Rect((Screen.width - 650)/2, Screen.height - 210, 650, 210));
		
		if(_GDB.selectList.Count == 1)
		{
			GUI.Box(new Rect(250, 80, 150, 30), "");
			GUI.Box(new Rect(250, 80, 150*_GDB.selectList[0].GetComponent<Stats>().lengthHealth, 30), "");
			
			GUI.Label(new Rect(250, 110, 100, 30), "Dam " + _GDB.selectList[0].GetComponent<Stats>().damage.ToString());
			GUI.Label(new Rect(360, 110, 100, 30), "Prot " + _GDB.selectList[0].GetComponent<Stats>().protect.ToString());
		}
		
		if(_GDB.selectList.Count > 1)
		{
			GUI.DrawTexture(new Rect(0, 20, 90, 90), _GDB.selectList[0].GetComponent<Stats>().icon);
			GUI.DrawTexture(new Rect(110, 20, 90, 90), _GDB.selectList[1].GetComponent<Stats>().icon);
			if(_GDB.selectList.Count > 2)
				GUI.DrawTexture(new Rect(220, 20, 90, 90), _GDB.selectList[2].GetComponent<Stats>().icon);
			if(_GDB.selectList.Count > 3)
				GUI.DrawTexture(new Rect(330, 20, 90, 90), _GDB.selectList[3].GetComponent<Stats>().icon);
			if(_GDB.selectList.Count > 4)
				GUI.DrawTexture(new Rect(440, 20, 90, 90), _GDB.selectList[4].GetComponent<Stats>().icon);
			if(_GDB.selectList.Count > 5)
				GUI.DrawTexture(new Rect(550, 20, 90, 90), _GDB.selectList[5].GetComponent<Stats>().icon);
			if(_GDB.selectList.Count > 6)
				GUI.DrawTexture(new Rect(0, 112, 90, 90), _GDB.selectList[6].GetComponent<Stats>().icon);
			if(_GDB.selectList.Count > 7)
				GUI.DrawTexture(new Rect(110, 112, 90, 90), _GDB.selectList[7].GetComponent<Stats>().icon);
			if(_GDB.selectList.Count > 8)
				GUI.DrawTexture(new Rect(220, 112, 90, 90), _GDB.selectList[8].GetComponent<Stats>().icon);
			if(_GDB.selectList.Count > 9)
				GUI.DrawTexture(new Rect(330, 112, 90, 90), _GDB.selectList[9].GetComponent<Stats>().icon);
			if(_GDB.selectList.Count > 10)
				GUI.DrawTexture(new Rect(440, 112, 90, 90), _GDB.selectList[10].GetComponent<Stats>().icon);
			if(_GDB.selectList.Count > 11)
				GUI.DrawTexture(new Rect(550, 112, 90, 90), _GDB.selectList[11].GetComponent<Stats>().icon);
		}
		GUI.EndGroup();
	}
	
	// Выделяет попавшие в зону объекты
	void FindSelect ()
	{	
		foreach(GameObject obj in _GDB.dwarfList)
		{
			float x = obj.transform.position.x;
			float z = obj.transform.position.z;
			if((x > _startPoint.x && x < _endPoint.x) || (x < _startPoint.x && x > _endPoint.x))
			{
				if((z > _startPoint.z && z < _endPoint.z) || (z < _startPoint.z && z > _endPoint.z))
				{
					if(!FindInSelectList(obj) && _GDB.selectList.Count < 12)
						obj.GetComponent<Stats>().SelectPlayer();
				}
			}
		}
        if(_GDB.selectList.Count != 0)
        {
            if (_GDB.activeObjectInterface != null)
                _GDB.DeactivationInterface();
        }
	}
	
	// Проверяет присутствует-ли объект в списке selectList
	bool FindInSelectList (GameObject obj)
	{
		foreach(GameObject selObj in _GDB.selectList)
		{
			if(selObj == obj)
				return true;
		}
		return false;
	}
	
	// Очистка выделенных юнитив
	public void ClearSelect ()
	{
		foreach(GameObject selObj in _GDB.selectList)
		{
			selObj.GetComponent<Stats>().SelectPlayer();
		}
		_GDB.selectList.Clear();
	}

    void StateInSelect (Transform targetGO)
    {
        foreach (GameObject obj in _GDB.selectList)
        {
            obj.GetComponent<Stats>().instruction = Stats.enInstruction.attack;
            obj.GetComponent<Stats>().targetTransform = targetGO;
        }
    }

    void StateInSelect (Vector3 targetVec) 
    {
        int i = 0;
        foreach (GameObject obj in _GDB.selectList)
        {
            obj.GetComponent<Stats>().instruction = Stats.enInstruction.move;
            obj.GetComponent<Stats>().targetVector = new Vector3(targetVec.x + pos[i].x, targetVec.y, targetVec.z + pos[i].z);
            i++;
        }
    }
}

[System.Serializable]
public class Position
{
    public float x;
    public float z;
}
