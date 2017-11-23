using SpaceCommander.Mechanics;
using UnityEngine;

namespace SpaceCommander.Mechanics.Weapons
{
    class ShieldBreakerTorpedo : SelfguidedMissile
    {
        protected override void Start()
        {
            base.Start();
            body.AddForce(-transform.up * dropImpulse, ForceMode.Impulse);
        }
        public override void Explode()
        {
            foreach (Unit x in Global.unitList)
            {
                if (Vector3.Distance(this.transform.position, x.transform.position) < 50 && x.ShieldForce > 1)
                    x.ShieldForce = -1;
            }
            Instantiate(Global.Prefab.EMIExplosionPrefab, this.transform.position, this.transform.rotation);
            GameObject blast = Instantiate(Global.Prefab.ExplosiveBlast, this.transform.position, this.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.Missile);
            Destroy(gameObject);
        }
    }
}
