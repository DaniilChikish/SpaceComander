using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    public class LaserImpulse : MonoBehaviour, IEnergy
    {
        public float damage;
        public float armorPiersing;
        public EnergyType type;

        public float Speed { get { return 1000; } }

        public float Damage { get { return damage; } }

        public float ArmorPiersing { get { return armorPiersing; } }

        // Use this for initialization
        protected void Start()       {       }
        public void StatUp(EnergyType type)
        {
            this.type = type;
            switch (type)
            {
                case EnergyType.RedRay:
                    {
                        damage = 10;
                        armorPiersing = 4;
                        break;
                    }
                case EnergyType.GreenRay:
                    {
                        damage = 20;
                        armorPiersing = 2;
                        break;
                    }
                case EnergyType.BlueRay:
                    {
                        damage = 25;
                        armorPiersing = 1;
                        break;
                    }
            }
            //gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.VelocityChange);
        }
        private void Update()
        {

        }
        public virtual float GetEnergy()
        {
            switch (type)
            {
                case EnergyType.RedRay:
                    {
                        return damage * 0.8f * Time.deltaTime;
                    }
                case EnergyType.GreenRay:
                    {
                        return damage * 0.4f * Time.deltaTime;
                    }
                case EnergyType.BlueRay:
                    {
                        return damage * 0.2f * Time.deltaTime;
                    }
                default:
                    return 0;
            }
        }
    }
}
