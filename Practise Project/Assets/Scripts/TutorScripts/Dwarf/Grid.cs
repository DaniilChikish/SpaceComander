using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

    private Vector3 vec;

	// Use this for initialization
	void Start () {
        vec = transform.rotation.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Euler(vec);
	}
}
