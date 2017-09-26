using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Weapons
{
    public class RailgunShell : Round, IShell
    {
        private Rigidbody body;
        public void StatUp()
        {
            this.speed = 300f;
            this.damage = 500f;
            this.armorPiersing = 6f;
            body.mass = 40f;

            this.ttl = 5f;
            body = this.GetComponent<Rigidbody>();
            body.AddForce(transform.forward * Speed, ForceMode.VelocityChange);
            this.canRicochet = true;
        }

        public void StatUp(float speed, float damage, float armorPiersing, float mass, bool canRicochet, GameObject explosionPrefab)
        {
            this.speed = speed;
            this.damage = damage;
            this.armorPiersing = armorPiersing;
            this.canRicochet = canRicochet;
            this.ttl = 5f;
            body = this.GetComponent<Rigidbody>();

            body.mass = mass;
            body.AddForce(transform.forward * Speed, ForceMode.VelocityChange);
        }

        public override void Update()
        {
            body.AddTorque(0, 0, 55, ForceMode.Acceleration);
            if (ttl > 0)
                ttl -= Time.deltaTime;
            else
                Destroy();
        }
        public override void Destroy()
        {
            Destroy(this.gameObject);
        }
    }
}
