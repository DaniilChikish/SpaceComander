using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    public class CannonShell : Round, IShell
    {
        protected bool canRicochet;
        protected bool explosive;
        protected GameObject explosionPrefab;
        public void StatUp(float speed, float damage, float armorPiersing, float mass, bool canRicochet, GameObject explosionPrefab)
        {
            this.speed = speed;
            this.damage = damage;
            this.armorPiersing = armorPiersing;
            this.canRicochet = canRicochet;
            if (explosionPrefab != null)
            {
                this.explosionPrefab = explosionPrefab;
                explosive = true;
            }
            this.ttl = 5f;
            this.GetComponent<Rigidbody>().mass = mass;

            this.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.VelocityChange);
        }
        protected override void Destroy()
        {
            if (explosive)
            Instantiate(explosionPrefab, gameObject.transform.position, gameObject.transform.rotation);

            Destroy(this.gameObject);
        }
        protected override void OnCollisionEnter(Collision collision)
        {
            if (explosive) Destroy();
            else if (! canRicochet && collision.gameObject.tag == "Unit" && armorPiersing - collision.gameObject.GetComponent<Unit>().ShellResist > -2) Destroy();
        }

    }
}
