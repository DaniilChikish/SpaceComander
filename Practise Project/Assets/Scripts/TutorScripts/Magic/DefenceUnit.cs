using UnityEngine;
using System.Collections;

public class DefenceUnit : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Stats>().protect *= 2;
        Invoke("DestroyComponent", 5f);
        transform.localScale = new Vector3(2, 2, 2);
	}

    void DestroyComponent ()
    {
        GetComponent<Stats>().protect /= 2;
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        Destroy(this);
    }
}
