using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class LaserBeam : Round, IEnergy
    {
        public EnergyType type;
        // Use this for initialization
        protected void Start()       {       }
        public void StatUp(EnergyType type)
        {
            this.type = type;
            switch (type)
            {
                case EnergyType.RedRay:
                    {
                        damage = 30;
                        speed = 550;
                        ttl = 1.5f;
                        armorPiersing = 4;
                        break;
                    }
                case EnergyType.GreenRay:
                    {
                        damage = 60;
                        speed = 450;
                        ttl = 1.5f;
                        armorPiersing = 2;
                        break;
                    }
                case EnergyType.BlueRay:
                    {
                        damage = 75;
                        speed = 400;
                        ttl = 1.5f;
                        armorPiersing = 1;
                        break;
                    }
            }
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.VelocityChange);
        }
        protected void OnCollisionExit(Collision collision)
        {
            Explode();
        }
        protected override void Explode()
        {
            Destroy(this.gameObject);
        }
        public virtual float GetEnergy()
        {
            switch (type)
            {
                case EnergyType.RedRay:
                    {
                        float output = damage * 0.8f;
                        damage -= output;
                        return output;
                    }
                case EnergyType.GreenRay:
                    {
                        float output = damage * 0.4f;
                        damage -= output;
                        return output;
                    }
                case EnergyType.BlueRay:
                    {
                        float output = damage * 0.2f;
                        damage -= output;
                        return output;
                    }
                default:
                    return 0;
            }
        }
    }
}
