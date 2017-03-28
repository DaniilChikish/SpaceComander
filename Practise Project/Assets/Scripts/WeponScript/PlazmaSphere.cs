using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class PlazmaSphere : Shell
    {
        // цель для ракеты
        public Transform target;
        // скорость поворота
        public float TurnSpeed;
        public float ScaleSpeed; // Скорость волны
        private float liveTime;

        protected override void Start()
        {
            //damage = 60;
            //speed = 6000;
            liveTime = ttl;
        }
        public new void Update()
        {
            if (ttl < liveTime * 0.9)
            {
                gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.Force);
                transform.localScale += new Vector3(ScaleSpeed, ScaleSpeed, ScaleSpeed);
            }
            if (target != null && ttl < liveTime * 0.8)
            {
                Quaternion targetRotation = Quaternion.LookRotation(target.position - this.transform.position, new Vector3(0, 1, 0));
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);
            }
            if (ttl < 0)
                Explode();
            else
                ttl -= Time.deltaTime;
        }

        internal void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}

