using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Weapons
{
    public class RailgunShell : Round
    {
        private Rigidbody body;
        public void StatUp()
        {
            this.speed = 300f;
            this.damage = 200f;
            this.armorPiersing = 6f;
            this.ttl = 1.5f;
            body = this.GetComponent<Rigidbody>();
            body.mass = 15f;
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
        protected override void Destroy()
        {
            Destroy(this.gameObject);
        }
    }
}
