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
        public float TurnSpeed;
        public float ScaleSpeed; // Скорость волны
        private float liveTime;

        protected void Start()       {        }
        public void StatUp(EnergyType type)
        {
            damage = 10f; // + 10 along second
            speed = 50;
            ttl = 8f;
            liveTime = ttl;
            armorPiersing = 2;
            this.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.Impulse);
        }
        public new void Update()
        {

            if (target != null)
            {
                transform.localScale += new Vector3(ScaleSpeed, ScaleSpeed, ScaleSpeed);
                gameObject.GetComponent<Rigidbody>().AddForce((target.position - this.transform.position).normalized * Speed, ForceMode.Force);
            }
            else
                gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.Force);
            if (ttl < 0 || damage < 1)
                Destroy();
            else
                ttl -= Time.deltaTime;
        }
        protected override void OnCollisionEnter(Collision collision) { }
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

        protected override void Destroy()
        {
            Destroy(this.gameObject);
        }
    }
}

