using UnityEngine;
using System.Collections;

public class BuildManager : MonoBehaviour {
	
	private GameObject _currentBuild;
	public LayerMask mask;
	
	private GlobalDB _GDB;

	// Use this for initialization
	void Start () {
		_GDB = GameObject.FindGameObjectWithTag("MainUI").GetComponent<GlobalDB>();
	}
	
	// Update is called once per frame
	void Update () {
		if(_currentBuild != null)
		{
			Ray ray;
			RaycastHit hit;
			
			ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, 10000.0f, mask))
			{
				_currentBuild.transform.position = hit.point;
			}
			
			if(Input.GetMouseButtonDown(0) && _GDB.numIntersection == 0)
			{
				if(_currentBuild.name == "Tower(Clone)")
					_currentBuild.GetComponent<Tower>().enabled = true;
				_currentBuild.tag = "Untagged";
				_currentBuild = null;
				_GDB.deactivationTrigger();
			}
		}
	}
	
	public void setBuild (GameObject go, Shop sh = null)
	{
		_currentBuild = (GameObject)Instantiate(go);
		_currentBuild.tag = "CurBuild";
		if(sh != null)
			sh.curFlag = _currentBuild;
		_GDB.activationTrigger();
	}
}
