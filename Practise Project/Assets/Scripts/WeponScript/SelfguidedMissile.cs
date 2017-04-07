using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class SelfguidedMissile : MonoBehaviour
    {
        public Transform target;// цель для ракеты       
        //public GameObject Blast;// префаб взрыва           
        public float Speed;// скорость ракеты           
        public float DropImpulse;//импульс сброса          
        public float TurnSpeed;// скорость поворота ракеты            
        public float explosionTime;// длительность жизни
        public float AimCone;
        private float lt;//продолжительность жизни
        private float detonateTimer;
        private bool isArmed;

        public void Start()
        {
            gameObject.GetComponent<Rigidbody>().AddForce(-transform.up * DropImpulse, ForceMode.Impulse);
            lt = 0;
        }
        public void Update()
        {
            if (isArmed)
            {
                if (detonateTimer > 0)
                    detonateTimer -= Time.deltaTime;
                else Explode();
            }
            else
            {
                if (lt > explosionTime)
                    Explode();
                else
                    lt += Time.deltaTime;
            }
            if (lt > 0.5)//задержка старта
            {
                if (target != null)//наведение
                {
                    Quaternion targetRotation = Quaternion.LookRotation(target.position - this.transform.position, new Vector3(0, 1, 0));
                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);
                    // угол между направлением на цель и направлением ракеты порядок имеет значение.
                    if (Vector3.Angle(target.transform.position - this.transform.position, this.transform.forward) > AimCone)
                        target = null;
                }
                //Debug.Log(gameObject.GetComponent<Rigidbody>().velocity.magnitude);

                //полет по прямой
                float multiplicator = Mathf.Pow((lt * 0.05f), (1f / 4f));
                //Debug.Log(multiplicator);
                //Debug.Log(Convert.ToSingle(multiplicator));
                //gameObject.GetComponent<Rigidbody>().velocity = transform.forward * Speed * Convert.ToSingle(multiplicator); 
                gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed * multiplicator, ForceMode.Acceleration);
            }
        }
        private void Explode()
        {
            GameObject blast = Instantiate(FindObjectOfType<GlobalController>().ExplosiveBlast, this.transform.position, this.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.Missile);
            Destroy(gameObject);
        }
        public void Arm()
        {
            if (lt > 1.5)
            {
                isArmed = true;
                detonateTimer = 0.2f;
            }
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
