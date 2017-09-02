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
            this.ttl = 5f;
            body = this.GetComponent<Rigidbody>();
            body.mass = 40f;
            body.AddForce(transform.forward * Speed, ForceMode.VelocityChange);
            this.canRicochet = true;
        }

        public void StatUp(float speed, float damage, float armorPiersing, float mass, bool canRicochet, GameObject explosionPrefab)
        {
            StatUp();
        }

        public override void Update()
        {
            body.AddTorque(0, 0, 55, ForceMode.Acceleration);
            if (ttl > 0)
                ttl -= Time.deltaTime;
            else
                Destroy();
        }
        protected override void Destroy()
        {
            Destroy(this.gameObject);
        }
    }
}
