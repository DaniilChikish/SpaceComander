using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Weapons
{
    class NukeTorpedo : SelfguidedMissile
    {
        protected override void Start()
        {
            base.Start();
            gameObject.GetComponent<Rigidbody>().mass = 300;
            gameObject.GetComponent<Rigidbody>().AddForce(-transform.up * dropImpulse, ForceMode.Impulse);
        }
        public override void Explode()
        {
            GameObject blast = Instantiate(GlobalController.GetInstance().Prefab.NukeBlast, this.transform.position, this.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.NukeTorpedo);
            Destroy(gameObject);
        }
    }
}
