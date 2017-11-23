using SpaceCommander.General;
using SpaceCommander.Mechanics;
using UnityEngine;

namespace SpaceCommander.Mechanics.Weapons
{
    public class MagnetoShell : Round, IShell
    {
        private Rigidbody body;
        public void StatUp()
        {
            this.speed = 600f;
            this.damage = 100f;
            this.armorPiersing = 9f;
            body.mass = 20f;

            this.ttl = 2f;
            body = this.GetComponent<Rigidbody>();
            body.AddForce(transform.forward * Speed, ForceMode.VelocityChange);
        }
        public override void Update()
        {
            damage = damage - (damage / 2 * Time.deltaTime);
            body.mass = body.mass - (body.mass / 2 * Time.deltaTime);
        }
        public void StatUp(Vector3 speed, float damage, float armorPiersing, float mass, bool canRicochet)
        {
            this.speed = speed.magnitude;
            this.damage = damage;
            this.armorPiersing = armorPiersing;
            this.canRicochet = canRicochet;
            this.ttl = 2f;
            body = this.GetComponent<Rigidbody>();

            body.mass = mass;
            body.AddForce(speed, ForceMode.VelocityChange);
        }
        public override void Destroy()
        {
            Destroy(this.gameObject);
        }

        public void StatUp(Vector3 speed, float damage, float armorPiersing, float mass, bool canRicochet, GameObject explosionPrefab)
        {
            StatUp(speed, damage, armorPiersing, mass, canRicochet);
        }

        public void StatUp(Vector3 speed, float damage, float armorPiersing, float mass, bool canRicochet, GameObject explosionPrefab, float ttl)
        {
            StatUp(speed, damage, armorPiersing, mass, canRicochet);
        }
    }
}