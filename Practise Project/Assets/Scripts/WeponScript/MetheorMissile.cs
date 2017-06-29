using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander
{
    class MetheorMissile : SelfguidedMissile
    {
        public override void Start()
        {
            type = MissileType.Metheor;
            Speed = 100f;
            DropImpulse = 250f;
            TurnSpeed = 4;
            explosionTime = 30f;
            AimCone = 45;
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
