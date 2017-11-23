using SpaceCommander.Mechanics;
using UnityEngine;

namespace SpaceCommander.Mechanics.Weapons
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
            GameObject blast = Instantiate(General.GlobalController.Instance.Prefab.NukeBlast, this.transform.position, this.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.NukeTorpedo);
            Destroy(gameObject);
        }
    }
}
