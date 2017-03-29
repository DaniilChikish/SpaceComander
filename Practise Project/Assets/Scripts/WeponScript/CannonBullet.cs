using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class CannonBullet : Round
    {
        //// Use this for initialization
        //protected override void Start()
        //{
        //    //damage = 30;
        //    //speed = 7500;
        //    //ttl = 2;
        //    gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed);
        //}
        protected override void OnCollisionEnter(Collision collision)
        {
        }
    }
}
