using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Weapons
{
    public class MagnetoShell : Round, IShell
    {
        private Rigidbody body;
        public void StatUp()
        {
            this.speed = 600f;
            this.damage = 100f;
            this.armorPiersing = 9f;
            this.ttl = 2f;
            body = this.GetComponent<Rigidbody>();
            body.mass = 20f;
            body.AddForce(transform.forward * Speed, ForceMode.VelocityChange);
        }

        public void StatUp(float speed, float damage, float armorPiersing, float mass, bool canRicochet, GameObject explosionPrefab)
        {
            StatUp();
        }
        protected override void Destroy()
        {
            Destroy(this.gameObject);
        }
    }
}