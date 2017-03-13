using UnityEngine;
using System.Collections;

public class StatsEnemy : MonoBehaviour {

    public float curHealth;
    public float maxHealth;
    public float lengthHealth;
    public int protect;
    public int damage;

    private GlobalDB _GDB;

	// Use this for initialization
	void Start () {
        _GDB = GameObject.FindGameObjectWithTag("MainUI").GetComponent<GlobalDB>();
        _GDB.enemyList.Add(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        if (curHealth <= 0)
            Dead();
	}

    void Dead()
    {
        _GDB.enemyList.Remove(gameObject);
        Destroy(gameObject);
    }

    public void ReceivingDamage (int recDamage)
    {
        curHealth -= recDamage * ((100 - protect) / 100f);
    }
}
