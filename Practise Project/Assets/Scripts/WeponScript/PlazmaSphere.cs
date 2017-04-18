using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
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
            damage = 20f; // + 20 along second
            speed = 70;
            ttl = 8f;
            liveTime = ttl;
            armorPiersing = 2;
            this.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.Impulse);
        }
        public new void Update()
        {
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.Force);

            if (target != null && ttl < liveTime * 0.9)
            {
                transform.localScale += new Vector3(ScaleSpeed, ScaleSpeed, ScaleSpeed);
                Quaternion targetRotation = Quaternion.LookRotation(target.position - this.transform.position, new Vector3(0, 1, 0));
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);
            }
            if (ttl < 0 || damage < 1)
                Explode();
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

        protected override void Explode()
        {
            Destroy(this.gameObject);
        }
    }
}

