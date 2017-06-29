using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander
{
    public class RailgunShell : Round, IShell
    {
        private Rigidbody body;
        ShellType type;
        public void StatUp(ShellType type)
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
                Explode();
        }
        protected override void Explode()
        {
            Destroy(this.gameObject);
        }
    }
}
