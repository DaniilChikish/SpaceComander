using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    class ThunderbolthHeavyRocket : SelfguidedMissile
    {
        private float fragRate = 32f;
        private float dispersion = 16f;
        private float fragSpeed = 200f;
        protected override void Start()
        {
            base.Start();
            gameObject.GetComponent<Rigidbody>().AddForce(-transform.up * dropImpulse, ForceMode.Impulse);
            Global = FindObjectOfType<GlobalController>();
            lifeTime = 0;
        }
        public override void Explode()
        {
            GameObject blast = Instantiate(FindObjectOfType<GlobalController>().ExplosiveBlast, this.transform.position, this.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.Missile);
            float damage, armorPiersing, mass;
            bool canRicochet = true;
            GameObject explosionPrefab = null;
            damage = 30f;
            armorPiersing = 4;
            mass = 4f;
            Quaternion dispersionDelta;
            for (int i = 0; i < fragRate; i++)
            {
                dispersionDelta = Weapon.RandomDirectionNormal(dispersion, Global);
                GameObject shell = Instantiate(Global.UnitaryShell, gameObject.transform.position, this.transform.rotation * dispersionDelta);
                shell.GetComponent<IShell>().StatUp(body.velocity + (fragSpeed * (dispersionDelta * this.transform.forward)), damage, armorPiersing, mass, canRicochet, explosionPrefab);
            }
            Destroy(gameObject);
        }
    }
}
