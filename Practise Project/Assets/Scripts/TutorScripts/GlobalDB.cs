using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalDB : MonoBehaviour {
	
	public int money;
	public List<GameObject> obj = new List<GameObject>();
	public int numIntersection;
	public GameObject activeObjectInterface;
	public List<GameObject> enemyList;	// список врагов
	public List<GameObject> dwarfList;	// список гномов
	public List<GameObject> selectList; // спиков выделенных объектов

	// Use this for initialization
	void Start () {
        
    }
	
	public void activationTrigger ()
	{
		for(int i = 0; i < obj.Count; i++)
		{
			obj[i].GetComponent<BoxCollider>().isTrigger = true;
		}
	}
	
	public void deactivationTrigger ()
	{
		for(int i = 0; i < obj.Count; i++)
		{
			obj[i].GetComponent<BoxCollider>().isTrigger = false;
		}
	}
	
	public void DeactivationInterface ()
	{
		switch(activeObjectInterface.name)
		{
		    case "Castle":
			    activeObjectInterface.GetComponent<Castle>().visible = false;
		    break;
			
		    case "Mining(Clone)":
			    activeObjectInterface.GetComponent<Mining>().VisibleApply(false);
		    break;
			
		    case "Tower(Clone)":
			    activeObjectInterface.GetComponent<Tower>().VisibleApply(false);
		    break;
			
		    case "Shop(Clone)":
			    activeObjectInterface.GetComponent<Shop>().VisibleApply(false);
                break;

            case "Magic(Clone)":
                activeObjectInterface.GetComponent<Magic>().VisibleApply(false);
                break;
		}
	}
}
