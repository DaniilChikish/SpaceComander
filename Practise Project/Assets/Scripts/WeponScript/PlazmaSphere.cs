using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class PlazmaSphere : Round
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
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.Force);

            if (target != null && ttl < liveTime * 0.7)
            {
                transform.localScale += new Vector3(ScaleSpeed, ScaleSpeed, ScaleSpeed);
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

