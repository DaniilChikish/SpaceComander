using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    public class CannonShell : Round, IShell
    {
        protected bool explosive;
        protected GameObject explosionPrefab;
        public void StatUp(Vector3 speed, float damage, float armorPiersing, float mass, bool canRicochet)
        {
            StatUp(speed, damage, armorPiersing, mass, canRicochet, null, 5f);
        }
        public void StatUp(Vector3 speed, float damage, float armorPiersing, float mass, bool canRicochet, GameObject explosionPrefab)
        {
            StatUp(speed, damage, armorPiersing, mass, canRicochet, explosionPrefab, 5f);
        }
        public void StatUp(Vector3 speed, float damage, float armorPiersing, float mass, bool canRicochet, GameObject explosionPrefab, float ttl)
        {
            this.speed = speed.magnitude;
            this.damage = damage;
            this.armorPiersing = armorPiersing;
            this.canRicochet = canRicochet;
            if (explosionPrefab != null)
            {
                this.explosionPrefab = explosionPrefab;
                explosive = true;
            }
            this.ttl = ttl;
            this.GetComponent<Rigidbody>().mass = mass;

            this.GetComponent<Rigidbody>().AddForce(speed, ForceMode.VelocityChange);
        }
        public override void Destroy()
        {
            if (explosive)
            {
                GameObject blast = Instantiate(explosionPrefab, gameObject.transform.position, gameObject.transform.rotation);
                if (GetComponent<Rigidbody>().mass >= 10)
                    blast.GetComponent<Explosion>().StatUp(BlastType.ExplosiveShell);
                else blast.GetComponent<Explosion>().StatUp(BlastType.Shell);
            }
            Destroy(this.gameObject);
        }
        protected override void OnCollisionEnter(Collision collision)
        {
            if (explosive) Destroy();
            else if (!canRicochet || (collision.gameObject.tag == "Unit" && armorPiersing - collision.gameObject.GetComponent<Unit>().ShellResist >= 0)) Destroy();
        }

    }
}
