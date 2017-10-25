using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    public class PlazmaSphere : Round, IEnergy
    {
        // цель для ракеты
        public Transform target;
        // скорость поворота
        public float scaleFactor; // Скорость волны
        private float liveTime;
        private Rigidbody body;
        private float maxDamage;

        protected void Start()
        {
            body = this.gameObject.GetComponent<Rigidbody>();
        }
        public void StatUp(EnergyType type)
        {
            body = this.gameObject.GetComponent<Rigidbody>();
            damage = 10f; // + 10 along second
            maxDamage = damage;
            speed = 100;
            ttl = 8f;
            liveTime = ttl;
            armorPiersing = 2;
            body.AddForce(this.gameObject.transform.forward * Speed * body.mass, ForceMode.Impulse);
        }
        public new void Update()
        {
            transform.localScale = new Vector3(scaleFactor * (damage / maxDamage), scaleFactor * (damage / maxDamage), scaleFactor * (damage / maxDamage));
            body.mass = scaleFactor * (damage / maxDamage) * 10;
            damage -= maxDamage * (1 / liveTime) * Time.deltaTime;
            if (target != null)
                body.AddForce((target.position - this.transform.position).normalized * Speed, ForceMode.Acceleration);
            else
                body.AddForce(transform.forward * Speed, ForceMode.Acceleration);
            if (ttl < 0 || damage < 1)
                Destroy();
            else
                ttl -= Time.deltaTime;
        }
        protected void OnTriggerEnter(Collider collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Shell":
                    {
                        Debug.Log("Triggered shell");
                        damage -= collision.gameObject.GetComponent<Rigidbody>().mass;
                        Destroy(collision.gameObject);
                        break;
                    }
            }
        }
        protected void OnTriggerStay(Collider collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Unit":
                    {
                        damage -= damage * Time.deltaTime;
                        break;
                    }
                case "Shell":
                    {
                        Debug.Log("Triggered shell");
                        damage -= collision.gameObject.GetComponent<Rigidbody>().mass;
                        Destroy(collision.gameObject);
                        break;
                    }
            }
        }
        protected override void OnCollisionEnter(Collision collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Shell":
                    {
                        damage -= collision.gameObject.GetComponent<Rigidbody>().mass;
                        Destroy(collision.gameObject);
                        break;
                    }
            }
        }
        protected void OnCollisionStay(Collision collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Unit":
                    {
                        damage -= damage * Time.deltaTime;
                        break;
                    }
            }
        }
        public virtual float GetEnergy()
        {
            float output = damage * 0.3f;
            damage -= output;
            return output;
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public override void Destroy()
        {
            Destroy(this.gameObject);
        }
    }
}

