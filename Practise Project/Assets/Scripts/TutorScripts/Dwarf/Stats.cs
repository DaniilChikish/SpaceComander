using UnityEngine;
using System.Collections;

public class Stats : MonoBehaviour {
	
	public float curHealth;
	public float maxHealth;
	public float lengthHealth;
	public int protect;
	public int damage;
	
	public Texture2D icon;

    public enum enInstruction
    {
        idle,
        move,
        attack
    }
    public enInstruction instruction;
    public Transform targetTransform;
    public Vector3 targetVector;
	
	private GlobalDB _GDB;

	// Use this for initialization
	void Start () 
	{
		_GDB = GameObject.FindGameObjectWithTag("MainUI").GetComponent<GlobalDB>();
		_GDB.dwarfList.Add(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if(curHealth <= 0)
			Dead();
		lengthHealth = curHealth / maxHealth;
	}
	
	void Dead ()
	{
		_GDB.dwarfList.Remove(gameObject);
		// дописать удаление из листа выделенных
	}
	
	public void SelectPlayer ()
	{
		Projector proj = transform.Find("Projector").GetComponent<Projector>();
		if(proj.enabled == false)
		{
			proj.enabled = true;
			_GDB.selectList.Add(gameObject);
		}
		else 
		{
			proj.enabled = false;
		}
	}

    public void ReceiptDamage (int dam)
    {
        curHealth = Mathf.Max(curHealth - (dam - protect), 0);
    }

    public void Healing(int health)
    {
        curHealth = Mathf.Min(curHealth + health, maxHealth);
    }
}
