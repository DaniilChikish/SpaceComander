using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    class InterceptorMissile : SelfguidedMissile
    {
        protected override void Start()
        {
            base.Start();
            Type = MissileType.Interceptor;
            gameObject.GetComponent<Rigidbody>().AddForce(-transform.up * DropImpulse, ForceMode.Impulse);
            lifeTime = 0;
        }
        protected override void Explode()
        {
            GameObject blast = Instantiate(Global.ExplosiveBlast, this.transform.position, this.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.Missile);
            Destroy(gameObject);
        }
    }
}
