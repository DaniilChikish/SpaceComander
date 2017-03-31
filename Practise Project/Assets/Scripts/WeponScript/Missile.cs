using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class Missile : MonoBehaviour
    {
        public Transform target;// цель для ракеты       
        public GameObject Blast;// префаб взрыва           
        public float Speed;// скорость ракеты           
        public float DropImpulse;//импульс сброса          
        public float TurnSpeed;// скорость поворота ракеты            
        public float explosionTime;// длительность жизни
        public float AimCone;
        private float lt;//продолжительность жизни

        public void Start()
        {
            gameObject.GetComponent<Rigidbody>().AddForce(-transform.up * DropImpulse, ForceMode.Impulse);
            lt = 0;
        }
        public void Update()
        {
            if (target != null)//наведение
            {
                Quaternion targetRotation = Quaternion.LookRotation(target.position - this.transform.position, new Vector3(0, 1, 0));
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);
                // угол между направлением на цель и направлением ракеты порядок имеет значение.
                if (Vector3.Angle(target.transform.position - this.transform.position, this.transform.forward) > AimCone)
                    target = null;
            }
            //полет по прямой
            float multiplicator = Mathf.Pow((lt * 0.05f), (1 / 4));
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed * multiplicator, ForceMode.VelocityChange);

            if (lt > explosionTime)
                Explode();
            else
                lt += Time.deltaTime;
        }
        public void Explode()
        {
            Instantiate(Blast, this.transform.position, this.transform.rotation);
            Destroy(gameObject);
        }

        // взрываем при коллизии
        public void OnCollisionEnter()
        {
            if (lt > explosionTime / 20)
                Explode();
        }
        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}
