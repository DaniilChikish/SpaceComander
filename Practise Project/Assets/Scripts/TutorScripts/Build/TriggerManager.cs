using UnityEngine;
using System.Collections;

public class TriggerManager : MonoBehaviour {
	
	private GlobalDB _GDB;
	public bool staticTrigger;

	// Use this for initialization
	void Start () {
		_GDB = GameObject.FindGameObjectWithTag("MainUI").GetComponent<GlobalDB>();
		if(!staticTrigger)
			_GDB.obj.Add(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerEnter (Collider col)
	{
		if(col.tag == "CurBuild")
			_GDB.numIntersection++;
	}
	
	void OnTriggerExit (Collider col)
	{
		if(col.tag == "CurBuild")
			_GDB.numIntersection--;
	}
}
