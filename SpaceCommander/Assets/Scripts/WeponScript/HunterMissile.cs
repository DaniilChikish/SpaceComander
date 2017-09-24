using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    class HunterMissile : SelfguidedMissile
    {
        public override void Start()
        {
            type = MissileType.Hunter;
            Speed = 40f;
            DropImpulse = 400f;
            TurnSpeed = 120;
            explosionTime = 35f;
            AimCone = 30;
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * DropImpulse, ForceMode.Impulse);
            lt = 0;
        }
        protected override void Explode()
        {
            GameObject blast = Instantiate(FindObjectOfType<GlobalController>().ExplosiveBlast, this.transform.position, this.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.Missile);
            Destroy(gameObject);
        }
    }
}
