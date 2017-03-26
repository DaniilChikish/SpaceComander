using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class Missile : MonoBehaviour
    {
        // цель для ракеты
        public Transform target;
        // префаб взрыва
        public GameObject explosionPrefab;
        // скорость ракеты
        public float Speed;
        public float DropImpulse;
        // скорость поворота ракеты
        public float TurnSpeed;
        // время до взрыва
        public float explosionTime;
        private float lt;

        public void Start()
        {
            gameObject.GetComponent<Rigidbody>().AddForce(transform.right * DropImpulse, ForceMode.Impulse);
            lt = 0;
        }
        //private void ActivateTruster()
        //{
        //    trust = true;
        //    gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.);
        //}
        public void Update()
        {
            if (target != null)
            {
                Quaternion targetRotation = Quaternion.LookRotation(target.position - this.transform.position, new Vector3(0, 1, 0));
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);
                float multiplicator = Mathf.Pow((lt * 0.05f), (1 / 4));
                //this.transform.position += this.transform.forward * Speed * multiplicator * Time.deltaTime;
                gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed * multiplicator, ForceMode.Force);
            }
            if (lt > explosionTime)
                Explode();
            else
                lt += Time.deltaTime;
        }
        //public void Update()
        //{
        //    // уменьшаем таймер
        //    explosionTime -= Time.deltaTime;

        //    // если время таймера истекло, то взрываем ракету
        //    if (explosionTime <= 0)
        //    {
        //        Explode();
        //        return;
        //    }


        //    // величина движения вперед
        //    Vector3 movement = _thisTransform.forward * speed * Time.deltaTime;

        //    // если указана цель
        //    if (target != null)
        //    {
        //        // направление на цель
        //        Vector3 direction = target.position - _thisTransform.position;

        //        // максимальный угол поворота в текущий кадр
        //        float maxAngle = TurnSpeed * Time.deltaTime;

        //        // угол между направлением на цель и направлением ракеты
        //        float angle = Vector3.Angle(_thisTransform.forward, direction);

        //        if (angle <= maxAngle)
        //        {
        //            // угол меньше максимального, значит поворачиваем на цель
        //            _thisTransform.forward = direction.normalized;
        //        }
        //        else
        //        {
        //            //сферическая интерполяция направления с использованием максимального угла поворота
        //            _thisTransform.forward = Vector3.Slerp(_thisTransform.forward, direction.normalized, maxAngle / angle);
        //        }

        //        // расстояние до цели
        //        float distanceToTarget = direction.magnitude;

        //        // если расстояние мало, то создаем взрыв
        //        if (distanceToTarget < movement.magnitude)
        //        {
        //            Explode();
        //            return;
        //        }
        //    }

        //    // двигамет ракету вперед
        //    //_thisTransform.position += movement;
        //}

        public void Explode()
        {
            Instantiate(explosionPrefab, this.transform.position, this.transform.rotation);
            Destroy(gameObject);
        }

        // взрываем при коллизии
        public void OnCollisionEnter()
        {
            if (lt > explosionTime / 20)
                Explode();
        }

        internal void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}
