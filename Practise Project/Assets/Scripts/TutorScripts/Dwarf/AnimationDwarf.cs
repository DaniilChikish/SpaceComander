using UnityEngine;
using System.Collections;

public class AnimationDwarf : MonoBehaviour {

    private ActiveState _AS;
    private Animator _anim;

	// Use this for initialization
	void Start () {
        _AS = gameObject.GetComponent<ActiveState>();
        _anim = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (_AS.animationState == ActiveState.enAnimation.idle)
        {
            _anim.SetBool("idle", true);
            _anim.SetBool("move", false);
            _anim.SetBool("attack", false);
        }
        if (_AS.animationState == ActiveState.enAnimation.move)
        {
            _anim.SetBool("idle", false);
            _anim.SetBool("move", true);
            _anim.SetBool("attack", false);
        }
        if (_AS.animationState == ActiveState.enAnimation.attact)
        {
            _anim.SetBool("idle", false);
            _anim.SetBool("move", false);
            _anim.SetBool("attack", true);
        }
	}
}
