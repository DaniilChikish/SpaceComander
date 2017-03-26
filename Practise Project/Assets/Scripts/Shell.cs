using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class Shell : MonoBehaviour
    {
        public WeaponType Type;
        private float speed;
        private float damage;
        private float ttl;
        public float Speed { get { return speed; } }
        public float Damage { get { return damage; } }
        public GameObject Blast;
        // Use this for initialization
        void Start()
        {
            switch (Type)
            {
                case WeaponType.Cannon:
                    {
                        damage = 30;
                        speed = 7500;
                        ttl = 2;
                        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed);
                        break;
                    }
                case WeaponType.Laser:
                    {
                        damage = 50;
                        speed = 20000;
                        ttl = 1.5f;
                        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed);
                        break;
                    }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (ttl > 0)
                ttl -= Time.deltaTime;
            else
                Explode();
        }
        private void OnCollisionEnter(Collision collision)
        {
            Explode();
        }
        private void Explode()
        {
            Destroy(this.gameObject);
        }
    }
}
