using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    class UnitaryTorpedo : SelfguidedMissile
    {
        protected override void Start()
        {
            base.Start();
            body.AddForce(-transform.up * dropImpulse, ForceMode.Impulse);
        }
        public override void Explode()
        {
            GameObject blast = Instantiate(Global.Prefab.ExplosiveBlast, this.transform.position, this.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.UnitaryTorpedo);
            Destroy(gameObject);
        }
    }
}
