using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Weapons
{
    class NukeTorpedo : Torpedo
    {
        protected override void Start()
        {
            Speed = 7f;// скорость ракеты      
            TurnSpeed = 5f;// скорость поворота ракеты            
            DropImpulse = 3000f;//импульс сброса                  
            explosionRange = 10f; //расстояние детонации
            gameObject.GetComponent<Rigidbody>().mass = 300;
            gameObject.GetComponent<Rigidbody>().AddForce(-transform.up * DropImpulse, ForceMode.Impulse);
            Global = FindObjectOfType<GlobalController>();
            lt = 0;
        }
        public override void Explode()
        {
            GameObject blast = Instantiate(Global.NukeBlast, this.transform.position, this.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.NukeTorpedo);
            Destroy(gameObject);
        }
    }
}
