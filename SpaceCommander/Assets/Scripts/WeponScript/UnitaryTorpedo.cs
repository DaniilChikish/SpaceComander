using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    class UnitaryTorpedo : Torpedo
    {
        protected override void Start()
        {
            base.Start();
            gameObject.GetComponent<Rigidbody>().AddForce(-transform.up * DropImpulse, ForceMode.Impulse);
            Global = FindObjectOfType<GlobalController>();
            lt = 0;
        }
        public override void Explode()
        {
            GameObject blast = Instantiate(FindObjectOfType<GlobalController>().ExplosiveBlast, this.transform.position, this.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.UnitaryTorpedo);
            Destroy(gameObject);
        }
    }
}
