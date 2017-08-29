using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Weapons
{
    class BombardierMissile : Torpedo
    {
        protected override void Start()
        {
            Speed = 80f;// скорость ракеты      
            TurnSpeed = 5f;// скорость поворота ракеты            
            DropImpulse = 200f;//импульс сброса                  
            explosionRange = 1f; //расстояние детонации
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * DropImpulse, ForceMode.Impulse);
            Global = FindObjectOfType<GlobalController>();
            lt = 0;
        }
        public override void Explode()
        {
            GameObject blast = Instantiate(FindObjectOfType<GlobalController>().ExplosiveBlast, this.transform.position, this.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.Missile);
            Destroy(gameObject);
        }
    }
}
