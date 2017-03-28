using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class LaserBeam : Shell
    {
        // Use this for initialization
        protected override void Start()
        {
            //damage = 50;
            //speed = 20000;
            //ttl = 1.5f;
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed);
        }
    }
}
