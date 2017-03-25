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
        private int ttl;
        public float Speed { get { return speed; } }
        public float Damage { get { return damage; } }
        // Use this for initialization
        void Start()
        {
            switch (Type)
            {
                case WeaponType.Cannon:
                    {
                        damage = 30;
                        speed = 7500;
                        ttl = 150;
                        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed);
                        break;
                    }
                case WeaponType.Laser:
                    {
                        damage = 50;
                        speed = 20000;
                        ttl = 150;
                        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed);
                        break;
                    }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (ttl > 0)
            {
                ttl--;
                //gameObject.GetComponent<Light>().intensity = gameObject.GetComponent<Light>().intensity * (ttl / 150);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        public void SetTarget(Vector3 target)
        {
            switch (Type)
            {
                case WeaponType.Missile:
                    {

                        break;
                    }
                case WeaponType.Torpedo:
                    {
                        break;
                    }
            }
        }

    }
}
